using System;

namespace SensorLab {
	public static class Util {
		public static string ToDmsString(double value) {
			value = Math.Abs(value);
			int d = (int)value;
			value *= 60; value %= 60;
			int m = (int)value;
			value *= 60; value %= 60;
			return string.Format("{0}°{1}′{2:F1}″", d, m, value);
		}
		public static string ToDmsString(double value, string negativeSuffix, string positiveSuffix) {
			string suffix;
			if (value < 0) suffix = negativeSuffix;
			else if (value > 0) suffix = positiveSuffix;
			else suffix = "";
			return string.Format("{0}{1}", ToDmsString(value), suffix);
		}
	}
}