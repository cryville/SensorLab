using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using Cryville.Input;
using Cryville.Input.Xamarin.Android;
using SensorLab.Controls;
using System;
using UnsafeIL;

namespace SensorLab.Fragments {
	public class Home : Fragment {
		ViewPager2 _viewPager;

		static bool _init;
		GnssStatusReceiver _gnssRecv;
		CompassView _compass;
		LocationManager _locMgr;
		LocationReceiver _locRecv;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			if (!_init) {
				_init = true;
				var param = new object[] { Activity };
				InputManager.HandlerRegistries.Add(typeof(AndroidAccelerometerHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidAccelerometerUncalibratedHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidGameRotationVectorHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidGravityHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidGyroscopeHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidGyroscopeUncalibratedHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidLinearAccelerationHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidMagneticFieldHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidMagneticFieldUncalibratedHandler), param);
				InputManager.HandlerRegistries.Add(typeof(AndroidRotationVectorHandler), param);
			}
			InputConsumer.Instance.OnInput += InputConsumer_OnInput;
			InputConsumer.Instance.Activate();

			_gnssRecv = new GnssStatusReceiver();
			_gnssRecv.SatelliteStatusChanged += OnSatelliteStatusChanged;
			_locMgr = (LocationManager)Activity.GetSystemService(Android.Content.Context.LocationService);
			_locRecv = new LocationReceiver();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			var view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
			_viewPager = view.FindViewById<ViewPager2>(Resource.Id.view_pager);
			_viewPager.Adapter = new ScreenSlidePagerAdapter(Activity);

			_compass = view.FindViewById<CompassView>(Resource.Id.layout_compass);
			CompassView.Satellites = _gnssRecv.ActiveSatellites;
			Start();

			return view;
		}

		class ScreenSlidePagerAdapter : FragmentStateAdapter {
			const int NUM_PAGES = 2;

			public ScreenSlidePagerAdapter(FragmentActivity fa) : base(fa) { }

			public override Fragment CreateFragment(int position) {
				return new Compass();
			}

			public override int ItemCount => NUM_PAGES;
		}

		readonly float[] _rvbuf = new float[4];
		readonly float[] _rmbuf = new float[9];
		readonly float[] _orbuf = new float[3];
		private unsafe void InputConsumer_OnInput(InputIdentifier identifier, InputFrame frame) {
			if (identifier.Source.Handler is AndroidRotationVectorHandler) {
				var vec = frame.Vector;
				fixed (float* ptr = _rvbuf) {
					Unsafe.CopyBlock(ptr, &vec, 4 * sizeof(float));
				}
				SensorManager.GetRotationMatrixFromVector(_rmbuf, _rvbuf);
				SensorManager.GetOrientation(_rmbuf, _orbuf);
				CompassView.CompassRotation = _orbuf[0] / MathF.PI * 180;
			}
			else if (identifier.Source.Handler is AndroidGravityHandler) {
				CompassView.FlipRotation = frame.Vector.Z < 0;
			}
		}

		public override void OnPause() {
			base.OnPause();
			Pause();
		}

		public override void OnResume() {
			base.OnResume();
			Start();
		}

		public override void OnDestroy() {
			base.OnDestroy();
			Pause();
		}

		void Start() {
			_locMgr.RequestLocationUpdates(
				LocationManager.GpsProvider,
				new LocationRequest.Builder(1000)
					.SetQuality((int)LocationRequestQuality.HighAccuracy)
					.Build(),
				Context.MainExecutor,
				_locRecv
			);
			_locMgr.RegisterGnssStatusCallback(_gnssRecv, null);
			InputConsumer.Instance.Activate();
		}

		void Pause() {
			InputConsumer.Instance.Deactivate();
			_locMgr.RemoveUpdates(_locRecv);
			_locMgr.UnregisterGnssStatusCallback(_gnssRecv);
		}

		private void OnSatelliteStatusChanged() {
			CompassView.Redraw();
		}
	}
}
