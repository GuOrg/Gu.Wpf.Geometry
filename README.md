# Gu.Wpf.Geometry
Small library with WPF geometries.

## Balloon
A `ContentControl`for showing balloons.

Note: WPF Popup clips to bounds so the connector will be clipped when used in a popup.

```
<geometry:Balloon Grid.Row="1"
                  HorizontalAlignment="Center"
                  VerticalAlignment="Center"
                  ConnectorAngle="25"
                  CornerRadius="15"
                  PlacementOptions="Bottom, Center"
                  PlacementTarget="{Binding ElementName=Target}">
    <Grid Margin="5">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <TextBlock Text="{Binding Text, ElementName=TextBox}" />
        <UniformGrid Grid.Row="1"
                     Rows="1">
            <Ellipse Width="20"
                     Height="20"
                     Fill="Blue" />
            <Ellipse Width="20"
                     Height="20"
                     Fill="Yellow" />
            <Ellipse Width="20"
                     Height="20"
                     Fill="Red" />
        </UniformGrid>
    </Grid>
</geometry:Balloon>
```

![balloon](http://i.imgur.com/5BLZzJU.png)

## BoxBalloon
Draws a border with a connector.
Note: the connector is drawn outside the bounds of the element.


##### Simple
```
<geometry:BoxBalloon Grid.Row="1"
                     Grid.Column="1"
                     ConnectorAngle="45"
                     ConnectorOffset="-10,-0"
                     CornerRadius="10"
                     Stroke="CornflowerBlue"
                     StrokeThickness="4" />
```
![simple balloon](http://i.imgur.com/YZDjCvj.png)

##### ConnectorAngle
![connector angle](http://i.imgur.com/lwViiPI.png)

##### ConnectorOffset

![connector offset](http://i.imgur.com/hT1fFsj.png)

##### With PlacementTarget
```
<Canvas>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="100" />
            <RowDefinition Height="100" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="100" />
            <ColumnDefinition Width="100" />
            <ColumnDefinition Width="100" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <ListBox x:Name="Placements"
                 Grid.Row="0"
                 Grid.RowSpan="3"
                 Grid.Column="2"
                 SelectedIndex="0">
            <geometry:PlacementOptions>Auto</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto 5</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto -5</geometry:PlacementOptions>
            <geometry:PlacementOptions>Center</geometry:PlacementOptions>
            <geometry:PlacementOptions>Center 5</geometry:PlacementOptions>
            <geometry:PlacementOptions>Center -5</geometry:PlacementOptions>
            <geometry:PlacementOptions>Top Left</geometry:PlacementOptions>
            <geometry:PlacementOptions>Top Center</geometry:PlacementOptions>
            <geometry:PlacementOptions>Top Right</geometry:PlacementOptions>
            <geometry:PlacementOptions>Bottom Left</geometry:PlacementOptions>
            <geometry:PlacementOptions>Bottom Center</geometry:PlacementOptions>
            <geometry:PlacementOptions>Bottom Right</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto Center</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto Left</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto Right</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto Top</geometry:PlacementOptions>
            <geometry:PlacementOptions>Auto Bottom</geometry:PlacementOptions>
        </ListBox>

        <geometry:BoxBalloon Grid.Row="1"
                             Grid.Column="1"
                             Margin="6"
                             ConnectorAngle="45"
                             CornerRadius="10"
                             PlacementOptions="{Binding SelectedItem,
                                                        ElementName=Placements}"
                             PlacementTarget="{Binding ElementName=Target}"
                             Stroke="HotPink"
                             StrokeThickness="4" />
    </Grid>
    <Rectangle x:Name="Target"
               Canvas.Left="100"
               Canvas.Top="50"
               Width="100"
               Height="10"
               Fill="Gainsboro"
               MouseLeftButtonDown="OnTargetMouseLeftDown" />
</Canvas>
```

[connected balloon](http://i.imgur.com/wJBJACc.webm)

##### PlacementTarget
Thge UIElement to connect the balloon to.

##### PlacementOptions
Where on the connected element the connector should be attached.

In xaml either input a string `<geometry:BoxBalloon PlacementOptions="Top Left" .../>`
Or use the `PlacementOptionsExtension`:
```
<geometry:BoxBalloon PlacementOptions="{geometry:PlacementOptions Horizontal=Auto,
                                                                  Vertical=Top,
                                                                  Offset=5}"
                     ...
                      />
```



## GradientPath

Based on [this blog post](http://www.charlespetzold.com/blog/2009/02/Graphical-Paths-with-Gradient-Colors.html)

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
