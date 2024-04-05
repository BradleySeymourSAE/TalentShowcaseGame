// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /************************************************************************************************************************/

    /// <summary>An object that can deal damage and be notified when it does so.</summary>
    public interface IDealDamage
    {
        void OnDealDamage(ITakeDamage target, int pointValue);
    }

    /************************************************************************************************************************/

    /// <summary>An object that can be damaged.</summary>
    public interface ITakeDamage
    {
        /// <summary>The <see cref="Transform"/> of this object.</summary>
        Transform transform { get; }

        /// <summary>Indicates whether this object will increase the score of whoever destroys it.</summary>
        bool IsWorthPoints { get; }

        /// <summary>Notifies this object that it has been damaged.</summary>
        void OnTakeDamage(IDealDamage source, int pointValue);
    }

    /************************************************************************************************************************/

    /// <summary>
    /// A set of simple methods which use <see cref="IDealDamage"/> and <see cref="ITakeDamage"/> to pass simple
    /// damage messages around.
    /// </summary>
    public static class DamageSystem
    {
        /************************************************************************************************************************/

        public static bool DealDamage(IDealDamage source, ITakeDamage target, int pointValue)
        {
            if (target != null)
            {
                target.OnTakeDamage(source, pointValue);

                if (source != null)
                    source.OnDealDamage(target, pointValue);

                return true;
            }
            else return false;
        }

        /************************************************************************************************************************/

        public static bool DealDamage(IDealDamage source, GameObject target, int pointValue)
            => DealDamage(source, target.GetComponentInParent<ITakeDamage>(), pointValue);

        /************************************************************************************************************************/

        public static bool DealDamage(IDealDamage source, Component target, int pointValue)
            => DealDamage(source, target.gameObject, pointValue);

        /************************************************************************************************************************/
    }
}
