using System.Collections.Generic;
using UnityEngine;
namespace Framework.Scripts.Core.Events
{
    public abstract class AbstractEventChannel<T> : ScriptableObject {
        private readonly HashSet<AbstractEventListener<T>> m_Observers = new();

        public void Invoke(T value) {
            foreach (AbstractEventListener<T> observer in m_Observers) {
                observer.Raise(value);
            }
        }
        
        public void Register(AbstractEventListener<T> observer) => m_Observers.Add(observer);
        public void Deregister(AbstractEventListener<T> observer) => m_Observers.Remove(observer);
    }

    public readonly struct Empty { }
    
    [CreateAssetMenu(menuName = "Events/EventChannel")]
    public class AbstractEventChannel : AbstractEventChannel<Empty> { }
}