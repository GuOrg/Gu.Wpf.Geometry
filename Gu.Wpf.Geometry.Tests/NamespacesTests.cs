namespace Gu.Wpf.Geometry.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Markup;

    using Xunit;

    public class NamespacesTests
    {
        private Assembly _assembly;
        private const string Uri = @"http://gu.se/Geometry";

        public NamespacesTests()
        {
            this._assembly = typeof(GradientPath).Assembly;
        }

        [Fact]
        public void XmlnsDefinitions()
        {
            string[] skip = { ".Annotations", ".Properties", "XamlGeneratedNamespace" };

            var strings = this._assembly.GetTypes()
                                   .Select(x => x.Namespace)
                                   .Distinct()
                                   .Where(x => !skip.Any(s => x.EndsWith(s)))
                                   .OrderBy(x => x)
                                   .ToArray();
            var attributes = this._assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsDefinitionAttribute))
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
            var prefixAttribute = this._assembly.CustomAttributes.Single(x => x.AttributeType == typeof(XmlnsPrefixAttribute));
            Assert.Equal(Uri, prefixAttribute.ConstructorArguments[0].Value);
        }
    }
}