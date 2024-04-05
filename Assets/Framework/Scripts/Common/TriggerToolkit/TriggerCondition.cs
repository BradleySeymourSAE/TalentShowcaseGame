using System.Collections.Generic;
using Framework.Common;
using UnityEngine;
using UnityEngine.Events;
namespace Framework.Scripts.Common.TriggerToolkit
{

    [System.Serializable]
    public class TriggerCondition
    {
        public enum ETriggerEventType
        {
            ENTER,
            EXIT,
            STAY
        }

        [SerializeField] private ETriggerEventType TriggerConditionType;
        [SerializeField] private List<string> Tags;
        [SerializeField] private float FloatValue;
        [SerializeField] private UnityEvent OnTrigger = new();
        [System.NonSerialized] private Dictionary<GameObject, float> TriggerEntryTimes = new();
        [System.NonSerialized] private Dictionary<GameObject, bool> NotificationSent = new();

        public System.Action<Collider2D, ETriggerEventType> OnTriggerAction;

        internal void HandleOnEnter(Collider2D other, ETriggerEventType EventType)
        {
            if (TriggerConditionType != ETriggerEventType.ENTER)
            {
                return;
            }
            if (MatchedConditionTag(other.gameObject.tag) == false)
            {
                return;
            }
            //  console.log(this, "OnTriggerEnter method has been invoked by : ", other.gameObject.name);
            OnTrigger.Invoke();
            OnTriggerAction?.Invoke(other, EventType);
        }

        internal void HandleOnStay(Collider2D other, ETriggerEventType EventType)
        {
            if (TriggerConditionType != ETriggerEventType.STAY)
            {
                return;
            }
            if (MatchedConditionTag(other.gameObject.tag) == false)
            {
                return;
            }

            if (TriggerEntryTimes.ContainsKey(other.gameObject) && !NotificationSent.ContainsKey(other.gameObject))
            {
                if ((Time.time - TriggerEntryTimes[other.gameObject]) > FloatValue)
                {
                    NotificationSent[other.gameObject] = true;
                    console.log(this, "OnTriggerStay method has been invoked by : ", other.gameObject.name);
                    OnTrigger.Invoke();
                    OnTriggerAction?.Invoke(other, EventType);
                }
            }
            else
            {
                TriggerEntryTimes[other.gameObject] = Time.time;
            }
        }

        internal void HandleOnExit(Collider2D other, TriggerCondition.ETriggerEventType EventType)
        {
            TriggerEntryTimes.Remove(other.gameObject);
            NotificationSent.Remove(other.gameObject);

            if (TriggerConditionType != ETriggerEventType.EXIT)
            {
                return;
            }
            if (MatchedConditionTag(other.gameObject.tag) == false)
            {
                return;
            }

           // console.log(this, "OnTriggerExit method has been invoked by : ", other.gameObject.name);
            OnTrigger.Invoke();
            OnTriggerAction?.Invoke(other, EventType);
        }

        private bool MatchedConditionTag(Collider2D other) => MatchedConditionTag(other.gameObject.tag);
        private bool MatchedConditionTag(string tag) => Tags.Contains(tag);

        public void DebugLogTriggerConditionTags()
        {
            foreach (string tag in Tags)
            {
                console.log(this, "Trigger Condition Tag: ", tag);
            }
        }
    }
}