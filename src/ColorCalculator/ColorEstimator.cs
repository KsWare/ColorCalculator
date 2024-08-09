using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static KsWare.ColorCalculator.Utils;

namespace KsWare.ColorCalculator;

public class ColorEstimator {

	public static Color EstimateFullColorWithAlpha(Color baseColor, Color mixedColor) {
		if (IsGrayScale(baseColor) && IsGrayScale(mixedColor)) return CalcBlackWhiteWithAlpha(baseColor, mixedColor);

		var bestColor = Colors.Black;
		var minError = double.MaxValue;

		// Convert mixedColor to HSV and calculate the average Hue value
		var mixedHsv = (HsvColor)mixedColor;
		var hueCenter = mixedHsv.ScH;

		// Define the Hue range on the basis of +-5% of the mean value
		var hueMin = hueCenter - 0.05;
		var hueMax = hueCenter + 0.05;

		// Iteration over the restricted Hue range and saturation/brightness (S = V)
		for (var h0 = hueMin; h0 <= hueMax; h0 += 0.005) {
			var h = (h0 + 1.0) % 1.0;
			for (var svStep = 255; svStep <= 255; svStep++) {
				var sv = svStep / 255.0;
				for (var alphaStep = 0; alphaStep <= 255; alphaStep++) {
					var alpha = alphaStep / 255.0;
					var hsvColor = new HsvColor(h, sv, sv);
					var fullColor = (Color)hsvColor;
					var currentColor = MixColors(baseColor, fullColor, alpha);
					var error = CalculateColorError(mixedColor, currentColor);
					if (error < minError) {
						minError = error;
						bestColor = Color.FromScRgb((float)alpha, fullColor.ScR, fullColor.ScG, fullColor.ScB);
						Debug.WriteLine($"{error} {alpha} {h} {sv}");
					}
				}
			}
		}

		return bestColor;
	}

	private static Color MixColors(Color baseColor, Color fullColor, double alpha) {
//		var r = (float)(alpha * fullColor.ScR + (1 - alpha) * baseColor.ScR);
//		var g = (float)(alpha * fullColor.ScG + (1 - alpha) * baseColor.ScG);
//		var b = (float)(alpha * fullColor.ScB + (1 - alpha) * baseColor.ScB);

//		var r = (float)Math.Sqrt(alpha * fullColor.ScR * fullColor.ScR + (1 - alpha) * baseColor.ScR * baseColor.ScR);
//		var g = (float)Math.Sqrt(alpha * fullColor.ScG * fullColor.ScG + (1 - alpha) * baseColor.ScG * baseColor.ScG);
//		var b = (float)Math.Sqrt(alpha * fullColor.ScB * fullColor.ScB + (1 - alpha) * baseColor.ScB * baseColor.ScB);

		// Rückführung auf sRGB
		float ConvertToSrgbChannel(float scChannel) {
			return (scChannel <= 0.0031308f) 
				? scChannel * 12.92f 
				: 1.055f * (float)Math.Pow(scChannel, 1.0 / 2.4) - 0.055f;
		}

		// Mischung in sRGB durchführen
		var rSrgb = ConvertToSrgbChannel(fullColor.ScR) * alpha + ConvertToSrgbChannel(baseColor.ScR) * (1 - alpha);
		var gSrgb = ConvertToSrgbChannel(fullColor.ScG) * alpha + ConvertToSrgbChannel(baseColor.ScG) * (1 - alpha);
		var bSrgb = ConvertToSrgbChannel(fullColor.ScB) * alpha + ConvertToSrgbChannel(baseColor.ScB) * (1 - alpha);

		// Konvertiere zurück in ScRGB
		float ConvertToScRgbChannel(float srgbChannel) {
			return (srgbChannel <= 0.04045f) 
				? srgbChannel / 12.92f 
				: (float)Math.Pow((srgbChannel + 0.055f) / 1.055f, 2.4f);
		}

		var r = ConvertToScRgbChannel((float)rSrgb);
		var g = ConvertToScRgbChannel((float)gSrgb);
		var b = ConvertToScRgbChannel((float)bSrgb);

		return Color.FromScRgb(1.0f, r, g, b);
	}

	private static double CalculateColorError(Color a, Color b) 
		=> Utils.EuclideanDistance(a, b)* 0.7 + Utils.EuclideanDistance((HsvColor)a, (HsvColor)b)* 0.3;

	public static void Test() {
		
	}

	public static void TestSpecificColorError() {
		// Gegebene Werte aus deinem Fall
		double h = 0.3333333432674408; // Hue für Grün (120°)
		double s = 1.0;                // Maximale Sättigung
		double v = 1.0;                // Maximale Helligkeit
		double alpha = 0.5;            // Alpha-Wert

		// Erwartete gemischte Farbe: R=0, G=0.5, B=0
		var expectedColor = Color.FromScRgb(1.0f, 0.0f, 0.5f, 0.0f);

		// Konvertiere von HSV nach RGB
		var hsvColor = new HsvColor(h, s, v);
		var fullColor = hsvColor.ToRgbColor();

		// Debugging: Untersuche die RGB-Werte nach der Konvertierung
		Debug.WriteLine($"Converted RGB Values: R={fullColor.ScR}, G={fullColor.ScG}, B={fullColor.ScB}");

		// Mischung der Farben mit Alpha = 0.5
		var baseColor = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f); // Schwarz
		var mixedColor = MixColors(baseColor, fullColor, alpha);

