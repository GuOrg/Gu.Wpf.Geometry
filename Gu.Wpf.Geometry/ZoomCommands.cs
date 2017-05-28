namespace Gu.Wpf.Geometry
{
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public static class ZoomCommands
    {
        /////// <summary>
        /////// The content is resized to fill the destination dimensions. The aspect ratio is not preserved.
        /////// </summary>
        ////public static RoutedUICommand Fill { get; } = Create();

        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        public static RoutedUICommand None { get; } = Create();

        /// <summary>
        /// The content is resized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        public static RoutedUICommand Uniform { get; } = Create();

        /// <summary>
        /// The content is resized to fill the destination dimensions while it preserves its native aspect ratio. If the aspect ratio of the destination rectangle differs from the source, the source content is clipped to fit in the destination dimensions.
        /// </summary>
        public static RoutedUICommand UniformToFill { get; } = Create();

        public static RoutedUICommand Increase { get; } = Create();

        public static RoutedUICommand Decrease { get; } = Create();

        private static RoutedUICommand Create([CallerMemberName] string name = null)
        {
            return new RoutedUICommand(name, name, typeof(ZoomCommands));
        }
    }
}