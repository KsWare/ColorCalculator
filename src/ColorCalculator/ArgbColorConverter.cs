using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KsWare.ColorCalculator;

public class ArgbColorConverter {

	public static Color FromString(string colorString) {
		colorString = colorString.Trim().ToUpper();

		if (IsHexString(colorString)) {
			// Hex-Format (RGB oder ARGB)
			return FromHexString(colorString);
		} else if (colorString.Contains(",")) {
			// Komma-getrenntes Format (könnte int oder float sein)
			return FromCommaSeparatedString(colorString);
		} else {
			throw new ArgumentException("Unrecognized color format.");
		}
	}

	private static bool IsHexString(string colorString) {
		// Prüfen, ob es sich um einen Hex-String handelt (6 oder 8 Zeichen, optional mit #)
		return colorString.Length == 6 || colorString.Length == 8 ||
			(colorString.Length == 7 && colorString.StartsWith("#")) ||
			(colorString.Length == 9 && colorString.StartsWith("#"));
	}

	private static Color FromHexString(string hex) {
		hex = hex.TrimStart('#');
		byte a = 255; // Standard-Alpha-Wert

		if (hex.Length == 6) {
			// RGB Format: RRGGBB
			var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return Color.FromArgb(a, r, g, b);
		} else if (hex.Length == 8) {
			// ARGB Format: AARRGGBB
			a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
			return Color.FromArgb(a, r, g, b);
		} else {
			throw new ArgumentException("Invalid hex color string format. Use RGB (RRGGBB) or ARGB (AARRGGBB).");
		}
	}

	private static Color FromCommaSeparatedString(string colorString) {
		var parts = colorString.Split(',');
		if (parts.Length != 3 && parts.Length != 4) 
			throw new ArgumentException("Invalid comma-separated color string format.");

		// Check whether floating point values (0.0 - 1.0) or integers (0-255) are involved
		var isFloat = true;
		foreach (var part in parts) {
			if (!float.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) 
				throw new ArgumentException("Invalid hex color string format.");
			if (value <= 1.0f) continue;
			isFloat = false;
			break;
		}

		if (isFloat) {
			var r = float.Parse(parts[^3], CultureInfo.InvariantCulture);
			var g = float.Parse(parts[^2], CultureInfo.InvariantCulture);
			var b = float.Parse(parts[^1], CultureInfo.InvariantCulture);
			var a = parts.Length == 4 ? float.Parse(parts[0], CultureInfo.InvariantCulture) : 1.0f;

			return Color.FromScRgb(a, r, g, b);
		} else {
			var r = byte.Parse(parts[^3]);
			var g = byte.Parse(parts[^2]);
			var b = byte.Parse(parts[^1]);
			var a = parts.Length == 4 ? byte.Parse(parts[0]) : (byte)255;

			return Color.FromArgb(a, r, g, b);
		}
	}
}
