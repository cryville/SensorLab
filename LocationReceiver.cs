using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;

namespace SensorLab {
	internal class LocationReceiver : Java.Lang.Object, ILocationListener {
		public void OnLocationChanged(Location location) { }

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		[Obsolete]
		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
	}
}