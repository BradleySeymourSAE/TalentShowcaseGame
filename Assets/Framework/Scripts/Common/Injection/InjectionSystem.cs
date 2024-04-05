using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Common;
using UnityEngine;
namespace Framework.Scripts.Common.Injection
{
    [DefaultExecutionOrder(-1000)]
    public class InjectionSystem : MonoBehaviour
    {
        private const BindingFlags k_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly Dictionary<System.Type, object> m_Registry = new();

        private void Awake()
        {
            MonoBehaviour[] behaviours = GetBehaviours();
            IEnumerable<IDependencyProvider> providers = behaviours.OfType<IDependencyProvider>();
            foreach (IDependencyProvider provider in providers)
            {
                Register(provider);
            }
            IEnumerable<MonoBehaviour> injectableBehaviours = behaviours.Where(IsInjectable); 
            foreach (MonoBehaviour injectableBehaviour in injectableBehaviours)
            {
                Inject(injectableBehaviour);
            } 
        }

        public void Register<T>(T ProviderInstance)
        {
            m_Registry[typeof(T)] = ProviderInstance;
        }

        public void ValidateDependencies()
        {
            MonoBehaviour[] monoBehaviours = GetBehaviours();
            IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
            HashSet<Type> providedDependencies = GetProvidedDependencies(providers);

            IEnumerable<string> invalidDependencies = monoBehaviours
                .SelectMany(behaviour => behaviour.GetType().GetFields(k_BINDING_FLAGS), (behaviour, field) => new { Behaviour = behaviour, Field = field})
                .Where(t => Attribute.IsDefined(t.Field, typeof(InjectionAttribute)))
                .Where(t => !providedDependencies.Contains(t.Field.FieldType) && t.Field.GetValue(t.Behaviour) == null)
                .Select(t => $"{t.Behaviour.GetType().Name} is missing dependency {t.Field.FieldType.Name} on GameObject {t.Behaviour.gameObject.name}");
            
            List<string> invalidDependencyList = invalidDependencies.ToList();
            
            if (invalidDependencyList.Any() == false) 
            {
                console.log(this, "All dependencies are valid.");
            } 
            else 
            {
                console.error(this, $"{invalidDependencyList.Count} dependencies are invalid:");
                foreach (string invalidDependency in invalidDependencyList) {
                    console.error(this, invalidDependency);
                }
            }
        }

        public void ClearDependencies()
        {
            foreach (MonoBehaviour behaviour in GetBehaviours())
            {
                Type type = behaviour.GetType();
                IEnumerable<FieldInfo> fields = type.GetFields(k_BINDING_FLAGS).Where(member => Attribute.IsDefined(member, typeof(InjectionAttribute)));
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(behaviour, null);
                }
            }

            console.log(this, "Cleared all injectible dependencies.");
        }

        private HashSet<System.Type> GetProvidedDependencies(IEnumerable<IDependencyProvider> Providers)
        {
            HashSet<Type> providedDependencies = new HashSet<Type>();
            foreach (IDependencyProvider provider in Providers)
            {
                MethodInfo[] methods = provider.GetType().GetMethods(k_BINDING_FLAGS);
                foreach (MethodInfo method in methods) 
                {
                    if (Attribute.IsDefined(method, typeof(ProviderAttribute)))
                    {
                        Type returnType = method.ReturnType;
                        providedDependencies.Add(returnType);
                    }
                }
            }
            return providedDependencies;
        }

        private void Inject(object instance)
        {
            Type type = instance.GetType();
            // Inject into fields
            IEnumerable<FieldInfo> injectableFields = type.GetFields(k_BINDING_FLAGS).Where(member => Attribute.IsDefined(member, typeof(InjectionAttribute)));
            foreach (FieldInfo injectableField in injectableFields)
            {
                if (injectableField.GetValue(instance) != null)
                {
                    console.warn(this, $"Field '{injectableField.Name}' of class '{type.Name}' is already set.");
                    continue;
                }

                Type fieldType = injectableField.FieldType;
                object resolvedInstance = ResolveDependencyFromType(fieldType);
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to inject dependency into field '{injectableField.Name}' of class '{type.Name}'.");
                }

                injectableField.SetValue(instance, resolvedInstance);
            }

            // Inject into methods
            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(k_BINDING_FLAGS).Where(member => Attribute.IsDefined(member, typeof(InjectionAttribute)));
            foreach (MethodInfo injectableMethod in injectableMethods)
            {
                Type[] requiredParameters = injectableMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                object[] resolvedInstances = requiredParameters.Select(ResolveDependencyFromType).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                {
                    throw new Exception($"Failed to inject dependencies into method '{injectableMethod.Name}' of class '{type.Name}'.");
                }
                injectableMethod.Invoke(instance, resolvedInstances);
            }

            // Inject into properties
            IEnumerable<PropertyInfo> injectableProperties = type.GetProperties(k_BINDING_FLAGS).Where(member => Attribute.IsDefined(member, typeof(InjectionAttribute)));
            foreach (PropertyInfo injectableProperty in injectableProperties)
            {
                Type propertyType = injectableProperty.PropertyType;
                object resolvedInstance = ResolveDependencyFromType(propertyType);
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to inject dependency into property '{injectableProperty.Name}' of class '{type.Name}'.");
                }
                injectableProperty.SetValue(instance, resolvedInstance);
            }
        }

        private void Register(IDependencyProvider Provider)
        {
            MethodInfo[] methods = Provider.GetType().GetMethods(k_BINDING_FLAGS);
            foreach (MethodInfo method in methods) 
            {
                if (Attribute.IsDefined(method, typeof(ProviderAttribute)))
                {
                    Type returnType = method.ReturnType;
                    object providedInstance = method.Invoke(Provider, null);
                    if (providedInstance != null)
                    {
                        m_Registry.Add(returnType, providedInstance);
                    }
                    else
                    {
                        throw new Exception(
                            $"Provider method '{method.Name}' in class '{Provider.GetType().Name}' returned null when providing type '{returnType.Name}'.");
                    }
                }
            }
        }

        private object ResolveDependencyFromType(System.Type Type)
        {
            m_Registry.TryGetValue(Type, out object dependency); 
            return dependency;
        }

        private static MonoBehaviour[] GetBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        private static bool IsInjectable(MonoBehaviour Behaviour)
        {
            return Behaviour.GetType().GetMembers(k_BINDING_FLAGS).Any(member => Attribute.IsDefined(member, typeof(InjectionAttribute)));
        }
    }
}