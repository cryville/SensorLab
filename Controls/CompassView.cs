using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;

namespace SensorLab.Controls {
	public abstract class CompassView : View {
		static readonly List<CompassView> _instances = new List<CompassView>();
		public static void Redraw() {
			foreach (var i in _instances) i.Invalidate();
		}

		protected static readonly Paint _strokeBg;
		protected static readonly Paint _fillText;

		static CompassView() {
			_strokeBg = new Paint { Color = Color.LightBlue, StrokeWidth = 0.02f, AntiAlias = true };
			_strokeBg.SetStyle(Paint.Style.Stroke);
			_fillText = new Paint { Color = Color.LightBlue, TextSize = 24f, TextAlign = Paint.Align.Center, AntiAlias = true };
			_fillText.SetStyle(Paint.Style.Fill);
		}

		public CompassView(Context context) : base(context) { _instances.Add(this); }
		public CompassView(Context context, IAttributeSet attrs) : base(context, attrs) { _instances.Add(this); }
		public CompassView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { _instances.Add(this); }
		public CompassView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { _instances.Add(this); }

		static float m_compassRotation;
		public static float CompassRotation {
			get => m_compassRotation;
			set {
				m_compassRotation = value;
				Redraw();
			}
		}

		internal static bool FlipRotation;

		protected sealed override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			var w = MeasureSpec.GetSize(widthMeasureSpec);
			var h = MeasureSpec.GetSize(heightMeasureSpec);
			var s = Math.Min(w, h);
			SetMeasuredDimension(s, s);
		}

		protected const float _compassSize = 0.84f;
		static readonly int[] _directions = new[] {
			Resource.String.dir_n, Resource.String.dir_ne,
			Resource.String.dir_e, Resource.String.dir_se,
			Resource.String.dir_s, Resource.String.dir_sw,
			Resource.String.dir_w, Resource.String.dir_nw,
		};

		protected sealed override void OnDraw(Canvas canvas) {
			base.OnDraw(canvas);
			float scale;
			if (canvas.Width > canvas.Height) scale = canvas.Height / 2f;
			else scale = canvas.Width / 2f;
			canvas.Scale(scale, scale);
			canvas.Translate(1, 1);
			_fillText.TextLocale = Java.Util.Locale.Default;
			float fcoef = FlipRotation ? -1 : 1;

			canvas.Rotate(-fcoef * CompassRotation, 0, 0);
			for (int i = 0; i < _directions.Length; i++) {
				canvas.DrawLine(0, -_compassSize, 0, -(_compassSize - 0.04f), _strokeBg);
				DrawText(canvas, Resources.GetString(_directions[i]), 0, -(_compassSize + 0.04f), _fillText);
				canvas.Rotate(fcoef / _directions.Length * 360, 0, 0);
			}
			canvas.Rotate(fcoef * CompassRotation, 0, 0);

			DrawContent(canvas, fcoef);
		}

		protected abstract void DrawContent(Canvas canvas, float flip);

		protected void DrawMagnitude(Canvas canvas, float magnitude, string caption = null) {
			canvas.DrawCircle(0, 0, magnitude * _compassSize, _strokeBg);
			if (string.IsNullOrEmpty(caption)) return;
			DrawText(canvas, caption, 0, magnitude * _compassSize - 0.02f, _fillText);
		}

		const float _textScale = 0.004f;
		protected void DrawText(Canvas canvas, string caption, float x, float y, Paint fill) {
			canvas.Save();
			canvas.Scale(_textScale, _textScale);
			canvas.DrawText(caption, x / _textScale, y / _textScale, fill);
			canvas.Restore();
		}
	}
}
