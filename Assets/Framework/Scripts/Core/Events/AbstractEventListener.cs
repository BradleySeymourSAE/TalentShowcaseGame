using UnityEngine;
using UnityEngine.Events;
namespace Framework.Scripts.Core.Events
{
    
    public abstract class AbstractEventListener<T> : MonoBehaviour {
        [SerializeField] private AbstractEventChannel<T> m_EventChannel;
        [SerializeField] private UnityEvent<T> m_Event;

        protected void Awake() {
            m_EventChannel.Register(this);
        }
        
        protected void OnDestroy() {
            m_EventChannel.Deregister(this);
        }
        
        public void Raise(T value) {
            m_Event?.Invoke(value);
        }
    }
    
    public class AbstractEventListener : AbstractEventListener<Empty> { }

}