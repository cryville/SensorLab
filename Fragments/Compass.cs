using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using SensorLab.Controls;

namespace SensorLab.Fragments {
	public class Compass : Fragment {
		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		SensorDataView _viewLinearAcceleration;
		SensorDataView _viewMagneticField;
		SensorDataView _viewGravity;
		SensorDataView _viewGyroscope;
		SensorDataView _viewDofAngles;

		SensorDataView _viewLocLatitude;
		SensorDataView _viewLocLongitude;
		SensorDataView _viewLocAltitude;
		SensorDataView _viewLocBearing;
		SensorDataView _viewLocSpeed;
		SensorDataView _viewLocAccuracy;
		SensorDataView _viewLocTime;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			var view = inflater.Inflate(Resource.Layout.fragment_compass, container, false);

			_viewLinearAcceleration = view.FindViewById<SensorDataView>(Resource.Id.sensor_linear_acceleration);
			_viewMagneticField = view.FindViewById<SensorDataView>(Resource.Id.sensor_magnetic_field);
			_viewGravity = view.FindViewById<SensorDataView>(Resource.Id.sensor_gravity);
			_viewGyroscope = view.FindViewById<SensorDataView>(Resource.Id.sensor_gyroscope);
			_viewDofAngles = view.FindViewById<SensorDataView>(Resource.Id.dof_angles);

			_viewLocLatitude = view.FindViewById<SensorDataView>(Resource.Id.location_latitude);
			_viewLocLongitude = view.FindViewById<SensorDataView>(Resource.Id.location_longitude);
			_viewLocAltitude = view.FindViewById<SensorDataView>(Resource.Id.location_altitude);
			_viewLocBearing = view.FindViewById<SensorDataView>(Resource.Id.location_bearing);
			_viewLocSpeed = view.FindViewById<SensorDataView>(Resource.Id.location_speed);
			_viewLocAccuracy = view.FindViewById<SensorDataView>(Resource.Id.location_accuracy);
			_viewLocTime = view.FindViewById<SensorDataView>(Resource.Id.location_time);

			return view;
		}
	}
}
