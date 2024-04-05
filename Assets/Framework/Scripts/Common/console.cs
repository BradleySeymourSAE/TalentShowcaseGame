// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace Framework.Scripts.Common
{
    public static class console
    {
        public static string GetCurrentMethodName() => new StackFrame(1).GetMethod().Name;
        public static string GetCurrentClassName() => new StackFrame(1).GetMethod().DeclaringType?.Name;
        public static int GetCurrentLineNumber() => new StackFrame(1, true).GetFileLineNumber();

        public static string GetCurrentClassAndMethod()
        {
            MethodBase method = new StackFrame(1).GetMethod();
            return $"{method.DeclaringType?.Name}.{method.Name}";
        }

        public static string GetCurrentClassMethodAndLine()
        {
            StackFrame frame = new StackFrame(1, true);
            MethodBase method = frame.GetMethod();
            return $"{method.DeclaringType?.Name}.{method.Name}({frame.GetFileLineNumber()})";
        }

        public static string GetCurrentMethodSignatureAndLine()
        {
            StackFrame frame = new StackFrame(1, true);
            MethodBase method = frame.GetMethod();
            return $"({method}):{frame.GetFileLineNumber()}";
        }

        public static void log(object message)
        {
            Debug.Log($"{GetCurrentClassAndMethod()}: {message}");
        }
        
        

        public static void warn(object message)
        {
            Debug.LogWarning($"{GetCurrentClassAndMethod()}:{message}");
        }

        public static void error(object message)
        {
            Debug.LogError($"{GetCurrentClassAndMethod()}:{message}");
        }

        public static void assert(bool condition, object message)
        {
            Debug.Assert(condition, CheckLogMessageHasComponent(typeof(console)) + $"{message}");
        }

        /// <summary>
        ///     Checks if the component parameter is null, if it is, then returns an empty string, if not then returns a formatted string of the components name 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private static string CheckLogMessageHasComponent(object component)
        {
            return component == null ? "" : $"[{component.GetType().Name}]:";
        }

        public static void log([CanBeNull] object component, params object[] message)
        {
            Debug.Log($"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        public static void logformat([CanBeNull] object component, params object[] message)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            int length = message.Length;
            for (int i = 0; i < length; i++)
            {
                switch (i % 2)
                {
                    case 0:
                        keys.Add(message[i].ToString());
                        break;
                    default:
                        values.Add(message[i].ToString());
                        break;
                }
            }

            Debug.Log($"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", keys.Zip(values, (k, v) => $"{k} : {v} \n"))}");
        }

        public static void warn([CanBeNull] object component, params object[] message)
        {
            Debug.LogWarning($"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        public static void error([CanBeNull] object component, params object[] message)
        {
            Debug.LogError($"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        public static void exception([CanBeNull] object component, Exception Exception, params object[] message)
        {
            error(component, $"{string.Join(" ", message)} {Exception.Message}");
            Debug.LogException(Exception);
        }

        public static void assert([CanBeNull] object component, bool condition, params object[] message)
        {
            Debug.Assert(condition, $"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        public static void is_true([CanBeNull] object component, bool condition, params object[] message)
        {
            Assert.IsTrue(condition, $"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        /// <summary>
        ///     Asserts that the condition is true and that the 'expected' and 'value' types are equal 
        /// </summary>
        /// <param name="component">typeof(this) component calling the method</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to log if the condition isn't equal to the expected value type</param>
        /// <typeparam name="T">The value type (int|class|struct|etc)</typeparam>
        /// <example>
        ///     <code>
        ///     int x = 1;
        ///     int y = 1;
        ///     Doesn't print a message to the console because x == y 
        ///     console.assertAreEqual(this, x, y, "Expected: 1, Actual: 2");
        ///     Does print a message to the console in this case 
        ///     console.assertNotEqual(this, x, y, "Value", x, "is not equal to value", y);
        ///     </code>
        /// </example>
        public static void assertAreEqual<T>([CanBeNull] object component, T expected, T actual, params object[] message)
        {
            Assert.AreEqual(expected, actual, $"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        /// <summary>
        ///     Asserts that the condition is NOT true and that the 'expected' and 'value' types are not equal 
        /// </summary>
        /// <param name="component">typeof(this) component calling the method</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to log if the condition isn't equal to the expected value type</param>
        /// <typeparam name="T"></typeparam>
        ///     <code>
        ///     int x = 1;
        ///     int y = 2;
        ///     Prints a message to the console because x != y 
        ///     console.assertAreEqual(this, x, y, "Value", x, "is not equal to value", y);
        ///     Doesn't print a message to the console in this case because x is in fact not equal which is what we want  
        ///     console.assertNotEqual(this, x, y, "Value", x, "not equal to", y);
        ///     </code>
        public static void assertNotEqual<T>([CanBeNull] object component, T expected, T actual, params object[] message)
        {
            Assert.AreNotEqual(expected, actual, $"{CheckLogMessageHasComponent(component)}" + $"{string.Join(" ", message)}");
        }

        public static void log_red([CanBeNull] object component, params object[] message)
        {
            console.log(component?.GetType().Name, StartMarker("red"), message, EndMarker());
        }

        public static void log_orange([CanBeNull] object component, params object[] message)
        {
            console.log(component?.GetType().Name, StartMarker("#D1681D"), message, EndMarker());
        }

        public static void log_yellow([CanBeNull] object component, params object[] message)
        {
            console.log(component?.GetType().Name, StartMarker("#E0D300"), message, EndMarker());
        }

        /// <summary>
        ///     Rich text marker for color
        ///    Can be either hexadecimal or named color 
        /// </summary>
        private static string StartMarker(string ColorValue)
        {
            return "<color='" + ColorValue + "'>";
        }

        /// <summary>
        /// close rich text marker for color
        /// </summary>
        private static string EndMarker()
        {
            return "</color>";
        }
    }
}