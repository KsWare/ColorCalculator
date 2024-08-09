using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KsWare.ColorCalculator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
	public MainWindow() {
		InitializeComponent();

		Test();
	}

	private void Test() {
		ColorEstimator.Test();
	}

	private void CalculateButton_OnClick(object sender, RoutedEventArgs e) {
		var baseColor = ArgbColorConverter.FromString(BaseColorTextBox.Text);
		var mixedColor = ArgbColorConverter.FromString(MixedColorTextBox.Text);

		BaseColorDisplay.Background = new SolidColorBrush(baseColor);
		MixedColorDisplay.Background = new SolidColorBrush(mixedColor);

		BaseRTextBox.Text = $"{baseColor.R}";
		BaseGTextBox.Text = $"{baseColor.G}";
		BaseBTextBox.Text = $"{baseColor.B}";

		MixedRTextBox.Text = $"{mixedColor.R}";
		MixedGTextBox.Text = $"{mixedColor.G}";
		MixedBTextBox.Text = $"{mixedColor.B}";

		var baseHsv = (HsvColor) baseColor;
		var mixedHsv = (HsvColor) mixedColor;

		BaseHTextBox.Text = $"{baseHsv.H}";
		BaseSTextBox.Text = $"{baseHsv.S}";
		BaseVTextBox.Text = $"{baseHsv.V}";

		MixedHTextBox.Text = $"{mixedHsv.H}";
		MixedSTextBox.Text = $"{mixedHsv.S}";
		MixedVTextBox.Text = $"{mixedHsv.V}";

		var fullColorWithAlpha = ColorEstimator.EstimateFullColorWithAlpha(baseColor, mixedColor);

		CalculatedATextBox.Text = $"{fullColorWithAlpha.A}";
		CalculatedRTextBox.Text = $"{fullColorWithAlpha.R}";
		CalculatedGTextBox.Text = $"{fullColorWithAlpha.G}";
		CalculatedBTextBox.Text = $"{fullColorWithAlpha.B}";

		var calculatedHsv = (HsvColor) fullColorWithAlpha;

		CalculatedHTextBox.Text = $"{calculatedHsv.H}";
		CalculatedSTextBox.Text = $"{calculatedHsv.S}";
		CalculatedVTextBox.Text = $"{calculatedHsv.V}";

		CalculatedColorTextBox.Text = $"{fullColorWithAlpha.A:X2}{fullColorWithAlpha.R:X2}{fullColorWithAlpha.G:X2}{fullColorWithAlpha.B:X2}";
		AlphaColorDisplay.Background = new SolidColorBrush(fullColorWithAlpha);
	}

}