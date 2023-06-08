using Android.Locations;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Cryville.Input;
using Cryville.Input.Xamarin.Android;
using SensorLab.Controls;
using System;

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

		internal void OnInput(InputIdentifier identifier, InputFrame frame) {
			var handler = identifier.Source.Handler;
			if (handler is AndroidLinearAccelerationHandler) _viewLinearAcceleration.DataValue = GetMagnitude(frame.Vector).ToString("F2");
			else if (handler is AndroidMagneticFieldHandler) _viewMagneticField.DataValue = GetMagnitude(frame.Vector).ToString("F1");
			else if (handler is AndroidGravityHandler) _viewGravity.DataValue = GetMagnitude(frame.Vector).ToString("F5");
			else if (handler is AndroidGyroscopeHandler) _viewGyroscope.DataValue = GetMagnitude(frame.Vector).ToString("F2");
		}

		internal void OnRotationInput(float[] orientation) {
			_viewDofAngles.DataValue = string.Format(
				"{0:F1} / {1:F1} / {2:F1}",
				180 / MathF.PI * orientation[0],
				180 / MathF.PI * orientation[1],
				180 / MathF.PI * orientation[2]
			);
		}

		internal void OnLocation(Location location) {
			_viewLocLatitude.DataValue = ToDMS(location.Latitude);
			_viewLocLongitude.DataValue = ToDMS(location.Longitude);
			_viewLocAltitude.DataValue = location.HasAltitude ? location.Altitude.ToString("F1") : "?";
			_viewLocAltitude.DataAccuracy = location.HasVerticalAccuracy ? location.VerticalAccuracyMeters.ToString("F1") : null;
			_viewLocBearing.DataValue = location.HasBearing ? location.Bearing.ToString("F1") : "?";
			_viewLocBearing.DataAccuracy = location.HasBearingAccuracy ? location.BearingAccuracyDegrees.ToString("F1") : null;
			_viewLocSpeed.DataValue = location.HasSpeed ? location.Speed.ToString("F1") : "?";
			_viewLocSpeed.DataAccuracy = location.HasSpeedAccuracy ? location.SpeedAccuracyMetersPerSecond.ToString("F1") : null;
			_viewLocAccuracy.DataValue = location.HasAccuracy ? location.Accuracy.ToString("F1") : "?";
			_viewLocTime.DataValue = DateTime.UnixEpoch.AddMilliseconds(location.Time).ToLocalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
		}

		static float GetMagnitude(InputVector vector) => MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);
		static string ToDMS(double value) {
			int d = (int)value;
			value *= 60; value %= 60;
			int m = (int)value;
			value *= 60; value %= 60;
			int s = (int)value;
			return string.Format("{0}¡ã{1}¡ä{2}¡å", d, m, s);
		}
	}
}
