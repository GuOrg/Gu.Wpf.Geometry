namespace Gu.Wpf.Geometry.Tests;

using System.Linq;
using System.Windows.Markup;

using NUnit.Framework;

public static class NamespacesTests
{
    private const string Uri = "http://gu.se/Geometry";

    [Test]
    public static void XmlnsPrefix()
    {
        var attributes = typeof(GradientPath).Assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsPrefixAttribute));
        foreach (var attribute in attributes)
        {
            Assert.AreEqual(Uri, attribute.ConstructorArguments[0].Value);
        }
    }
}
