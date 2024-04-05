// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to.

using System;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Provides a central location for accessing various different kinds of <see cref="FloatingText"/>.
    /// </summary>
    public static class TextManager
    {
        /************************************************************************************************************************/

        /// <summary>
        /// In many games, damage types target different kinds of armour or have additional effects, but for this
        /// example the damage type just determines which kind of text is used to show the damage number.
        /// </summary>
        public enum DamageType
        {
            Physical,
            Fire,
            Cold,
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// <see cref="DamageType.Physical"/>.
        /// </summary>
        [AssetPool]
        private static readonly ObjectPool<FloatingText> PhysicalDamageText;

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// <see cref="DamageType.Fire"/>.
        /// </summary>
        [AssetPool]
        private static readonly ObjectPool<FloatingText> FireDamageText;

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// <see cref="DamageType.Cold"/>.
        /// </summary>
        [AssetPool]
        private static readonly ObjectPool<FloatingText> ColdDamageText;

        /// <summary>Shows a <see cref="FloatingText"/> damage of a particular type.</summary>
        public static void ShowDamageText(DamageType type, int amount, Vector3 position)
        {
            ObjectPool<FloatingText> pool;

            switch (type)
            {
                case DamageType.Physical: pool = PhysicalDamageText; break;
                case DamageType.Fire: pool = FireDamageText; break;
                case DamageType.Cold: pool = ColdDamageText; break;
                default: throw new ArgumentException("Unknown DamageType");
            }

            pool.Show(amount.ToString(), position);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// indicating status effects.
        /// </summary>
        [AssetPool]
        private static readonly ObjectPool<FloatingText> StatusText;

        /// <summary>Shows a <see cref="FloatingText"/> to indicate a status effect.</summary>
        public static void ShowStatusText(string text, Vector3 position, float duration)
        {
            var floatingText = StatusText.Show(text, position);
            floatingText.LifeTime = duration;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// conversations.
        /// </summary>
        [AssetPool]
        private static readonly ObjectPool<FloatingText> SpeechText;

        /// <summary>Shows a <see cref="FloatingText"/> for speech.</summary>
        public static FloatingText ShowSpeechText(string text, Vector3 position)
        {
            return SpeechText.Show(text, position);
        }

        /************************************************************************************************************************/
    }
}
