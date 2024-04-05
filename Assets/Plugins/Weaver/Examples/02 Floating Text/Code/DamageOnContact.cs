// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Shows a <see cref="FloatingText"/> as if damage were being dealt whenever a collision is detected.
    /// </summary>
    public sealed class DamageOnContact : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The type of damage to deal.</summary>
        public TextManager.DamageType type;

        /// <summary>The minimum amount of damage that can be dealt.</summary>
        public int min;

        /// <summary>The maximum amount of damage that can be dealt.</summary>
        public int max;

        /// <summary>The amount of force to give the colliding <see cref="Rigidbody"/>.</summary>
        public float force;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity whenever a collision is detected between this object and another.
        /// Applies a force to the colliding object's <see cref="Rigidbody"/> and shows a <see cref="FloatingText"/> as
        /// if damage were being dealt.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            var contact = collision.contacts[0];

            if (collision.rigidbody != null)
                collision.rigidbody.AddForce(contact.normal * -force, ForceMode.Impulse);

            var amount = Random.Range(min, max);
            TextManager.ShowDamageText(type, amount, contact.point);
        }

        /************************************************************************************************************************/
    }
}
