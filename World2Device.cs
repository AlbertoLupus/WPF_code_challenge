using System.Windows.Controls;
using System.Windows.Media;

public class World2Device
{
    private Matrix matrix;

    public World2Device(
        double wxmin, double wxmax, double wymin, double wymax, // world coordinates
        double dxmin, double dxmax, double dymin, double dymax  // device coordinates
    )
    {
        matrix = Matrix.Identity;
        matrix.Translate(-wxmin, -wymin);                   // translate world origin

        // var wmin = Math.Min(wxmin, wymin);
        // var wmax = Math.Max(wxmax, wymax);
        // var dmin = Math.Min(dxmin, dymin);
        // var dmax = Math.Max(dxmax, dymax);
        // var scale = (dmax - dmin) / (wmax - wmin);
        var xscale = (dxmax - dxmin) / (wxmax - wxmin);
        var yscale = (dymax - dymin) / (wymax - wymin);
        matrix.Scale(xscale, yscale);                       // scale x and y-axis     

        matrix.Translate(dxmin, dymin);                     // translate device origin
    }

    public void RenderTransform(Canvas canvas)
    {
        canvas.RenderTransform = new MatrixTransform(matrix);
    }
}