# Gu.Wpf.Geometry

Small library based on [this blog post](http://www.charlespetzold.com/blog/2009/02/Graphical-Paths-with-Gradient-Colors.html)

```
<geometry:GradientPath GradientMode="Parallel"
                        StrokeThickness="10"
                        UseLayoutRounding="True">
    <geometry:GradientPath.Data>
        <PathGeometry>
            <PathFigure StartPoint="35,400">
                <LineSegment Point="100,300" />
                <BezierSegment Point1="300,10"
                                Point2="500,600"
                                Point3="700,300" />
                <LineSegment Point="800,100" />
            </PathFigure>
            <PathFigure StartPoint="200,200">
                <LineSegment Point="200,400" />
                <LineSegment Point="400,400" />
            </PathFigure>
        </PathGeometry>
    </geometry:GradientPath.Data>
    <geometry:GradientPath.GradientStops>
        <GradientStop Offset="0" Color="#4C0000FF" />
        <GradientStop Offset="0.5" Color="#4CFF0000" />
        <GradientStop Offset="1" Color="#4CFFFF00" />
    </geometry:GradientPath.GradientStops>
</geometry:GradientPath>
```
![screenie](http://i.imgur.com/YxNoS87.gif)
