using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.Util;
using System;
using System.Collections.Generic;

namespace SensorLab.Controls {
	public class RadarCompassView : CompassView {
		static readonly Paint _fillName;
		static readonly Paint _fillPoi;

		static RadarCompassView() {
			_fillName = new Paint { Color = Color.White, TextSize = 12f, TextAlign = Paint.Align.Left, AntiAlias = true };
			_fillName.SetStyle(Paint.Style.Fill);
			_fillPoi = new Paint { Color = Color.White, AntiAlias = true };
			_fillPoi.SetStyle(Paint.Style.Fill);
		}

		public RadarCompassView(Context context) : base(context) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs) : base(context, attrs) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { Init(); }
		public RadarCompassView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { Init(); }
		void Init() {
			UpdateBaseMagnitude();
		}

		struct PoiCache {
			public string name;
			public double latitude;
			public double longitude;
			public float distance;
			public float bearing;
		}

		bool _firstFix = false;
		readonly Location m_location = new Location((string)null);
		public Location Location {
			get => m_location;
			set {
				_firstFix = true;
				m_location.Set(value);
				_maxMagnitude = 0;
				for (int i = 0; i < _pois.Count; i++) {
					PoiCache poi = _pois[i];
					Location.DistanceBetween(m_location.Latitude, m_location.Longitude, poi.latitude, poi.longitude, _distBuffer);
					poi.distance = _distBuffer[0];
					poi.bearing = _distBuffer[1];
					if (_distBuffer[0] > _maxMagnitude) _maxMagnitude = _distBuffer[0];
					_pois[i] = poi;
				}
				UpdateBaseMagnitude();
				Invalidate();
			}
		}

		readonly List<PoiCache> _pois = new List<PoiCache>();
		public void ClearPois() {
			_pois.Clear();
			_maxMagnitude = 0;
		}
		readonly float[] _distBuffer = new float[3];
		public void AddPoi(string name, double lat, double lon) {
			Location.DistanceBetween(m_location.Latitude, m_location.Longitude, lat, lon, _distBuffer);
			_pois.Add(new PoiCache {
				name = name,
				latitude = lat,
				longitude = lon,
				distance = _distBuffer[0],
				bearing = _distBuffer[1]
			});
			if (_firstFix && _distBuffer[0] > _maxMagnitude) _maxMagnitude = _distBuffer[0];
		}
		public void EndAddPois() {
			UpdateBaseMagnitude();
			Invalidate();
		}

		int _magnitudeAlign = 10;
		double _maxMagnitude = 1000;
		int _magnitudes = 3;
		void UpdateBaseMagnitude() {
			if (_maxMagnitude <= 0) {
				_baseSqrtMagnitude = 0;
				return;
			}
			double a = Math.Sqrt(_maxMagnitude) / _magnitudes; // square root of target base magnitude
			a *= a; // target base magnitude
			double b = Math.Pow(_magnitudeAlign, Math.Ceiling(Math.Log(a, _magnitudeAlign))) / _magnitudeAlign; // aligned unit base magnitude
			double c = b;
			while (c < a) c += b; // base magnitude
			_baseSqrtMagnitude = Math.Sqrt(c);
		}
		double _baseSqrtMagnitude = 0;

		const float _poiSize = 0.02f;
		protected override void DrawContent(Canvas canvas, float flip) {
			for (int i = 1; i <= _magnitudes; i++) {
				double rmag = _baseSqrtMagnitude * i;
				DrawMagnitude(canvas, (float)i / _magnitudes, (rmag * rmag).ToString());
			}
			if (!_firstFix) return;
			foreach (var poi in _pois) {
				var mag = (float)(_compassSize * Math.Sqrt(poi.distance) / _magnitudes / _baseSqrtMagnitude);
				var ori = flip * (poi.bearing - CompassRotation) / 180 * MathF.PI;
				float x = mag * MathF.Sin(ori), y = -mag * MathF.Cos(ori);
				canvas.DrawCircle(x, y, _poiSize, _fillPoi);
				DrawText(canvas, poi.name, x + 0.02f, y - 0.01f, _fillName);
			}
		}
	}
}