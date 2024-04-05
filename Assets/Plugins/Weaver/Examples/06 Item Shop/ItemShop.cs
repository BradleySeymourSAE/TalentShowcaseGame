// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Initializes an <see cref="ItemDisplay"/> for each item in an <see cref="ItemList"/>.
    /// </summary>
    public sealed class ItemShop : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private ItemList _Items;

        /// <summary>The list to get items from.</summary>
        public ItemList Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        [SerializeField]
        private ItemDisplay _DisplayTemplate;

        /// <summary>The default <see cref="ItemDisplay"/> to clone for each item.</summary>
        public ItemDisplay DisplayTemplate
        {
            get { return _DisplayTemplate; }
            set { _DisplayTemplate = value; }
        }

        [SerializeField]
        private float _ItemSpacing;

        /// <summary>The amount of space to put between each <see cref="ItemDisplay"/>.</summary>
        public float ItemSpacing
        {
            get { return _ItemSpacing; }
            set { _ItemSpacing = value; }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity the first time this component becomes enabled and active.
        /// Instantiates a copy of the <see cref="_DisplayTemplate"/> for each item in the <see cref="_Items"/> list.
        /// </summary>
        private void Awake()
        {
            var parent = _DisplayTemplate.Transform.parent;
            var itemHeight = -(_DisplayTemplate.Transform.rect.height + _ItemSpacing);

            for (int i = 0; i < _Items.Count; i++)
            {
                var display = Instantiate(_DisplayTemplate, parent);
                display.Transform.anchoredPosition = new Vector2(0, i * itemHeight);
                display.SetItem(_Items, i);
                display.gameObject.SetActive(true);
            }
        }

        /************************************************************************************************************************/
    }
}
