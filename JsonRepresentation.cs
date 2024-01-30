using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public class PathFactoryJson : IEnumerable<System.Windows.Shapes.Path>
{
    readonly string path;
    double strokeThickness;

    public PathFactoryJson(string path, double strokeThickness)
    {
        this.path = path;
        this.strokeThickness = strokeThickness;
    }

    public IEnumerator<System.Windows.Shapes.Path> GetEnumerator()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ShapeConverter());

        var json = System.IO.File.ReadAllText(path);
        var shapes = JsonSerializer.Deserialize<List<ShapeRepresentationJson>>(json, options);
        foreach (var shape in shapes ?? [])
        {
            yield return ((IShapeRepresentation)shape).Path(strokeThickness);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public interface IShapeRepresentation
{
    System.Windows.Shapes.Path Path(double strokeThickness);
}

// Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS8618

public class ShapeRepresentationJson
{
    public string Type { get; set; }
    public string Color { get; set; }
    protected Point PointFrom(string representation) => new(
        Double.Parse(representation.Split(";")[0]), Double.Parse(representation.Split(";")[1]));
    protected Color ColorFrom(string representation) => new()
    {
        A = Byte.Parse(representation.Split(";")[0]),
        R = Byte.Parse(representation.Split(";")[1]),
        G = Byte.Parse(representation.Split(";")[2]),
        B = Byte.Parse(representation.Split(";")[3])
    };
}

public class SolidShapeRepresentationJson : ShapeRepresentationJson
{
    public bool Filled { get; set; }
    public Brush Stroke => new SolidColorBrush(ColorFrom(Color));
    public Brush? Fill => Filled ? new SolidColorBrush(ColorFrom(Color)) : null;
}

public class LineRepresentationJson : ShapeRepresentationJson, IShapeRepresentation
{
    public string A { get; set; }
    public string B { get; set; }

    private Geometry Geometry() => new LineGeometry(PointFrom(A), PointFrom(B));

    public Path Path(double strokeThickness) => new()
    {
        StrokeThickness = strokeThickness,
        Stroke = new SolidColorBrush(ColorFrom(Color)),
        Data = Geometry()
    };
}

public class CircleRepresentationJson : SolidShapeRepresentationJson, IShapeRepresentation
{
    public string Center { get; set; }
    public double Radius { get; set; }

    private Geometry Geometry() => new EllipseGeometry(PointFrom(Center), Radius, Radius);

    public Path Path(double strokeThickness) => new()
    {
        StrokeThickness = strokeThickness,
        Stroke = Stroke,
        Fill = Fill,
        Data = Geometry()
    };
}

public class TriangleRepresentationJson : SolidShapeRepresentationJson, IShapeRepresentation
{
    public string A { get; set; }
    public string B { get; set; }
    public string C { get; set; }

    //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-create-a-shape-using-a-streamgeometry?view=netframeworkdesktop-4.8
    private Geometry Geometry()
    {
        var result = new StreamGeometry();
        using (StreamGeometryContext ctx = result.Open())
        {
            ctx.BeginFigure(PointFrom(A), isFilled: Filled, isClosed: true);
            ctx.LineTo(PointFrom(B), isStroked: true, isSmoothJoin: false);
            ctx.LineTo(PointFrom(C), isStroked: true, isSmoothJoin: false);
        }
        result.Freeze();

        return result;
    }

    public Path Path(double strokeThickness) => new()
    {
        StrokeThickness = strokeThickness,
        Stroke = Stroke,
        Fill = Fill,
        Data = Geometry()
    };
}

// Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning restore CS8618

// https://stackoverflow.com/questions/62121242/deserialize-json-array-which-has-mixed-values-system-text-json
public class ShapeConverter : JsonConverter<ShapeRepresentationJson>
{
    // using this will result in StackOverFlowException!
    private JsonSerializerOptions options = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };

    public override ShapeRepresentationJson? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var type = doc.RootElement.GetProperty(@"type").GetString();
            var text = doc.RootElement.GetRawText();
            switch (type)
            {
                case "line":
                    return JsonSerializer.Deserialize<LineRepresentationJson>(text, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                case "circle":
                    return JsonSerializer.Deserialize<CircleRepresentationJson>(text, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                case "triangle":
                    return JsonSerializer.Deserialize<TriangleRepresentationJson>(text, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                default:
                    throw new DomainException($"unknown shape: {type}");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, ShapeRepresentationJson value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
