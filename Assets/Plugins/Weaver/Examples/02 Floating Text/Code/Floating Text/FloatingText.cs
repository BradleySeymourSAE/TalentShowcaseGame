// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A <see cref="PoolableBehaviour{T}"/> which manages a text object in the scene. This base class is a facade to
    /// allow implementations such as <see cref="FloatingTextUnity"/> to use any text system.
    /// </summary>
    public abstract class FloatingText : PoolableBehaviour<FloatingText>
    {
        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEngine.Transform"/> of the text.</summary>
        [SerializeField]
        private Transform _Transform;

        /// <summary>The <see cref="UnityEngine.Transform"/> of the text.</summary>
        public Transform Transform => _Transform;

        /// <summary>The initial <see cref="LifeTime"/> this text will be assigned by default when shown.</summary>
        [SerializeField]
        private float _DefaultLifeTime = 1;

        /************************************************************************************************************************/

        /// <summary>The amount of time (in seconds) which this text will be shown for.</summary>
        public float LifeTime { get; set; }

        /// <summary>The amount of time since this text was shown.</summary>
        public float Timer { get; set; }

        /// <summary>
        /// The <see cref="BillboardManager"/> that controls the rotation of this text.
        /// Usually that means keeping it facing the camera.
        /// </summary>
        public BillboardManager Manager { get; private set; }

        /************************************************************************************************************************/

        /// <summary>The text string currently being shown.</summary>
        public abstract string Text { get; set; }

        /// <summary>The height of the text in world-space.</summary>
        public abstract float Size { get; set; }

        /// <summary>The <see cref="UnityEngine.Color"/> of the text.</summary>
        public abstract Color Color { get; set; }

        /************************************************************************************************************************/

        /// <summary>The <see cref="Timer"/> rescaled to between 0-1 based on the total <see cref="LifeTime"/>.</summary>
        public float Progress
        {
            get => Timer / LifeTime;
            set => Timer = value * LifeTime;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component is first added (in Edit Mode) or the "Reset" function is used.
        /// </summary>
        public virtual void Reset()
        {
            _Transform = transform;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sets the details of this text and activates its <see cref="GameObject"/>.
        /// </summary>
        public void Show(string text, Vector3 position, BillboardManager manager)
        {
            this.EditorSetName(text);

            Text = text;
            _Transform.position = position;
            LifeTime = _DefaultLifeTime;
            Timer = 0;

            Manager = manager;
            manager.Add(_Transform);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Calls <see cref="Show(string, Vector3, BillboardManager)"/> using <see cref="BillboardManager.GetMain"/> as
        /// the `manager`.
        /// </summary>
        public void Show(string text, Vector3 position)
        {
            Show(text, position, BillboardManager.GetMain());
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the <see cref="Timer"/> and checks if it has exceeded the <see cref="LifeTime"/>.
        /// If so, this text is returned to its <see cref="ObjectPool{T}"/> if it was created by one, otherwise this
        /// text is destroyed.
        /// </summary>
        protected virtual void Update()
        {
            Timer += Time.deltaTime;
            if (Timer >= LifeTime)
            {
                this.TryReleaseOrDestroyGameObject();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by the <see cref="PoolableBehaviour{T}.Pool"/> when releasing this component to it.
        /// Removes this text from the <see cref="Manager"/>, asserts that the <see cref="Component.gameObject"/> was
        /// active and deactivates it (unless overridden).
        /// </summary>
        public override void OnRelease()
        {
            if (Manager != null)
            {
                Manager.Remove(_Transform);
                Manager = null;
            }

            base.OnRelease();
        }

        /************************************************************************************************************************/
    }

    /// <summary>Extension methods for <see cref="FloatingText"/>.</summary>
    public static class FloatingTextExtensions
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Shorthand for calling <see cref="ObjectPool{T}.Acquire"/> followed by
        /// <see cref="FloatingText.Show(string, Vector3)"/>.
        /// </summary>
        public static T Show<T>(this ObjectPool<T> pool, string text, Vector3 position)
            where T : FloatingText
        {
            var floatingText = pool.Acquire();
            floatingText.Show(text, position);
            return floatingText;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Shorthand for calling <see cref="ObjectPool{T}.Acquire"/> followed by
        /// <see cref="FloatingText.Show(string, Vector3, BillboardManager)"/>.
        /// </summary>
        public static T Show<T>(this ObjectPool<T> pool, string text, Vector3 position, BillboardManager manager)
            where T : FloatingText
        {
            var floatingText = pool.Acquire();
            floatingText.Show(text, position, manager);
            return floatingText;
        }

        /************************************************************************************************************************/
    }
}
