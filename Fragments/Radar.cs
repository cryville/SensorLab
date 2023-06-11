using Android.Content;
using Android.Database;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using SensorLab.Controls;
using SensorLab.Dialogs;

namespace SensorLab.Fragments {
	public class Radar : Fragment {
		internal ContentResolver _resolver;

		RadarCompassView _compass;

		RecyclerView _poiList;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			var view = inflater.Inflate(Resource.Layout.fragment_radar, container, false);

			_compass = view.FindViewById<RadarCompassView>(Resource.Id.layout_compass);

			_resolver = Context.ContentResolver;

			view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_poi).Click += FabAddPoi_Click;

			_poiList = view.FindViewById<RecyclerView>(Resource.Id.list_poi);
			_poiList.SetAdapter(new PoiRecyclerViewAdapter(this));

			return view;
		}

		private void FabAddPoi_Click(object sender, System.EventArgs e) {
			OpenPoiDialog(null);
		}
		internal void OpenPoiDialog(int? id) {
			new PoiDialog(_resolver, id).Show(ParentFragmentManager, null);
		}

		internal void OnLocation(Location location) {
			_compass.Location = location;
		}
		internal void OnPoiUpdate(ICursor cursor) {
			_compass.ClearPois();
			var colName = cursor.GetColumnIndex("name");
			var colLatitude = cursor.GetColumnIndex("latitude");
			var colLongitude = cursor.GetColumnIndex("longitude");
			while (cursor.MoveToNext()) {
				_compass.AddPoi(cursor.GetString(colName), cursor.GetDouble(colLatitude), cursor.GetDouble(colLongitude));
			}
			_compass.EndAddPois();
		}
	}
	class PoiRecyclerViewAdapter : RecyclerView.Adapter {
		class ViewHolder : RecyclerView.ViewHolder {
			readonly PoiRecyclerViewAdapter _parent;
			public TextView Caption { get; private set; }
			public int Id { get; set; }
			public ViewHolder(PoiRecyclerViewAdapter parent, View view) : base(view) {
				_parent = parent;
				Caption = view.FindViewById<TextView>(Resource.Id.text_poi_caption);
				view.Click += View_Click;
			}
			private void View_Click(object sender, System.EventArgs e) {
				_parent._parent.OpenPoiDialog(Id);
			}
		}

		class Observer : ContentObserver {
			readonly PoiRecyclerViewAdapter _parent;
			public Observer(PoiRecyclerViewAdapter parent) : base(new Handler(Looper.MainLooper)) { _parent = parent; }
			public override void OnChange(bool selfChange) {
				base.OnChange(selfChange);
				_parent.RefreshData();
				_parent.NotifyDataSetChanged();
			}
		}

		readonly Radar _parent;
		readonly Observer _observer;
		ICursor _cursor;
		readonly int _colId;
		readonly int _colName;
		public PoiRecyclerViewAdapter(Radar parent) {
			_parent = parent;
			_observer = new Observer(this);
			RefreshData();
			_colId = _cursor.GetColumnIndex("id");
			_colName = _cursor.GetColumnIndex("name");
		}

		void RefreshData() {
			_cursor?.Close();
			_cursor = _parent._resolver.Query(PoiProvider.Uri, null, null, null, null);
			_parent.OnPoiUpdate(_cursor);
			_cursor.RegisterContentObserver(_observer);
		}

		public override int ItemCount => _cursor.Count;

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			return new ViewHolder(this, LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_poi, parent, false));
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			var h = (ViewHolder)holder;
			_cursor.MoveToPosition(position);
			h.Id = _cursor.GetInt(_colId);
			h.Caption.Text = _cursor.GetString(_colName);
		}
	}
}