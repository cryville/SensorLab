using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace SensorLab.Fragments {
	public class Compass : Fragment {

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			return inflater.Inflate(Resource.Layout.fragment_compass, container, false);
		}
	}
}
