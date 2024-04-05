// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System.Collections.Generic;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Controls the rotation of any objects it is given to make them constantly face its <see cref="Transform"/>
    /// </summary>
    public class BillboardManager : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// If true, billboards will face the position of this object (looks better when the camera rotates).
        /// Otherwise they will face the same direction as this object (more efficient).
        /// </summary>
        [Tooltip("If true, billboards will face the position of this object (looks better when the camera rotates)." +
            " Otherwise they will face the same direction as this object (more efficient).")]
        public bool billboardsFacePoint;

        /// <summary>The <see cref="Component.transform"/>, cached for efficienty.</summary>
        public Transform Transform { get; private set; }

        /// <summary>The objects being managed.</summary>
        protected readonly List<Transform> Billboards = new List<Transform>();

        /************************************************************************************************************************/

        private static BillboardManager _Main;

        /// <summary>
        /// Gets the <see cref="BillboardManager"/> of the specified subtype attached to <see cref="Camera.main"/>.
        /// Automatically creates a new one if none was present.
        /// </summary>
        public static BillboardManager GetMain<T>() where T : BillboardManager
        {
            if (_Main == null)
                _Main = Camera.main.gameObject.AddComponent<T>();

            return _Main;
        }

        /// <summary>
        /// Gets the <see cref="BillboardManager"/> attached to <see cref="Camera.main"/>.
        /// Automatically creates a new one if none was present.
        /// </summary>
        public static BillboardManager GetMain() => GetMain<BillboardManager>();

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity the first time this component becomes enabled and active.
        /// Cached the <see cref="Component.transform"/>.
        /// </summary>
        protected virtual void Awake()
        {
            Transform = transform;
        }

        /************************************************************************************************************************/

        /// <summary>Adds the given `billboard` to the list of objects being managed.</summary>
        public void Add(Transform billboard)
        {
            Billboards.Add(billboard);
            enabled = true;
        }

        /// <summary>Removes the given `billboard` from the list of objects being managed.</summary>
        public void Remove(Transform billboard)
        {
            Billboards.Remove(billboard);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the rotations of all objects being managed to keep them facing the <see cref="Transform"/>.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (billboardsFacePoint)
            {
                var position = Transform.position;

                for (int i = Billboards.Count - 1; i >= 0; i--)
                {
                    var billboard = Billboards[i];
                    if (billboard != null)
                    {
                        billboard.LookAt(position);
                    }
                    else
                    {
                        Billboards.RemoveAt(i);
                    }
                }
            }
            else
            {
                var rotation = Transform.rotation;

                for (int i = Billboards.Count - 1; i >= 0; i--)
                {
                    var billboard = Billboards[i];
                    if (billboard != null)
                    {
                        billboard.rotation = rotation;
                    }
                    else
                    {
                        Billboards.RemoveAt(i);
                    }
                }
            }

            if (Billboards.Count == 0)
                enabled = false;
        }

        /************************************************************************************************************************/
    }
}
