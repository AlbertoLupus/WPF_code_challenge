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

namespace wscad_coding_challenge;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // alias to Canvas element from *.xaml to prevent multiple error messages in Visual Code:
    // The name 'canGraph' does not exist in the currnet context (CS0103)
    private Canvas localCanvas;

    public MainWindow()
    {
        InitializeComponent();
        localCanvas = canvas;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        const double wxmin = -10;
        const double wxmax = 110;
        const double wymin = -1;
        const double wymax = 11;
        // const double xstep = 10;
        // const double ystep = 1;
        // const double xtic = 5;
        // const double ytic = 0.5;

        const double dmargin = 10;
        double dxmin = dmargin;
        double dxmax = localCanvas.Width - dmargin;
        double dymin = dmargin;
        double dymax = localCanvas.Height - dmargin;

        // prepare transformation matrices
        var w = new World2Device(wxmin, wxmax, wymin, wymax, dxmin, dxmax, dymax, dymin);

        // localCanvas.RenderTransform = new MatrixTransform(WToDMatrix);

        // x-axis
        var xaxis_geom = new GeometryGroup();
        var p0 = new Point(wxmin, 0);
        var p1 = new Point(wxmax, 0);
        xaxis_geom.Children.Add(new LineGeometry(w.T(p0), w.T(p1)));

        var xaxis_path = new Path
        {
            StrokeThickness = 1,
            Stroke = Brushes.Black,
            Data = xaxis_geom
        };

        localCanvas.Children.Add(xaxis_path);

        // y-axis
        var yaxis_geom = new GeometryGroup();
        var py0 = new Point(0, wymin);
        var py1 = new Point(0, wymax);
        yaxis_geom.Children.Add(new LineGeometry(w.T(py0), w.T(py1)));

        var yaxis_path = new Path
        {
            StrokeThickness = 1,
            Stroke = Brushes.Black,
            Data = yaxis_geom
        };

        localCanvas.Children.Add(yaxis_path);
    }
}