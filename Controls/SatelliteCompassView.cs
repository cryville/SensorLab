using Android.Content;
using Android.Graphics;
using Android.Util;
using System;
using System.Collections.Generic;

namespace SensorLab.Controls {
	public class SatelliteCompassView : CompassView {
		static readonly Paint _fillId;
		static readonly Paint _fillSatellite;
		static readonly Color _colorFix;
		static readonly Color _colorEphemeris;
		static readonly Color _colorAlmanac;
		static readonly Color _colorUnknown;

		static SatelliteCompassView() {
			_fillId = new Paint { Color = Color.Green, TextSize = 12f, TextAlign = Paint.Align.Left, AntiAlias = true };
			_fillId.SetStyle(Paint.Style.Fill);
			_fillSatellite = new Paint { Color = Color.Green, AntiAlias = true };
			_fillSatellite.SetStyle(Paint.Style.Fill);
			_colorFix = Color.Green;
			_colorEphemeris = Color.Yellow;
			_colorAlmanac = Color.Red;
			_colorUnknown = Color.Gray;
		}

		public SatelliteCompassView(Context context) : base(context) { }
		public SatelliteCompassView(Context context, IAttributeSet attrs) : base(context, attrs) { }
		public SatelliteCompassView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
		public SatelliteCompassView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }

		internal Dictionary<SatelliteIdentifier, SatelliteStatus> Satellites;

		const float _satelliteSize = 0.02f;
		protected override void DrawContent(Canvas canvas, float flip) {
			for (float i = 30; i <= 90; i += 30) {
				DrawMagnitude(canvas, i / 90, (90 - i).ToString());
			}

			if (Satellites != null) {
				lock (Satellites) {
					foreach (var sat in Satellites) {
						var mag = _compassSize * (90 - sat.Value.Elevation) / 90;
						var ori = flip * (sat.Value.Azimuth - CompassRotation) / 180 * MathF.PI;
						Color color;
						if (sat.Value.IsUsedInFix) color = _colorFix;
						else if (sat.Value.HasEphemerisData) color = _colorEphemeris;
						else if (sat.Value.HasAlmanacData) color = _colorAlmanac;
						else color = _colorUnknown;
						_fillSatellite.Color = _fillId.Color = color;
						float x = mag * MathF.Sin(ori), y = -mag * MathF.Cos(ori);
						float satScale = MathF.Sqrt(sat.Value.CNR / 10);
						canvas.DrawCircle(x, y, _satelliteSize * satScale, _fillSatellite);
						DrawText(canvas, string.Format("{0}-{1}", sat.Key.Constellation, sat.Key.SVID), x + 0.02f * satScale, y - 0.01f * satScale, _fillId);
					}
				}
			}
		}
	}
}