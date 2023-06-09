using Android.Content;
using Android.OS;
using Android.Text;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Google.Android.Material.TextField;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SensorLab.Dialogs {
	public class PoiDialog : DialogFragment {
		const string _regexLocation = @"^(-)?([\d\.]+)('(([\d\.]+)('(([\d\.]+)'?)?)?)?)?\s+(-)?([\d\.]+)('(([\d\.]+)('(([\d\.]+)'?)?)?)?)?$";

		readonly ContentResolver _resolver;
		readonly int? _id;

		public PoiDialog(ContentResolver resolver, int? id) {
			_resolver = resolver;
			_id = id;
		}

		TextInputLayout _locationInputLayout;

		TextInputEditText _nameInput;
		TextInputEditText _locationInput;

		public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState) {
			var builder = new AlertDialog.Builder(Activity);
			var view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_poi, null);

			_locationInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.input_layout_poi_location);

			_nameInput = view.FindViewById<TextInputEditText>(Resource.Id.input_poi_name);
			_locationInput = view.FindViewById<TextInputEditText>(Resource.Id.input_poi_location);
			_locationInput.AfterTextChanged += LocationInput_AfterTextChanged;

			if (_id != null) {
				var c = _resolver.Query(PoiProvider.Uri, null, "id = ?", new string[] { _id.Value.ToString(CultureInfo.InvariantCulture) }, null);
				c.MoveToNext();
				_nameInput.Text = c.GetString(c.GetColumnIndex("name"));
				_locationInput.Text = ToLocationString(
					c.GetDouble(c.GetColumnIndex("latitude")),
					c.GetDouble(c.GetColumnIndex("longitude"))
				);
			}

			view.FindViewById<Button>(Resource.Id.btn_cancel).Click += CancelButton_Click;
			view.FindViewById<Button>(Resource.Id.btn_ok).Click += OkButton_Click;

			builder.SetView(view);
			return builder.Create();
		}

		private unsafe void LocationInput_AfterTextChanged(object sender, AfterTextChangedEventArgs e) {
			var m = Regex.Match(e.Editable.ToString(), _regexLocation);
			if (m.Success) _locationInputLayout.Error = null;
			else _locationInputLayout.Error = Resources.GetString(Resource.String.error_invalid_format);
		}

		private void CancelButton_Click(object sender, EventArgs e) {
			Dismiss();
		}

		private void OkButton_Click(object sender, EventArgs e) {
			var values = new ContentValues();
			values.Put("name", _nameInput.Text.Trim());
			values.Put("color", 0xffffff);
			try {
				ParseLocation(_locationInput.Text, out var lat, out var lon);
				values.Put("latitude", lat);
				values.Put("longitude", lon);
			}
			catch (FormatException) {
				return;
			}
			if (_id == null) {
				_resolver.Insert(PoiProvider.Uri, values);
			}
			else {
				_resolver.Update(PoiProvider.Uri, values, "id = ?", new string[] { _id.Value.ToString(CultureInfo.InvariantCulture) });
			}
			Dismiss();
		}

		static void ParseLocation(string str, out double lat, out double lon) {
			var m = Regex.Match(str, _regexLocation);
			if (!m.Success) throw new FormatException();
			static double ParseLocationComponent(Match match, int groupOffset) {
				double d = double.Parse(match.Groups[groupOffset + 1].Value, CultureInfo.InvariantCulture);
				double m = match.Groups[groupOffset + 4].Success ? double.Parse(match.Groups[groupOffset + 4].Value, CultureInfo.InvariantCulture) : 0;
				double s = match.Groups[groupOffset + 7].Success ? double.Parse(match.Groups[groupOffset + 7].Value, CultureInfo.InvariantCulture) : 0;
				return (d + m / 60d + s / 3600d) * (match.Groups[groupOffset].Success ? -1 : 1);
			}
			lat = ParseLocationComponent(m, 1);
			lon = ParseLocationComponent(m, 9);
		}
		static string ToLocationString(double lat, double lon) {
			static string ToLocationComponentString(double value) {
				int d = (int)value;
				value *= 60; value %= 60;
				int m = (int)value;
				value *= 60; value %= 60;
				double s = Math.Round(value, 3);
				return string.Format(CultureInfo.InvariantCulture, "{0}'{1}'{2}", d, m, s);
			}
			return string.Format("{0} {1}", ToLocationComponentString(lat), ToLocationComponentString(lon));
		}
	}
}