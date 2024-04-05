using Framework.Scripts.Core.Events;
using Framework.Scripts.Core.ObjectPool;
using UnityEngine;
namespace Framework.Scripts.Core.Collectibles
{
    [CreateAssetMenu(fileName = "CollectibleItemSettings", menuName = "Collectibles/CollectibleItemSettings")]
    public class CollectibleItemSettings : FlyweightSettings
    {
        public int ScoreValue = 20;
        public bool DestroyOnCollect = true;
        public IntegerEventChannel CollectibleEventChannel;
        public float CollectionEffectDuration = 0.5f; 
        public FlyweightSettings CollectionEffect;
        public Sprite Sprite; 
    }
}