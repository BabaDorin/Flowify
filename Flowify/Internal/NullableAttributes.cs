#if NETSTANDARD2_0

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class AllowNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class DisallowNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class DoesNotReturnAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;
        public bool ParameterValue { get; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    internal sealed class MaybeNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    internal sealed class NotNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;
        public string ParameterName { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) => Members = new[] { member };
        public MemberNotNullAttribute(params string[] members) => Members = members;
        public string[] Members { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = new[] { member };
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        public bool ReturnValue { get; }
        public string[] Members { get; }
    }
}

#endif
