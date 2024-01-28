using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

    private Matrix WToDMatrix, DToWMatrix;
    private void PrepareTransformations(
        double wxmin, double wxmax, double wymin, double wymax,
        double dxmin, double dxmax, double dymin, double dymax
    )
    {
        WToDMatrix = Matrix.Identity;
        WToDMatrix.Translate(-wxmin, -wymin);

        var xscale = (dxmax - dxmin) / (wxmax - wxmin);
        var yscale = (dymax - dymin) / (wymax - wymin);
        var scale = Math.Abs(Math.Min(xscale, yscale));
        // WToDMatrix.Scale(Math.Sign(xscale) * scale, Math.Sign(yscale) * scale);
        WToDMatrix.Scale(xscale, yscale);

        WToDMatrix.Translate(dxmin, dymin);

        // map from device to world coordinates
        DToWMatrix = WToDMatrix;
        DToWMatrix.Invert();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var geometries = new ShapeFactoryJson("..\\..\\..\\content.json");
        var group = new GeometryGroup();
        group.Children = new GeometryCollection(geometries);

        const double dmargin = 0;
        double dxmin = dmargin;
        double dxmax = localCanvas.Width - dmargin;
        double dymin = dmargin;
        double dymax = localCanvas.Height - dmargin;

        double wxmin = group.Bounds.X;
        double wxmax = wxmin + group.Bounds.Width;
        double wymin = group.Bounds.Y;
        double wymax = wymin + group.Bounds.Height;

        var w = new World2Device(wxmin, wxmax, wymin, wymax, dxmin, dxmax, dymax, dymin);
        PrepareTransformations(wxmin, wxmax, wymin, wymax, dxmin, dxmax, dymax, dymin);
        localCanvas.RenderTransform = new MatrixTransform(WToDMatrix);

        DrawAxises(dxmin, dxmax, dymin, dymax, wxmin, wxmax, wymin, wymax);

        var path = new Path
        {
            StrokeThickness = 1,
            Stroke = Brushes.Black,
            Data = group
        };
        localCanvas.Children.Add(path);
    }

    private void DrawAxises(double dxmin, double dxmax, double dymin, double dymax, double wxmin, double wxmax, double wymin, double wymax)
    {
        // x-axis
        var xaxis = new GeometryGroup();
        var p0 = new Point(wxmin, 0);
        var p1 = new Point(wxmax, 0);
        xaxis.Children.Add(new LineGeometry(p0, p1));

        var xscale = (dxmax - dxmin) / (wxmax - wxmin);
        var yscale = (dymax - dymin) / (wymax - wymin);
        var xaxis_path = new Path
        {
            StrokeThickness = 1.0 / yscale,
            Stroke = Brushes.Black,
            Data = xaxis
        };

        localCanvas.Children.Add(xaxis_path);

        // y-axis
        var yaxis = new GeometryGroup();
        var py0 = new Point(0, wymin);
        var py1 = new Point(0, wymax);
        yaxis.Children.Add(new LineGeometry(py0, py1));

        var yaxis_path = new Path
        {
            StrokeThickness = 1.0 / xscale,
            Stroke = Brushes.Black,
            Data = yaxis
        };

        localCanvas.Children.Add(yaxis_path);
    }
}