#### 3.0.0
* BREAKING: net48;net6.0-windows

#### 2.2.8
* InvalidateVisual on ContentMatrixProperty change

#### 2.2.7
* tweak annotations

#### 2.2.6
* netcoreapp3.1
* nullable: enable

#### 2.2.3
* GradientPath with IsLargeArc=true

#### 2.2.2
* BUGFIX: GradientPath with ArcSegment

#### 2.2.0
* FEATURE: Balloon.PlacementRectangle

#### 2.1.0
* BUGFIX: Handle ZoomUniform & ZoomUniformToFill when child is not arranged yet.
* FEATURE: Expose more API on ZoomBox, ZoomNone(), ZoomUniform(), ZoomUniformToFill()

#### 2.0.0
* BUGFIX: Handle transparency in AngularGradientEffect
* BUGFIX: Handle transparency in FadeEffect
* BUGFIX: Nicer HLSL interpolation, should render nicer HslWheelEffect and HsvWheelEffect
* FEATURE: StartAngle & CentralAngle for AngularGradientEffect, HslWheelEffect, HsvWheelEffect
* BREAKING: Renamed AngularGradientEffect.CentralAngle, was ArcLength
