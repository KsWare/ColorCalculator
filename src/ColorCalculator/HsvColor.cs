using System.Windows.Media;

namespace KsWare.ColorCalculator;

public readonly struct HsvColor {

	public HsvColor(double scH, double scS, double scV) {
		ScH = scH;
		ScS = scS;
		ScV = scV;
	}

	public HsvColor(int h, int s, int v) {
		ScH = h / 100d;
		ScH = s / 100d;
		ScS = v / 360d;
	}

	public double ScH { get; }
	public double ScS { get; }
	public double ScV { get; }

	public int H => (int)(ScH * 360d);
	public int S => (int)(ScS * 100d);
	public int V => (int)(ScV * 100d);

	public override string ToString() {
		return $"H: {ScH}, S: {ScS}, V: {ScV}";
	}
	
	// Conversion from HSV to RGB
	public Color ToRgbColor() {
		double r, g, b;

		var i = (int)(ScH * 6);
		var f = ScH * 6 - i;
		var p = ScV * (1 - ScS);
		var q = ScV * (1 - f * ScS);
		var t = ScV * (1 - (1 - f) * ScS);

		switch (i % 6) {
			case 0: r = ScV; g = t; b = p; break;
			case 1: r = q; g = ScV; b = p; break;
			case 2: r = p; g = ScV; b = t; break;
			case 3: r = p; g = q; b = ScV; break;
			case 4: r = t; g = p; b = ScV; break;
			case 5: r = ScV; g = p; b = q; break;
			default: r = g = b = 0; /* Should never be achieved */ break;
		}
		return Color.FromScRgb(1.0f, (float)r, (float)g, (float)b);
	}

	// Conversion from RGB to HSV
	public static HsvColor FromColor(Color color) {
		var r = color.ScR;
		var g = color.ScG;
		var b = color.ScB;

		var max = (float)Math.Max(r, Math.Max(g, b));
		var min = (float)Math.Min(r, Math.Min(g, b));
		var delta = max - min;

		var h = 0.0f;
		if (delta != 0) {
			if (max == r) h = (g - b) / delta + (g < b ? 6 : 0);
			else if (max == g) h = (b - r) / delta + 2;
			else if (max == b) h = (r - g) / delta + 4;
			h /= 6.0f;
		}

		var s = max == 0 ? 0 : delta / max;
		var v = max;

		return new HsvColor(h, s, v);
	}

	public static explicit operator HsvColor(Color color) {
		return HsvColor.FromColor(color);
	}

	public static explicit operator Color(HsvColor hsvColor) {
		return hsvColor.ToRgbColor();
	}
}