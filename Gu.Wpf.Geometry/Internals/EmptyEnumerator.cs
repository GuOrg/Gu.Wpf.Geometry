namespace Gu.Wpf.Geometry
{
    using System;
    using System.Collections;

    internal class EmptyEnumerator : IEnumerator
    {
        internal static readonly IEnumerator Instance = new EmptyEnumerator();

        private EmptyEnumerator()
        {
        }

        object IEnumerator.Current => throw new InvalidOperationException();

        void IEnumerator.Reset()
        {
        }

        public bool MoveNext()
        {
            return false;
        }
    }
}
