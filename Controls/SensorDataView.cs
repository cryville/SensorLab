using Android.Content;
using Android.Util;
using Android.Widget;

namespace SensorLab.Controls {
	public class SensorDataView : FrameLayout {
		TextView _textCaption;
		public string Caption {
			get => _textCaption.Text;
			set => _textCaption.Text = value;
		}

		TextView _textUnit;
		string m_Unit;
		public string Unit {
			get => m_Unit;
			set {
				m_Unit = value;
				if (string.IsNullOrEmpty(value))
					_textUnit.Text = "";
				else
					_textUnit.Text = string.Format(" {0}", value);
			}
		}

		TextView _textDataValue;
		public string DataValue {
			get => _textDataValue.Text;
			set => _textDataValue.Text = value;
		}

		TextView _textDataAccuracy;
		string m_dataAccuracy;
		public string DataAccuracy {
			get => m_dataAccuracy;
			set {
				m_dataAccuracy = value;
				if (string.IsNullOrEmpty(value))
					_textDataAccuracy.Text = "";
				else
					_textDataAccuracy.Text = "¦Ò" + value;
			}
		}

		public SensorDataView(Context context) : base(context) {
			InflateView(context, null);
		}

		public SensorDataView(Context context, IAttributeSet attrs) : base(context, attrs) {
			InflateView(context, attrs);
		}

		public SensorDataView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) {
			InflateView(context, attrs);
		}

		public SensorDataView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) {
			InflateView(context, attrs);
		}

		void InflateView(Context context, IAttributeSet attrs) {
			var view = Inflate(context, Resource.Layout.view_sensor_data, this);
			_textCaption = view.FindViewById<TextView>(Resource.Id.text_sensor_caption);
			_textUnit = view.FindViewById<TextView>(Resource.Id.text_sensor_unit);
			_textDataValue = view.FindViewById<TextView>(Resource.Id.text_sensor_data_value);
			_textDataAccuracy = view.FindViewById<TextView>(Resource.Id.text_sensor_data_accuracy);
			if (attrs != null) {
				var a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.SensorLab_Controls_SensorDataView, 0, 0);
				try {
					Caption = a.GetString(Resource.Styleable.SensorLab_Controls_SensorDataView_caption);
					Unit = a.GetString(Resource.Styleable.SensorLab_Controls_SensorDataView_unit);
				}
				finally {
					a.Recycle();
				}
			}
		}
	}
}
