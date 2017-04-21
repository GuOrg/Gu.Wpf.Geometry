namespace Gu.Wpf.Geometry.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Markup;

    using Xunit;

    public class NamespacesTests
    {
        private const string Uri = "http://gu.se/Geometry";

        private readonly Assembly assembly;

        public NamespacesTests()
        {
            this.assembly = typeof(GradientPath).Assembly;
        }

        [Fact]
        public void XmlnsDefinitions()
        {
            string[] skip = { ".Annotations", ".Properties", "XamlGeneratedNamespace" };

            var strings = this.assembly.GetTypes()
                                   .Select(x => x.Namespace)
                                   .Distinct()
                                   .Where(x => !skip.Any(x.EndsWith))
                                   .OrderBy(x => x)
                                   .ToArray();
            var attributes = this.assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsDefinitionAttribute))
                                      .ToArray();
            var actuals = attributes.Select(a => a.ConstructorArguments[1].Value)
                                    .OrderBy(x => x);
            foreach (var s in strings)
            {
                Console.WriteLine(@"[assembly: XmlnsDefinition(""{0}"", ""{1}"")]", Uri, s);
            }

            Assert.Equal(strings, actuals);
            foreach (var attribute in attributes)
            {
                Assert.Equal(Uri, attribute.ConstructorArguments[0].Value);
            }
        }

        [Fact]
        public void XmlnsPrefix()
        {
            var prefixAttribute = this.assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsPrefixAttribute));
            Assert.All(prefixAttribute.Select(a => a.ConstructorArguments[0].Value), x => Assert.Equal(Uri, x));
        }
    }
}