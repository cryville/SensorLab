using Android.Content;
using Android.Graphics;
using Android.Util;
using System;

namespace SensorLab.Controls {
	public class RadarCompassView : CompassView {
		public RadarCompassView(Context context) : base(context) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs) : base(context, attrs) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { Init(); }
		void Init() {
			UpdateBaseMagnitude();
		}

		int _magnitudeAlign = 10;
		double _maxMagnitude = 1000;
		int _magnitudes = 3;
		void UpdateBaseMagnitude() {
			double a = Math.Sqrt(_maxMagnitude) / _magnitudes; // square root of target base magnitude
			a *= a; // target base magnitude
			double b = Math.Pow(_magnitudeAlign, Math.Ceiling(Math.Log(a, _magnitudeAlign))) / _magnitudeAlign; // aligned unit base magnitude
			double c = b;
			while (c < a) c += b; // base magnitude
			_baseSqrtMagnitude = Math.Sqrt(c);
		}
		double _baseSqrtMagnitude = 0;
		protected override void DrawContent(Canvas canvas, float flip) {
			for (int i = 1; i <= _magnitudes; i++) {
				double rmag = _baseSqrtMagnitude * i;
				DrawMagnitude(canvas, (float)i / _magnitudes, (rmag * rmag).ToString());
			}
		}
	}
}