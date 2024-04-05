using System.Collections.Generic;
using UnityEngine;
namespace Framework.Scripts.Common
{
    [CreateAssetMenu(fileName = "Readme", menuName = "Framework/Tools/Readme")]
    public class Readme : ScriptableObject
    {
        public Texture2D icon;
        public float iconMaxWidth = 128f;
        public string title;
        public List<Section> sections = new List<Section>();

        [System.Serializable]
        public class Section
        {
            public string heading;
            public string text;
            public string linkText;
            public string url;

            public Texture2D Image;
            [Range(64.0f,512.0f)] public float ImageWidth = 512.0f;
            [Range(64.0f,512.0f)] public float ImageHeight = 512.0f;
        }

        [System.Serializable]
        public class LinkReference
        {
            public string Link;
            public string Url; 
        }
    }
}
