using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Common.Attributes;
using Framework.Scripts.Common.TriggerToolkit;
using Framework.Scripts.Core.ObjectPool;
using UnityEngine;
namespace Framework.Scripts.Core.Collectibles
{
    /// <summary>
    /// Represents a collectible item in the game. Uses the 
    /// </summary>
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] protected TriggerToolkit TriggerDetection;
        [SerializeField] protected SpriteRenderer SpriteRenderer;
        [SerializeField] protected Animator Animator; 
        
        public CollectibleItemSettings Settings { get; private set; } 
        
        public virtual async void Initialize(CollectibleItemSettings Settings)
        {
            this.Settings = Settings;
            this.SpriteRenderer.sprite = Settings.Sprite;
            await new WaitForSeconds(UnityEngine.Random.Range(0.0f, 1.0f)); 
            Animator.SetTrigger(Animations.Hover); 
        }

        protected void OnEnable()
        {
            gameObject.AssignIfNull(ref TriggerDetection);
            gameObject.AssignIfNull(ref SpriteRenderer);
            gameObject.AssignIfNull(ref Animator);
            TriggerDetection.OnTriggerEvent += OnDetection;
        }

        protected void OnDisable()
        {
            TriggerDetection.OnTriggerEvent -= OnDetection; 
        }

        private void OnDetection(Collider2D Collider, TriggerCondition.ETriggerEventType EventType)
        {
            if (EventType is TriggerCondition.ETriggerEventType.ENTER)
            {
                this.TriggerDetection.enabled = false; 
                Settings.CollectibleEventChannel.Invoke(Settings.ScoreValue);
                if (Settings.CollectionEffect != null)
                {
                    FlyweightBehaviour instance = ObjectPoolFactory.Spawn(Settings.CollectionEffect);
                    instance.transform.position = transform.position; 
                    instance.transform.rotation = Quaternion.identity;
                    ObjectPoolFactory.DespawnAfterDelay(instance, Settings.CollectionEffectDuration);
                    Destroy(this.gameObject, 0.01f); 
                }
            }
         }
    }

}