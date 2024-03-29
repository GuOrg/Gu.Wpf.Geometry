# Gu.Wpf.Geometry
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/GuOrg/Gu.Wpf.Geometry?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.Geometry.svg)](https://www.nuget.org/packages/Gu.Wpf.Geometry/)
[![Build status](https://ci.appveyor.com/api/projects/status/1nk9elw89ker9fjs?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-wpf-geometry)
[![Build Status](https://dev.azure.com/guorg/Gu.Wpf.Geometry/_apis/build/status/GuOrg.Gu.Wpf.Geometry?branchName=master)](https://dev.azure.com/guorg/Gu.Wpf.Geometry/_build/latest?definitionId=6&branchName=master)
[![ci](https://github.com/GuOrg/Gu.Wpf.Geometry/actions/workflows/ci.yml/badge.svg)](https://github.com/GuOrg/Gu.Wpf.Geometry/actions/workflows/ci.yml)

Small library with WPF geometries and shaders.

# Contents
- [Geometries](#geometries)
  - [Balloon](#balloon)
  - [BoxBalloon](#boxballoon)
        - [Simple](#simple)
        - [ConnectorAngle](#connectorangle)
        - [ConnectorOffset](#connectoroffset)
        - [With PlacementTarget](#with-placementtarget)
        - [PlacementTarget](#placementtarget)
        - [PlacementOptions](#placementoptions)
  - [EllipseBalloon](#ellipseballoon)
  - [GradientPath](#gradientpath)
- [Controls](#controls)
  - [Zoombox](#zoombox)
- [Effects](#effects)
  - [DesaturateEffect](#desaturateeffect)
  - [FadeEffect](#fadeeffect)
  - [MaskEffect](#maskeffect)
  - [AngularGradientEffect](#angulargradienteffect)
  - [HsvWheelEffect](#hsvwheeleffect)
  - [HslWheelEffect](#hslwheeleffect)

# Geometries

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

```xaml
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
```xaml
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

![connected balloon](http://i.imgur.com/wJBJACc.gif)

##### With PlacementRectangle
```xaml
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
                             PlacementRectangle="100, 50, 100, 10"
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

![connected balloon](http://i.imgur.com/wJBJACc.gif)

##### With PlacementRectangle and PlacementTarget
```xaml
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
                             PlacementRectangle="0, 0, 100, 10"
							 PlacementTarget={Binding ElementName=Target}
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

![connected balloon](http://i.imgur.com/wJBJACc.gif)

##### PlacementTarget
The UIElement to connect the balloon to.

##### PlacementRectangle
The rectangle to connect the balloon to. If used with PlacementTarget, the rectangle coordinates are relative to it. If used alone, the coordinates are relative to the parent UIElement.

##### PlacementOptions
Where on the connected element the connector should be attached.

In xaml either input a string `<geometry:BoxBalloon PlacementOptions="Top Left" .../>`
Or use the `PlacementOptionsExtension`:
```xaml
<geometry:BoxBalloon PlacementOptions="{geometry:PlacementOptions Horizontal=Auto,
                                                                  Vertical=Top,
                                                                  Offset=5}"
                     ...
                      />
```

## EllipseBalloon

Same API as BoxBalloon but renders:

![EllipseBallon](http://i.imgur.com/2yMYmfR.png)

## GradientPath

Based on [this blog post](http://www.charlespetzold.com/blog/2009/02/Graphical-Paths-with-Gradient-Colors.html)

```xaml
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

# Controls

## Zoombox

Panel for zooming & panning. Handles touch but IsManipulationEnabled has default value `false` meaning no touch.

```xaml
<UserControl ...
             xmlns:effects="http://gu.se/Geometry">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <effects:Zoombox x:Name="ImageBox"
                         IsManipulationEnabled="True"
                         MaxZoom="10"
                         MinZoom="0.1">
            <Image Source="Images/Circles.png" />
        </effects:Zoombox>
        <UniformGrid Grid.Row="1" Rows="1">
            <Button Command="effects:ZoomCommands.Increase"
                    CommandParameter="2.0"
                    CommandTarget="{Binding ElementName=ImageBox}"
                    Content="Increase" />

            <Button Command="effects:ZoomCommands.Decrease"
                    CommandParameter="2.0"
                    CommandTarget="{Binding ElementName=ImageBox}"
                    Content="Decrease" />

            <Button Command="effects:ZoomCommands.None"
                    CommandTarget="{Binding ElementName=ImageBox}"
                    Content="None" />

            <Button Command="effects:ZoomCommands.Uniform"
                    CommandTarget="{Binding ElementName=ImageBox}"
                    Content="Uniform" />

            <Button Command="effects:ZoomCommands.UniformToFill"
                    CommandTarget="{Binding ElementName=ImageBox}"
                    Content="UniformToFill" />
        </UniformGrid>
    </Grid>
</UserControl>
```

# Effects
A collection of pixel shaders.

## DesaturateEffect
Desaturate an image, brush or WPF controls. It has only one property `Strength` 0 means the effect does nothing and 1 means a grayscale image is returned.

```xaml
<UserControl
    ...
    xmlns:effects="http://gu.se/Geometry">
    <Grid>
        <Image Source="Images/Hustler.jpg">
            <Image.Effect>
                <effects:DesaturateEffect Strength="{Binding ElementName=Slider, Path=Value}" />
            </Image.Effect>
        </Image>
        <Slider
            x:Name="Slider"
            VerticalAlignment="Bottom"
            Maximum="1"
            Minimum="0" />
    </Grid>
</UserControl>
```

## FadeEffect
Fade an image to a colour.

```xaml
<UserControl ...
             xmlns:effects="http://gu.se/Geometry">
    <Grid>
        <Image Source="../Images/Hustler.jpg">
            <Image.Effect>
                <effects:FadeEffect To="Black" Strength="{Binding ElementName=StrengthSlider, Path=Value}" />
            </Image.Effect>
        </Image>
        <Slider
            x:Name="StrengthSlider"
            VerticalAlignment="Bottom"
            Maximum="1"
            Minimum="0" />
    </Grid>
</UserControl>
```

## MaskEffect
Render an image as a selection mask.

```xaml
<UserControl ...
             xmlns:effects="http://gu.se/Geometry">
    <Grid>
        <Image Source="../Images/Circle.png">
            <Image.Effect>
                <effects:MaskEffect  />
            </Image.Effect>
        </Image>
    </Grid>
</UserControl>
```

## AngularGradientEffect
A gradient that changes value along the angle. Perhaps useful for spinners.
Replaces the pixels of the owning element. The owning element must have a `Brush` set the brush can be any color that is not transparent.

### StartAngle
The angle for the `StartColor` -360 - 360 degrees, positive clockwise. Zero at 12 o'clock.

### CentralAngle
The anglular extent of the gradient -360 - 360 degrees, positive clockwise.

### CenterPoint
The center point of the gradient (0, 0) - (1, 1) default is (0.5, 0.5) fore a gradient around the center.

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:effects="http://gu.se/Geometry"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        Title="AngularGradientWindow"
        SizeToContent="WidthAndHeight"
        mc:Ignorable="d">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <GroupBox x:Name="Render"
                  Grid.Row="0"
                  Grid.Column="1"
                  Margin="50"
                  Background="LightGray"
                  BorderThickness="0"
                  Style="{StaticResource InvisibleGroupBoxStyle}">
            <Ellipse x:Name="Ellipse"
                     Width="200"
                     Height="200"
                     Stroke="White"
                     StrokeThickness="25">
                <Ellipse.Effect>
                    <effects:AngularGradientEffect CenterPoint="{Binding ElementName=CenterPoint,
                                                                         Path=Text}"
                                                   CentralAngle="{Binding ElementName=CentralAngle,
                                                                          Path=Value}"
                                                   EndColor="{Binding ElementName=EndColour,
                                                                      Path=Text}"
                                                   StartAngle="{Binding ElementName=StartAngle,
                                                                        Path=Value}"
                                                   StartColor="{Binding ElementName=StartColour,
                                                                        Path=Text}" />
                </Ellipse.Effect>
            </Ellipse>
        </GroupBox>

        <TextBlock Grid.Row="1"
                   Grid.Column="0"
                   Text="Start colour:" />
        <TextBox x:Name="StartColour"
                 Grid.Row="1"
                 Grid.Column="1"
                 Text="#FFFF0000" />

        <TextBlock Grid.Row="2"
                   Grid.Column="0"
                   Text="End colour:" />
        <TextBox x:Name="EndColour"
                 Grid.Row="2"
                 Grid.Column="1"
                 Text="#00FF0000" />

        <TextBlock Grid.Row="3"
                   Grid.Column="0"
                   Text="Center point:" />
        <TextBox x:Name="CenterPoint"
                 Grid.Row="3"
                 Grid.Column="1"
                 Text="0.5 0.5" />

        <TextBlock Grid.Row="4"
                   Grid.Column="0"
                   Text="Start angle:" />
        <Slider x:Name="StartAngle"
                Grid.Row="4"
                Grid.Column="1"
                Maximum="360"
                Minimum="-360" />

        <TextBlock Grid.Row="5"
                   Grid.Column="0"
                   Text="Central angle:" />
        <Slider x:Name="CentralAngle"
                Grid.Row="5"
                Grid.Column="1"
                Maximum="360"
                Minimum="-360"
                Value="360" />
    </Grid>
</Window>

```

![animation](https://user-images.githubusercontent.com/1640096/33270557-c407bbdc-d384-11e7-9e81-03d5ab9f7b08.gif)

## HsvWheelEffect
A gradient that changes value along the angle. Perhaps useful for colour pickers.
Note that the element must have a background or fill. The brush can be any colour.

```xaml
<UserControl ...
             xmlns:effects="http://gu.se/Geometry">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <Ellipse Grid.ColumnSpan="2"
                 Width="300"
                 Height="300"
                 HorizontalAlignment="Center"
                 Fill="White">
            <Ellipse.Effect>
                <effects:HsvWheelEffect InnerRadius="{Binding ElementName=InnerRadius, Path=Value}"
                                        InnerSaturation="{Binding ElementName=InnerSaturation, Path=Value}" />
            </Ellipse.Effect>
        </Ellipse>

        <Label Grid.Row="1"
               Grid.Column="0"
               Content="Inner radius:" />
        <Slider x:Name="InnerRadius"
                Grid.Row="1"
                Grid.Column="1"
                Maximum="1"
                Minimum="0"
                Value="0" />

        <Label Grid.Row="2"
               Grid.Column="0"
               Content="Inner saturation:" />
        <Slider x:Name="InnerSaturation"
                Grid.Row="2"
                Grid.Column="1"
                Maximum="1"
                Minimum="0"
                Value="0" />
    </Grid>
</UserControl>
```

## HslWheelEffect
A gradient that changes value along the angle. Perhaps useful for colour pickers.
Note that the element must have a background or fill. The brush can be any colour.
Same API as HsvWheelEffect
