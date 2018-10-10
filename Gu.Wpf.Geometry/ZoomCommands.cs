namespace Gu.Wpf.Geometry
{
    using System.Windows.Input;

    /// <summary>
    /// A collection of commands for <see cref="Zoombox"/>.
    /// </summary>
    public static class ZoomCommands
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        public static RoutedUICommand None { get; } = new RoutedUICommand(nameof(None), nameof(None), typeof(ZoomCommands));

        /// <summary>
        /// The content is re-sized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        public static RoutedUICommand Uniform { get; } = new RoutedUICommand(nameof(Uniform), nameof(Uniform), typeof(ZoomCommands));

        /// <summary>
        /// The content is re-sized to fill the destination dimensions while it preserves its native aspect ratio. If the aspect ratio of the destination rectangle differs from the source, the source content is clipped to fit in the destination dimensions.
        /// </summary>
        public static RoutedUICommand UniformToFill { get; } = new RoutedUICommand(nameof(UniformToFill), nameof(UniformToFill), typeof(ZoomCommands));

        /// <summary>
        /// Increase the zoom.
        /// </summary>
        public static RoutedUICommand Increase { get; } = new RoutedUICommand(nameof(Increase), nameof(Increase), typeof(ZoomCommands));

        /// <summary>
        /// Decrease the zoom.
        /// </summary>
        public static RoutedUICommand Decrease { get; } = new RoutedUICommand(nameof(Decrease), nameof(Decrease), typeof(ZoomCommands));
    }
}
