using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;

namespace SensorLab.Controls {
	public class CompassView : View {
		static readonly List<CompassView> _instances = new List<CompassView>();
		public static void Redraw() {
			foreach (var i in _instances) i.Invalidate();
		}

		static readonly Paint _strokeBg;
		static readonly Paint _fillText;
		static readonly Paint _fillId;
		static readonly Paint _fillSatellite;
		static readonly Color _colorFix;
		static readonly Color _colorEphemeris;
		static readonly Color _colorAlmanac;
		static readonly Color _colorUnknown;

		static CompassView() {
			_strokeBg = new Paint { Color = Color.LightBlue, StrokeWidth = 0.02f, AntiAlias = true };
			_strokeBg.SetStyle(Paint.Style.Stroke);
			_fillText = new Paint { Color = Color.LightBlue, TextSize = 24f, TextAlign = Paint.Align.Center, AntiAlias = true };
			_fillText.SetStyle(Paint.Style.Fill);
			_fillId = new Paint { Color = Color.Green, TextSize = 12f, TextAlign = Paint.Align.Left, AntiAlias = true };
			_fillId.SetStyle(Paint.Style.Fill);
			_fillSatellite = new Paint { Color = Color.Green, AntiAlias = true };
			_fillSatellite.SetStyle(Paint.Style.Fill);
			_colorFix = Color.Green;
			_colorEphemeris = Color.Yellow;
			_colorAlmanac = Color.Red;
			_colorUnknown = Color.Gray;
		}

		public CompassView(Context context) : base(context) {
			_instances.Add(this);
		}

		public CompassView(Context context, IAttributeSet attrs) : base(context, attrs) {
			_instances.Add(this);
		}

		public CompassView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) {
			_instances.Add(this);
		}

		public CompassView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) {
			_instances.Add(this);
		}

		static float m_compassRotation;
		public static float CompassRotation {
			get => m_compassRotation;
			set {
				m_compassRotation = value;
				Redraw();
			}
		}

		internal static Dictionary<SatelliteIdentifier, SatelliteStatus> Satellites;
		internal static bool FlipRotation;

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			var w = MeasureSpec.GetSize(widthMeasureSpec);
			var h = MeasureSpec.GetSize(heightMeasureSpec);
			var s = Math.Min(w, h);
			SetMeasuredDimension(s, s);
		}

		const float _compassSize = 0.84f;
		const float _satelliteSize = 0.02f;
		static readonly int[] _directions = new[] {
			Resource.String.dir_n, Resource.String.dir_ne,
			Resource.String.dir_e, Resource.String.dir_se,
			Resource.String.dir_s, Resource.String.dir_sw,
			Resource.String.dir_w, Resource.String.dir_nw,
		};

		protected override void OnDraw(Canvas canvas) {
			base.OnDraw(canvas);
			float scale;
			if (canvas.Width > canvas.Height) scale = canvas.Height / 2f;
			else scale = canvas.Width / 2f;
			canvas.Scale(scale, scale);
			canvas.Translate(1, 1);
			_fillText.TextLocale = Java.Util.Locale.Default;
			float fcoef = FlipRotation ? -1 : 1;

			for (float i = 30; i <= 90; i += 30) {
				canvas.DrawCircle(0, 0, i / 90 * _compassSize, _strokeBg);
				canvas.Save();
				canvas.Scale(0.004f, 0.004f);
				canvas.DrawText((90 - i).ToString(), 0, (i / 90 * _compassSize - 0.02f) / 0.004f, _fillText);
				canvas.Restore();
			}

			canvas.Rotate(-fcoef * CompassRotation, 0, 0);
			for (int i = 0; i < _directions.Length; i++) {
				canvas.DrawLine(0, -_compassSize, 0, -(_compassSize - 0.04f), _strokeBg);
				canvas.Save();
				canvas.Scale(0.004f, 0.004f);
				canvas.DrawText(Resources.GetString(_directions[i]), 0, -(_compassSize + 0.04f) / 0.004f, _fillText);
				canvas.Restore();
				canvas.Rotate(fcoef / _directions.Length * 360, 0, 0);
			}
			canvas.Rotate(fcoef * CompassRotation, 0, 0);

			if (Satellites != null) {
				lock (Satellites) {
					foreach (var sat in Satellites) {
						var mag = _compassSize * (90 - sat.Value.Elevation) / 90;
						var ori = fcoef * (sat.Value.Azimuth - CompassRotation) / 180 * MathF.PI;
						Color color;
						if (sat.Value.IsUsedInFix) color = _colorFix;
						else if (sat.Value.HasEphemerisData) color = _colorEphemeris;
						else if (sat.Value.HasAlmanacData) color = _colorAlmanac;
						else color = _colorUnknown;
						_fillSatellite.Color = _fillId.Color = color;
						float x = mag * MathF.Sin(ori), y = -mag * MathF.Cos(ori);
						float satScale = MathF.Sqrt(sat.Value.CNR / 10);
						canvas.DrawCircle(x, y, _satelliteSize * satScale, _fillSatellite);
						canvas.Save();
						canvas.Scale(0.004f, 0.004f);
						canvas.DrawText(
							string.Format("{0}-{1}", sat.Key.Constellation, sat.Key.SVID),
							(x + 0.02f * satScale) / 0.004f, (y - 0.01f * satScale) / 0.004f, _fillId
						);
						canvas.Restore();
					}
				}
			}
		}
	}
}
