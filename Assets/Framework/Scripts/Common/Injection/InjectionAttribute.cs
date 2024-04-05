using System;
namespace Framework.Scripts.Common.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectionAttribute : UnityEngine.PropertyAttribute { }
}