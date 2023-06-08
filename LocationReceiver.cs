using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;

namespace SensorLab {
	internal class LocationReceiver : Java.Lang.Object, ILocationListener {
		readonly Action<Location> _cb;
		public LocationReceiver(Action<Location> cb) {
			_cb = cb;
		}

		public void OnLocationChanged(Location location) {
			_cb(location);
		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		[Obsolete]
		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
	}
}