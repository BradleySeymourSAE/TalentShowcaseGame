// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A <see cref="FloatingText"/> that uses Unity's inbuilt <see cref="UnityEngine.TextMesh"/> component.
    /// </summary>
    public sealed class FloatingTextUnity : FloatingText
    {
        /************************************************************************************************************************/

        /// <summary>The component used to display the actual text.</summary>
        [SerializeField]
        private TextMesh _TextMesh;

        /// <summary>The component used to display the actual text.</summary>
        public TextMesh TextMesh => _TextMesh;

        /// <summary>The text string currently being shown.</summary>
        public override string Text { get => _TextMesh.text; set => _TextMesh.text = value; }

        /// <summary>The height of the text in world-space.</summary>
        public override float Size { get => _TextMesh.characterSize; set => _TextMesh.characterSize = value; }

        /// <summary>The <see cref="UnityEngine.Color"/> of the text.</summary>
        public override Color Color { get => _TextMesh.color; set => _TextMesh.color = value; }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component is first added (in Edit Mode) or the "Reset" function is used.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _TextMesh = gameObject.GetOrAddComponent<TextMesh>();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a simple floating text in front of the camera for a short time (generally for debugging purposes).
        /// </summary>
        public static FloatingText Show(object text)
        {
            if (text == null)
                return null;

            var str = text.ToString();

            var cameraTransform = Camera.main.transform;
            var position = cameraTransform.position + cameraTransform.forward * 5 + Random.insideUnitSphere * 3;

            var floatingText = new GameObject(str).AddComponent<FloatingTextUnity>();
            floatingText.InitializeDefaults();

            floatingText.Show(str, position);
            floatingText.Transform.SetParent(cameraTransform, true);
            Destroy(floatingText.gameObject, 3);
            return floatingText;
        }

        /************************************************************************************************************************/

        public void InitializeDefaults()
        {
            Reset();

            var renderer = gameObject.GetOrAddComponent<MeshRenderer>();

            _TextMesh.alignment = TextAlignment.Center;
            _TextMesh.anchor = TextAnchor.MiddleCenter;
            if (_TextMesh.font == null)
                _TextMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            renderer.sharedMaterial = _TextMesh.font.material;
        }

        /************************************************************************************************************************/
    }
}
