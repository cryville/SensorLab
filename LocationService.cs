using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Lifecycle;

namespace SensorLab {
	[Register("world.cryville.sensorlab.LocationService")]
	public class LocationService : Service {
		public readonly static MutableLiveData LocationData = new MutableLiveData();

		const int _notificationId = 1;
		static string _notificationTitle;
		NotificationManager _ntfMgr;
		LocationManager _locMgr;
		LocationReceiver _locRecv;

		public override void OnCreate() {
			base.OnCreate();
			_ntfMgr = (NotificationManager)GetSystemService(NotificationService);
			_notificationTitle = Resources.GetString(Resource.String.app_name);
			_ntfMgr.CreateNotificationChannel(new NotificationChannel("location", Resources.GetString(Resource.String.location), NotificationImportance.Default));
			StartForeground(_notificationId, BuildNotification(Resources.GetString(Resource.String.location_fixing)));
		}
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			_locMgr = (LocationManager)GetSystemService(LocationService);
			_locRecv = new LocationReceiver(OnLocationChanged);
			_locMgr.RequestLocationUpdates(
				LocationManager.GpsProvider,
				new LocationRequest.Builder(1000)
					.SetQuality((int)LocationRequestQuality.HighAccuracy)
					.Build(),
				MainExecutor,
				_locRecv
			);
			return StartCommandResult.Sticky;
		}
		public override void OnDestroy() {
			base.OnDestroy();
			_locMgr.RemoveUpdates(_locRecv);
			StopForeground(StopForegroundFlags.Remove);
			StopSelf();
		}
		public override IBinder OnBind(Intent intent) => null;

		void OnLocationChanged(Location location) {
			_ntfMgr.Notify(_notificationId, BuildNotification(string.Format("{0} {1}",
				Util.ToDmsString(location.Latitude, "S", "N"),
				Util.ToDmsString(location.Longitude, "W", "E")
			)));
			LocationData.SetValue(location);
		}
		Notification BuildNotification(string text) => new NotificationCompat.Builder(this, "location")
			.SetContentTitle(_notificationTitle)
			.SetContentText(text)
			.SetOngoing(true)
			.SetSmallIcon(Resource.Mipmap.ic_launcher)
			.Build();
	}
}