using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Framework.Scripts.Common
{
    public interface ITransform
    {
        Transform transform { get; }
    } 
    /// <summary>
    ///     A KD-Tree Nearest Behaviour Data Structure for use within unity. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KdTree<T> : IEnumerable<T> where T : class, ITransform
    {
        protected KdNode m_RootNode;
        protected KdNode m_PreviousNode;
        protected int m_Count;
        protected bool m_Use2DOnly;
        protected float m_LastUpdate = -1.0f;
        protected KdNode[] m_OpenNodes;

        public int Count { get => m_Count; }
        public bool IsReadOnly { get => false; }
        public float AverageSearchLength { protected set; get; }
        public float AverageSearchDeep { protected set; get; }

        /// <summary>
        /// create a tree
        /// </summary>
        /// <param name="Use2DOnly">just use x/z</param>
        public KdTree(bool Use2DOnly = false)
        {
            m_Use2DOnly = Use2DOnly;
        }

        public T this[int key]
        {
            get
            {
                if (key >= m_Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                KdNode current = m_RootNode;
                for(int i = 0; i < key; i++)
                {
                    current = current.Next;
                }
                return current.Component;
            }
        }

        /// <summary>
        /// add item
        /// </summary>
        /// <param name="item">item</param>
        public void Add(T item)
        {
            AddNode_Internal(
                new KdNode{ Component = item });
        }

        /// <summary>
        /// batch add items
        /// </summary>
        /// <param name="Collection">items</param>
        public void AddAll(List<T> Collection)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Add(Collection[i]);
            }
        }

        /// <summary>
        ///     Find all objects that matches the given predicate
        /// </summary>
        /// <param name="Predicate">lamda expression</param>
        public KdTree<T> FindAll(Predicate<T> Predicate)
        {
            KdTree<T> list = new KdTree<T>(m_Use2DOnly);
            for (int i = 0; i < this.Count; i++)
            {
                if (Predicate(this[i]))
                {
                    list.Add(this[i]);
                }
            }
            return list;
        }
        /// <summary>
        ///     Find first object that matches the given predicate
        /// </summary>
        /// <param name="Predicate">lamda expression</param>
        public T Find(Predicate<T> Predicate)
        {
            KdNode current = m_RootNode;
            while (current != null)
            {
                if (Predicate(current.Component))
                {
                    return current.Component;
                }
                current = current.Next;
            }
            return default(T);
        }
        
        /// <summary>
        ///      Find's the index of the first node that matches the given predicate 
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        public int IndexOf(T Node)
        {
            KdNode current = m_RootNode;
            int index = 0;
            while (current != null)
            {
                if (current.Component == Node)
                {
                    return index;
                }
                current = current.Next;
                index++;
            }
            return -1;
        }
        
        /// <summary>
        ///     Remove at position i (position in list or loop)
        /// </summary>
        public void RemoveAt(int PositionIndex)
        {
            List<KdNode> list = new List<KdNode>(GetNodes_Internal());
            list.RemoveAt(PositionIndex);
            Clear();
            foreach (KdNode node in list)
            {
                node.Next = null;
                AddNode_Internal(node);
            }
        }
 
        /// <summary>
        /// remove all objects that matches the given predicate
        /// </summary>
        /// <param name="Predicate">lamda expression</param>
        public void RemoveWhere(Predicate<T> Predicate)
        {
            List<KdNode> list = new List<KdNode>(GetNodes_Internal());
            list.RemoveAll(n => Predicate(n.Component));
            Clear();
            for (int i = 0; i < list.Count; i++)
            {
                KdNode node = list[i];
                node.Next = null;
                AddNode_Internal(node);
            }
        }
        
        /// <summary>
        /// count all objects that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        /// <returns>matching object count</returns>
        public int CountAll(Predicate<T> match)
        {
            int count = 0;
            for (int i = 0; i < this.Count; i++)
            {
                T node = this[i];
                if (match(node))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        ///     Clear tree
        /// </summary>
        public void Clear()
        {
            //rest for the garbage collection
            m_RootNode = null;
            m_PreviousNode = null;
            m_Count = 0;
        }

        /// <summary>
        ///     Update positions (if objects moved)
        /// </summary>
        /// <param name="dt">Updates per second</param>
        public void UpdatePositions(float dt)
        {
            if (Time.timeSinceLevelLoad - m_LastUpdate < 1.0f / dt)
            {
                return;
            }
            m_LastUpdate = Time.timeSinceLevelLoad;
            UpdatePositions_Internal();
        }
        
        /// <summary>
        /// Method to enable foreach-loops
        /// </summary>
        /// <returns>Enumberator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            KdNode current = m_RootNode;
            while (current != null)
            {
                yield return current.Component;
                current = current.Next;
            }
        }

        /// <summary>
        /// Convert to list
        /// </summary>
        /// <returns>list</returns>
        public List<T> ToList()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add(this[i]);
            }
            return list;
        }

        /// <summary>
        /// Method to enable foreach-loops
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Find closest object to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>closest object</returns>
        public T FindClosest(Vector3 position)
        {
            return FindClosestNode_Internal(position);
        }

        /// <summary>
        ///     Find closest object to given position
        /// </summary>
        /// <param name="position">The position to query against</param>
        /// <param name="distance">The maximum distance, default (30u)</param>
        /// <param name="Self">Ignore self in the query</param>
        /// <returns></returns>
        public T FindClosest(Vector3 position, Predicate<T> Condition, float Distance = 50.0f)
        {
            return FindClosestNodeWithinRange_Internal(position, Distance, Condition); 
        }


        /// <summary>
        ///     Find
        /// </summary>
        /// <param name="position"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public List<T> FindWithinRange(Vector3 position, out T closest, float Distance = 50.0f, Predicate<T> Condition = null)
        {
            List<T> nearby = new List<T>();
            float closestValue = float.MaxValue;
            closest = default(T);
            foreach (T type in FindNearby(position))
            {
                float magnitude = (type.transform.position - position).sqrMagnitude;
                if (magnitude < Distance * Distance)
                {
                    if (magnitude < closestValue)
                    {
                        closestValue = magnitude;
                        closest = type;
                    }
                    nearby.Add(type);
                }
                if (Condition != null)
                {
                    nearby.RemoveAll(Condition);
                }
            }
            return nearby;
        }

        /// <summary>
        ///     Find's all objects within range of the given position, ordered by distance (with result count limit)
        /// </summary>
        /// <param name="Position">The position reference</param>
        /// <param name="Distance">The distance from the position to check</param>
        /// <param name="LimitResults">The (max) number of results</param>
        /// <param name="Condition">The match case condition to be met, if not null.</param>
        /// <returns></returns>
        public List<T> FindWithinRangeOrderedWithLimit(Vector3 Position, float Distance, int LimitResults, Predicate<T> Condition = null)
        {
            List<T> nearby = new List<T>();
            foreach (T type in FindNearby(Position))
            {
                float magnitude = (type.transform.position - Position).sqrMagnitude;
                if (magnitude < Distance * Distance)
                {
                    nearby.Add(type);
                }
            }
            if (Condition != null)
            {
                nearby.RemoveAll(Condition);
            }
            nearby.Sort((a, b) => (a.transform.position - Position).sqrMagnitude.CompareTo((b.transform.position - Position).sqrMagnitude));
            if (nearby.Count > LimitResults)
            {
                nearby.RemoveRange(LimitResults, nearby.Count - LimitResults);
            }
            return nearby;
        }
        
        /// <summary>
        ///     Find all nearby objects from the given position, with condition 
        /// </summary>
        /// <param name="position">The position to query</param>
        /// <param name="IgnoreSelf">Whether to ignore self in the query</param>
        /// <param name="Conditional">A condition the results need to pass</param>
        /// <returns></returns>
        public IEnumerable<T> FindNearby(Vector3 position, Predicate<T> Conditional)
        {
            return FindNearby(position).Where(x => Conditional(x)); 
        }
        
        /// <summary>
        /// Find close objects to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>close object</returns>
        public IEnumerable<T> FindNearby(Vector3 position)
        {
            List<T> allNearbyNodes = new List<T>();
            FindClosestNode_Internal(position, allNearbyNodes);
            return allNearbyNodes;
        }

        /// <summary>
        ///     Internally find's the closest nodes within a range internally 
        /// </summary>
        /// <param name="position">The position to query against</param>
        /// <param name="MaxDistance">The maximum distance threshold</param>
        /// <param name="IgnoreSelf">If IgnoreSelf is specified, won't return the specified entry in the query </param>
        /// <returns></returns>
        protected virtual T FindClosestNodeWithinRange_Internal(Vector3 position, float MaxDistance, Predicate<T> Condition)
        {
            T closest = default(T);
            float distance = float.MaxValue; 
            List<T> nearby = FindNearby(position).Where(x => Condition(x)).ToList();
            for (int i = 0; i < nearby.Count; ++i)
            {
                float between = Vector3.Distance(position, nearby[i].transform.position);
                if (between > MaxDistance)
                {
                    continue; 
                }
                if (between < distance)
                {
                    closest = nearby[i];
                    distance = between;
                } 
            }
            return closest; 
        }
        
           /// <summary>
        ///     Find's the closest node to the given position, if List is not null, it will add all nodes that are within the given range to the list<br/><br/> 
        ///     <b>Note:</b>if the list contains the same object that is calling this method, it will be included also
        /// </summary>
        /// <param name="position">The position to query</param>
        /// <param name="traversed">The output list of the nodes, if specified. Otherwise, null.</param>
        /// <returns>Returns the closest node to a position</returns> 
        protected virtual T FindClosestNode_Internal(Vector3 position, List<T> traversed = null)
        {
            if (m_RootNode == null)
            {
                return null;
            }
            float closest = float.MaxValue;
            KdNode nearest = null;
            if (m_OpenNodes == null || m_OpenNodes.Length < Count)
            {
                m_OpenNodes = new KdNode[Count];
            }
            for (int i = 0; i < m_OpenNodes.Length; i++)
            {
                m_OpenNodes[i] = null;
            }

            int openAdd = 0;
            int openCur = 0;

            if (m_RootNode != null)
            {
                m_OpenNodes[openAdd++] = m_RootNode;
            }

            while (openCur < m_OpenNodes.Length && m_OpenNodes[openCur] != null)
            {
                KdNode current = m_OpenNodes[openCur++];
                traversed?.Add(current.Component);

                float nodeDist = GetNodeDistance_Internal(position, current.Component.transform.position);
                if (nodeDist < closest)
                {
                    closest = nodeDist;
                    nearest = current;
                }
                float currentSplitNode = GetSplitNodeValue_Internal(current);
                float currentSplitSearch = GetSplitNodeValue_Internal(current.Level, position);

                if (currentSplitSearch < currentSplitNode)
                {
                    if (current.Left != null)
                    {
                        m_OpenNodes[openAdd++] = current.Left; //go left
                    }
                    if (Mathf.Abs(currentSplitNode - currentSplitSearch) * Mathf.Abs(currentSplitNode - currentSplitSearch) < closest && current.Right != null)
                    {
                        m_OpenNodes[openAdd++] = current.Right; //go right
                    }
                }
                else
                {
                    if (current.Right != null)
                    {
                        m_OpenNodes[openAdd++] = current.Right; //go right
                    }
                    if (Mathf.Abs(currentSplitNode - currentSplitSearch) * Mathf.Abs(currentSplitNode - currentSplitSearch) < closest && current.Left != null)
                    {
                        m_OpenNodes[openAdd++] = current.Left; //go left
                    }
                }
            }
            AverageSearchLength = (99.0f * AverageSearchLength + openCur) / 100.0f;
            AverageSearchDeep = (99.0f * AverageSearchDeep + nearest.Level) / 100.0f;
            return nearest.Component;
        }


        #if UNITY_EDITOR 
        /// <summary>
        ///     Draw's the bound's of all nodes in the tree recursively. <br/>
        ///     <b>Note:</b> This is only for debugging and MUST be called from either <see cref="OnDrawGizmos()"/> or <see cref="OnDrawGizmosSelected()"/>
        /// </summary>
        public void DrawAllBounds()
        {
            KdNode current = m_RootNode;
            while (current != null)
            {
                current.DrawAllBounds(); 
                current = current.Next;
            }
        }

        /// <summary>
        ///     Draw's the objects in the tree recursively 
        ///     <b>Note:</b> This is only for debugging and MUST be called from either <see cref="OnDrawGizmos()"/> or <see cref="OnDrawGizmosSelected()"/>
        /// </summary>
        public void DrawAllObjects()
        {
            KdNode current = m_RootNode;
            while(current != null)
            {
                current.DrawAllObjects(); 
                current = current.Next; 
            }
        }
        #endif
        
        /// <summary>
        ///     Update positions (if objects moved)
        /// </summary>
        protected void UpdatePositions_Internal()
        {
            KdNode current = m_RootNode;
            while (current != null)
            {
                current.m_PreviousReference = current.Next;
                current = current.Next;
            }
            current = m_RootNode;
            Clear();
            while (current != null)
            {
                AddNode_Internal(current);
                current = current.m_PreviousReference;
            }
        }

        
        protected float GetNodeDistance_Internal(Vector3 a, Vector3 b)
        {
            if (m_Use2DOnly)
            {
                return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
            }
            else
            {
                return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
            }
        }
        
        protected float GetSplitNodeValue_Internal(int level, Vector3 position)
        {
            if (m_Use2DOnly)
            {
                return (level % 2 == 0) ? position.x : position.z;
            }
            else
            {
                return (level % 3 == 0) ? position.x : (level % 3 == 1) ? position.y : position.z;
            }
        }

        protected void AddNode_Internal(KdNode NewNode)
        {
            m_Count++;
            NewNode.Left = null;
            NewNode.Right = null;
            NewNode.Level = 0;
            KdNode parent = FindParentNode_Internal(NewNode.Component.transform.position);

            //set last
            if (m_PreviousNode != null)
            {
                m_PreviousNode.Next = NewNode;
            }
            m_PreviousNode = NewNode;

            //set root
            if (parent == null)
            {
                m_RootNode = NewNode;
                return;
            }
            float splitParent = GetSplitNodeValue_Internal(parent);
            float splitNew = GetSplitNodeValue_Internal(parent.Level, NewNode.Component.transform.position);
        
            NewNode.Level = parent.Level + 1;
            if (splitNew < splitParent)
            {
                parent.Left = NewNode; //go left
            }
            else
            {
                parent.Right = NewNode; //go right
            }
        }

        protected KdNode FindParentNode_Internal(Vector3 Position)
        {
            KdNode current = m_RootNode;
            KdNode parent = m_RootNode;
            while (current != null)
            {
                float splitCurrent = GetSplitNodeValue_Internal(current);
                float splitSearch = GetSplitNodeValue_Internal(current.Level, Position);
                parent = current;
                if (splitSearch < splitCurrent)
                {
                    current = current.Left; //go left
                }
                else
                {
                    current = current.Right; //go right
                }
            }
            return parent;
        }
        
        protected float GetSplitNodeValue_Internal(KdNode node)
        {
            return GetSplitNodeValue_Internal(node.Level, node.Component.transform.position);
        }

        protected IEnumerable<KdNode> GetNodes_Internal()
        {
            KdNode current = m_RootNode;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        /// <summary>
        ///     Protected class that represents a node in the kd-tree structure
        /// </summary>
        protected class KdNode 
        {
            internal T Component;
            internal int Level;
            internal KdNode Left;
            internal KdNode Right;
            internal KdNode Next;
            internal KdNode m_PreviousReference;

            #if UNITY_EDITOR 
            public void DrawAllBounds()
            {
                Gizmos.color = new Color(1.0f, 0.5f, 1.0f, 0.8f);
                Bounds bounds = new Bounds(Component.transform.position, GetParentBounds(Component.transform.gameObject).size); 
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }

            public void DrawAllObjects()
            {
                Gizmos.color = new Color(0, 0, 1, 0.75f); 
                Gizmos.DrawIcon(Component.transform.position, "marker.tif",true); 
            }
            
            private static Bounds GetParentBounds(GameObject gameObject)
            {
                Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
                foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
                {
                    bounds.Encapsulate(renderer.bounds);
                }
                return bounds;
            } 
            #endif
        }
    }
}