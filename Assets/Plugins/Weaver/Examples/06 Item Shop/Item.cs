// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>Details about an <see cref="Item"/>.</summary>
    [Serializable]
    public struct ItemMetaData
    {
        /************************************************************************************************************************/

        [SerializeField]
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [SerializeField]
        private Texture2D _Icon;
        public Texture2D Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        [SerializeField]
        private int _Cost;
        public int Cost
        {
            get { return _Cost; }
            set { _Cost = value; }
        }

        [SerializeField]
        private float _Weight;
        public float Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        [SerializeField]
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /************************************************************************************************************************/
    }

    /// <summary>
    /// An item that can be purchased from a shop.
    /// <para></para>
    /// This class could contain any kind of functionality, but for the purpose of this simple example it just holds
    /// the meta-data.
    /// </summary>
    public sealed class Item : MonoBehaviour, IMetaDataProvider<ItemMetaData>
    {
        /************************************************************************************************************************/

        [SerializeField]
        private ItemMetaData _MetaData;

        /// <summary>This item's details.</summary>
        public ItemMetaData MetaData
        {
            get { return _MetaData; }
            set { _MetaData = value; }
        }

        /************************************************************************************************************************/
    }
}
