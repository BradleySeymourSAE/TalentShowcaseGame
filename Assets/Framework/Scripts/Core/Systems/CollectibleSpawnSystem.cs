using System;
using System.Collections.Generic;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Collectibles;
using Framework.Scripts.Core.ObjectPool;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Framework.Scripts.Core.Systems
{
    
    [System.Serializable]
    public class WeightedCollectibleItemChance
    {
        public CollectibleItemSettings CollectibleItemSettings;
        [Range(0.0f,1.0f)] public float Weight = 0.5f; 
    } 
    
    public sealed class CollectibleSpawnSystem : Singleton<CollectibleSpawnSystem>
    {
        [SerializeField] private List<WeightedCollectibleItemChance> m_CollectibleItemSettings = new(); 
        [SerializeField] private Transform[] m_SpawnPoints = Array.Empty<Transform>();

        private void Start()
        {
            foreach (Transform point in m_SpawnPoints)
            {
                float random = UnityEngine.Random.value; 
                float totalWeight = 0.0f; 
                foreach (WeightedCollectibleItemChance item in m_CollectibleItemSettings)
                {
                    totalWeight += item.Weight; 
                    if (random <= totalWeight)
                    {
                        CollectibleItem collectibleItem = Object.Instantiate(item.CollectibleItemSettings.Prefab).GetComponent<CollectibleItem>(); 
                        collectibleItem.transform.position = point.position; 
                        collectibleItem.Initialize(item.CollectibleItemSettings); 
                        break; 
                    }
                } 
            }
        } 
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_SpawnPoints.Length == 0)
            {
                return; 
            } 
            
            foreach (var point in m_SpawnPoints)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
                UnityEditor.Handles.Label(point.position, point.name); 
            }
        }
        #endif 
    }
}