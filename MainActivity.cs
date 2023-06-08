using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Java.Lang;
using Java.Util;

namespace SensorLab {
	[Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			RegisterForActivityResult(
				new ActivityResultContracts.RequestMultiplePermissions(),
				new ActivityResultHandler(this)
			).Launch(new string[] {
				Manifest.Permission.AccessCoarseLocation,
				Manifest.Permission.AccessFineLocation,
			});
		}

		void OnLocationPermissionRequested() {
			var navHostFrag = (NavHostFragment)SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment);
			var navCtrl = navHostFrag.NavController;
			var appBarConf = new AppBarConfiguration.Builder(navCtrl.Graph)
				.SetOpenableLayout(FindViewById<DrawerLayout>(Resource.Id.drawer))
				.Build();
			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			NavigationUI.SetupWithNavController(toolbar, navCtrl, appBarConf);
		}

		class ActivityResultHandler : Object, IActivityResultCallback {
			readonly MainActivity _activity;

			public ActivityResultHandler(MainActivity activity) { _activity = activity; }

			public void OnActivityResult(Object result) {
				IMap map = (IMap)result;
				_activity.OnLocationPermissionRequested();
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}