		// Debugging: Untersuche die resultierenden RGB-Werte nach der Mischung
		Debug.WriteLine($"Mixed RGB Values: R={mixedColor.ScR}, G={mixedColor.ScG}, B={mixedColor.ScB}");

		// Berechne den Fehler zwischen der erwarteten und der gemischten Farbe
		var error = CalculateColorError(expectedColor, mixedColor);

		// Gebe den berechneten Fehler aus
		Debug.WriteLine($"Calculated Error: {error}");
	}

	public static void InvestigateSmallBlueValues() {
		// Festgelegte HSV-Werte, die das Problem verursachen
		var h = 0.3333333432674408; // Hue für Grün (120°)
		var s = 1.0;                // Maximale Sättigung
		var v = 1.0;                // Maximale Helligkeit

		// Konvertiere von HSV nach RGB
		var hsvColor = new HsvColor(h, s, v);
		var rgbColor = hsvColor.ToRgbColor();

		// Debugging: Untersuche die RGB-Werte nach der Konvertierung
		Debug.WriteLine($"Converted RGB Values: R={rgbColor.ScR}, G={rgbColor.ScG}, B={rgbColor.ScB}");

		// Mischung der Farben mit Alpha = 0.5
		var baseColor = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f); // Schwarz
		double alpha = 0.5;
		var mixedColor = MixColors(baseColor, rgbColor, alpha);

		// Debugging: Untersuche die resultierenden RGB-Werte nach der Mischung
		Debug.WriteLine($"Mixed RGB Values: R={mixedColor.ScR}, G={mixedColor.ScG}, B={mixedColor.ScB}");

		// Überprüfe, ob kleine Blauwerte vorhanden sind und welchen Einfluss sie haben
		var expectedColor = Color.FromScRgb(1.0f, 0.0f, 0.5f, 0.0f); // Erwartetes Ergebnis

		// Berechne den Fehler und gebe ihn aus
		var error = CalculateColorError(expectedColor, mixedColor);
		Debug.WriteLine($"Calculated Error: {error}");

		// Vergleiche die Farbwerte direkt
		Debug.WriteLine($"Direct Comparison: R={Math.Abs(expectedColor.ScR - mixedColor.ScR)}, " +
		                $"G={Math.Abs(expectedColor.ScG - mixedColor.ScG)}, " +
		                $"B={Math.Abs(expectedColor.ScB - mixedColor.ScB)}");
	}

	public static void TestHsvToRgb() {
		var hsvColor = new HsvColor(120 / 360.0, 1.0, 1.0); // HSV(120, 1.0, 1.0)
		var rgbColor = (Color)hsvColor;  // Umwandlung von HSV zu RGB

		Console.WriteLine($"RGB-Wert aus HSV(120, 1.0, 1.0): {rgbColor}");

		// Erwarteter Wert: RGB(0.0, 1.0, 0.0) oder #00FF00
		var expectedColor = Color.FromScRgb(1.0f, 0.0f, 1.0f, 0.0f);

		bool isEqual = Math.Abs(rgbColor.ScR - expectedColor.ScR) < 1e-6 &&
		               Math.Abs(rgbColor.ScG - expectedColor.ScG) < 1e-6 &&
		               Math.Abs(rgbColor.ScB - expectedColor.ScB) < 1e-6;

		Console.WriteLine($"Farben sind gleich: {isEqual}");
	}

	public static void CalculateColorErrorTest() {
		var expected  = Color.FromScRgb(1, 0, 0.5f, 0);
		var baseColor = Color.FromScRgb(1, 0, 0, 0);
		var fullColor = Color.FromScRgb(1, 0, 1f, 0);
		var mixed50 = CalculateColorError(expected, MixColors(baseColor, fullColor, 0.5));
		var mixed37 = CalculateColorError(expected, MixColors(baseColor, fullColor, 0.37));
	}

	public static void MixColorsTest() {
		var alpha = 0.5; // entspricht 0x80
		var fullColor = Color.FromRgb(0, 255, 0); // #00FF00
		var mixed = MixColors(Color.FromRgb(0,00, 0), fullColor, alpha);
		
		Debug.WriteLine(mixed); // Sollte (0, 128, 0) ausgeben
		var isEqual = mixed.ScR == 0 &&
		              mixed.ScG == 0.5 &&
		              mixed.ScB == 0;
        
		Debug.WriteLine($"Farben sind gleich: {isEqual}");
	}

}


