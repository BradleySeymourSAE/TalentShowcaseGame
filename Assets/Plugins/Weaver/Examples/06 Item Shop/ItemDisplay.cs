// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;
using UnityEngine.UI;

namespace Weaver.Examples
{
    /// <summary>
    /// A group of UI components which display the details of an <see cref="Item"/>.
    /// </summary>
    public sealed class ItemDisplay : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private RectTransform _Transform;

        /// <summary>The <see cref="RectTransform"/> of this object.</summary>
        public RectTransform Transform
        {
            get { return _Transform; }
            set { _Transform = value; }
        }

        [SerializeField]
        private Text _Name;

        /// <summary>A <see cref="Text"/> component used to display the item's name.</summary>
        public Text Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [SerializeField]
        private Text _IsLoaded;

        /// <summary>A <see cref="Text"/> component used to indicate whether the item is currently loaded.</summary>
        public Text IsLoaded
        {
            get { return _IsLoaded; }
            set { _IsLoaded = value; }
        }

        [SerializeField]
        private RawImage _Icon;

        /// <summary>A <see cref="RawImage"/> component used to display the item's icon.</summary>
        public RawImage Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        [SerializeField]
        private Text _Cost;

        /// <summary>A <see cref="Text"/> component used to display the item's cost.</summary>
        public Text Cost
        {
            get { return _Cost; }
            set { _Cost = value; }
        }

        [SerializeField]
        private Text _Weight;

        /// <summary>A <see cref="Text"/> component used to display the item's weight.</summary>
        public Text Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        [SerializeField]
        private Text _Description;

        /// <summary>A <see cref="Text"/> component used to display the item's description.</summary>
        public Text Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /************************************************************************************************************************/

        /// <summary>The list containing the target item to display the details of.</summary>
        private ItemList _Items;

        /// <summary>The position of the target item in the <see cref="_Items"/> list.</summary>
        private int _Index;

        /************************************************************************************************************************/

        /// <summary>
        /// Initializes this display to show the details of the item at the specified `index`.
        /// </summary>
        public void SetItem(ItemList items, int index)
        {
            _Items = items;
            _Index = index;

            var metaData = items.GetMetaData(index);

            name = _Name.text = metaData.Name;
            _Icon.texture = metaData.Icon;
            _Cost.text = "Cost: " + metaData.Cost;
            _Weight.text = "Weight: " + metaData.Weight;
            _Description.text = metaData.Description;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called when this display is clicked on.
        /// Ensures that the target item is loaded and spawns an instance of it in the scene.
        /// </summary>
        public void OnClick()
        {
            var position = Random.onUnitSphere * 3;

            // Get the target item prefab.
            // Doing this will load the prefab if it wasn't loaded already.
            var prefab = _Items[_Index];// Alternatively _Items.GetAsset.

            var item = Instantiate(prefab, position, Random.rotationUniform);
            Debug.Log("Spawned " + item.name + " at " + position, item);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the text indicating whether the target item is loaded or not.
        /// <para></para>
        /// In this example the only thing that will actually cause the item to be loaded is the <see cref="OnClick"/>
        /// method.
        /// <para></para>
        /// Also note that everything will always be loaded while in the Unity Editor because it needs to ensure that
        /// the list contains the correct references and data for the assets currently in the target directory.
        /// </summary>
        private void Update()
        {
            _IsLoaded.text = _Items.GetIsLoaded(_Index) ? "Is Loaded" : "Not Loaded";
        }

        /************************************************************************************************************************/
    }
}
