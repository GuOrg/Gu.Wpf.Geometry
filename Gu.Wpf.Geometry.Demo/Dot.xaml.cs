namespace Gu.Wpf.Geometry.Demo;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public partial class Dot : UserControl
{
    /// <summary>Identifies the <see cref="Center"/> dependency property.</summary>
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
        nameof(Center),
        typeof(Point),
        typeof(Dot),
        new PropertyMetadata(
            default(Point),
            (obj, args) => ((Dot)obj).EllipseGeometry.SetCurrentValue(System.Windows.Media.EllipseGeometry.CenterProperty, (Point)args.NewValue)));

    private bool isDragging;
    private Point mouseDragStart;
    private Point dragStartPos;

    public Dot()
    {
        this.InitializeComponent();
    }

    public Point Center
    {
        get => (Point)this.GetValue(CenterProperty);
        set => this.SetValue(CenterProperty, value);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        this.isDragging = true;
        this.mouseDragStart = e.GetPosition(this);
        this.dragStartPos = this.Center;
        _ = this.CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!this.isDragging)
        {
            return;
        }

        var pos = e.GetPosition(this);
        var offset = pos - this.mouseDragStart;
        var center = this.dragStartPos + offset;
        this.SetCurrentValue(CenterProperty, center);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (!this.isDragging)
        {
            return;
        }

        this.isDragging = false;
        this.ReleaseMouseCapture();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        this.isDragging = false;
        base.OnLostMouseCapture(e);
    }
}
