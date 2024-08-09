using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KsWare.ColorCalculator;

internal class Utils {

	public static double EuclideanDistance(Color a, Color b) {
		var rDifference = a.ScR - b.ScR;
		var gDifference = a.ScG - b.ScG;
		var bDifference = a.ScB - b.ScB;

		// Weighting of the color channels according to human sensitivity
		var weightedDifference = 0.30 * rDifference * rDifference +
		                         0.59 * gDifference * gDifference +
		                         0.11 * bDifference * bDifference;

		// Calculation of the Euclidean distance of the weighted values
		return Math.Sqrt(weightedDifference);
	}

	public static double EuclideanDistance(HsvColor a, HsvColor b) {
		var hDifference = a.ScH - b.ScH;
		var sDifference = a.ScS - b.ScS;
		var vDifference = a.ScV - b.ScV;

		// Weighting of the color channels in the HSV color space
		var weightedDifference = 0.5 * hDifference * hDifference +
		                         1.0 * sDifference * sDifference +
		                         1.0 * vDifference * vDifference;

		// Calculation of the Euclidean distance of the weighted values
		return Math.Sqrt(weightedDifference);
	}

	public static bool IsGrayScale(Color color) {
		// Überprüft, ob die Farbe ein Grauton ist, d.h., ob R, G und B gleich sind
		return Math.Abs(color.ScR - color.ScG) < 0.0001 && Math.Abs(color.ScG - color.ScB) < 0.0001;
	}

	public static Color CalcBlackWhiteWithAlpha(Color baseColor, Color mixedColor) {
		// works only for grayscale values!
		var fullColor = mixedColor.R > baseColor.R ? Colors.White : Colors.Black;
		var baseValue = baseColor.R / 255.0;
		var mixedValue = mixedColor.R / 255.0;
		var alpha = fullColor == Colors.White 
			? (mixedValue - baseValue) / (1.0 - baseValue) 
			: mixedValue / baseValue;
		return Color.FromArgb((byte)(alpha * 255), fullColor.R, fullColor.R, fullColor.R);
	}
}
