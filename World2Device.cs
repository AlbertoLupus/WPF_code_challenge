using System.Windows;
using System.Windows.Media;

public class World2Device
{
    public Matrix matrix { get; private set; }

    public World2Device(
        double wxmin, double wxmax, double wymin, double wymax, // world coordinates
        double dxmin, double dxmax, double dymin, double dymax  // device coordinates
    )
    {
        matrix = Matrix.Identity;
        matrix.Translate(-wxmin, -wymin);                   // translate origin

        var xscale = (dxmax - dxmin) / (wxmax - wxmin);
        var yscale = (dymax - dymin) / (wymax - wymin);
        matrix.Scale(xscale, yscale);                       // scale x and y-axis     

        matrix.Translate(dxmin, dymin);
    }

    public Point T(Point p) => matrix.Transform(p);
}