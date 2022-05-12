using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(false)]

[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100D3687DF6F8B0E6A6B7B73F276803765856327CEAB4EA9F369BCD6F30E59E0586DE0DF248F8F8B7A541D4FCB44F2E20785F90969D3A6BC83257A49E5B005D683C3C44DBCD8088C4C7C6445A29078E6781F7C53AFB8EB381F4C0D6EB6ED11FA0062B2BC6E554157ADC6BF83EF16AC4E07E4CFBBAFA3E668B55F5A4D05DE18A34BB", AllInternalsVisible = true)]
[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Demo, PublicKey=0024000004800000940000000602000000240000525341310004000001000100D3687DF6F8B0E6A6B7B73F276803765856327CEAB4EA9F369BCD6F30E59E0586DE0DF248F8F8B7A541D4FCB44F2E20785F90969D3A6BC83257A49E5B005D683C3C44DBCD8088C4C7C6445A29078E6781F7C53AFB8EB381F4C0D6EB6ED11FA0062B2BC6E554157ADC6BF83EF16AC4E07E4CFBBAFA3E668B55F5A4D05DE18A34BB", AllInternalsVisible = true)]
[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Benchmarks, PublicKey=0024000004800000940000000602000000240000525341310004000001000100D3687DF6F8B0E6A6B7B73F276803765856327CEAB4EA9F369BCD6F30E59E0586DE0DF248F8F8B7A541D4FCB44F2E20785F90969D3A6BC83257A49E5B005D683C3C44DBCD8088C4C7C6445A29078E6781F7C53AFB8EB381F4C0D6EB6ED11FA0062B2BC6E554157ADC6BF83EF16AC4E07E4CFBBAFA3E668B55F5A4D05DE18A34BB", AllInternalsVisible = true)]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: XmlnsDefinition("http://gu.se/Geometry", "Gu.Wpf.Geometry")]
[assembly: XmlnsDefinition("http://gu.se/Geometry", "Gu.Wpf.Geometry.Effects")]
[assembly: XmlnsPrefix("http://gu.se/Geometry", "geometry")]
[assembly: XmlnsPrefix("http://gu.se/Geometry", "effects")]

#if NET48
#pragma warning disable SA1402, SA1502, SA1600, SA1649, GU0073
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class AllowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class DisallowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class MaybeNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class NotNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName) => this.ParameterName = parameterName;

        public string ParameterName { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue) => this.ParameterValue = parameterValue;

        public bool ParameterValue { get; }
    }
}
#endif
