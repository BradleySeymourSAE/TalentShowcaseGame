using System;
namespace Framework.Scripts.Common.Injection
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProviderAttribute : UnityEngine.PropertyAttribute { }
}