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

    private void Canvas_SizeChanged(object sender, RoutedEventArgs e)
    {
        if (paths == null)
        {
            return;
        }

        AdaptToScreen();
    }

    private double strokeThickness = 1.0;

    private void AdaptToScreen()
    {
        var group = new GeometryGroup();
        group.Children = new GeometryCollection(paths.Select(x => x.Data));

        const double dmargin = 0;
        double dxmin = dmargin;
        double dxmax = localCanvas.Width - dmargin;
        double dymin = dmargin;
        double dymax = localCanvas.Height - dmargin;

        double halfStrokeThickness = strokeThickness / 2;
        double wxmin = group.Bounds.X - halfStrokeThickness;
        double wxmax = wxmin + group.Bounds.Width + strokeThickness;
        double wymin = group.Bounds.Y - halfStrokeThickness;
        double wymax = wymin + group.Bounds.Height + 2 * strokeThickness;

        var w = new World2Device(wxmin, wxmax, wymin, wymax, dxmin, dxmax, dymax, dymin);
        w.RenderTransform(localCanvas);
    }

    private List<Path>? paths;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        paths = new PathFactoryJson("..\\..\\..\\content.json", strokeThickness).ToList();
        AdaptToScreen();

        // DrawAxises(dxmin, dxmax, dymin, dymax, wxmin, wxmax, wymin, wymax);

        foreach (var path in paths)
        {
            localCanvas.Children.Add(path);
        }
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