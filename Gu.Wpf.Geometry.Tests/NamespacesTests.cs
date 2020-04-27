namespace Gu.Wpf.Geometry.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Markup;

    using NUnit.Framework;

    public class NamespacesTests
    {
        private const string Uri = "http://gu.se/Geometry";

        private readonly Assembly assembly;

        public NamespacesTests()
        {
            this.assembly = typeof(GradientPath).Assembly;
        }

        [Test]
        public void XmlnsPrefix()
        {
            var attributes = this.assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsPrefixAttribute));
            foreach (var attribute in attributes)
            {
                Assert.AreEqual(Uri, attribute.ConstructorArguments[0].Value);
            }
        }
    }
}
