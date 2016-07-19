namespace Hitboxes
{
    using UnityEngine;
    using System.Collections.Generic;
    using Observer_Observable;

    /// <summary>
    /// This class needs to be attached to a GameObject with a collider component.
    /// It keeps a dictionary that contains all colliders, that are touching the GameObject's collider, as keys.
    /// The value that is assigned to a collider, that is a key in the dictionary, is a HashSet of Vector2s.
    /// These are the normalvectors of the collider's surface at the points where they are colliding with the GameObject's collider.
    /// </summary>
    /// 
    /// <author>Nikolai Elich</author>
    /// <version>20.06.2016</version>
    public class HitboxStateChecker : ObservableService
    {
        private Dictionary<Collider2D, HashSet<Vector2>> _touchingColliders;    // The Dictionary containing the colliders and their assigned HashSets of normalvectors.

        void OnEnable()
        {
            _touchingColliders = new Dictionary<Collider2D, HashSet<Vector2>>();
        }

        public HashSet<Vector2> getTouchingNormalvectors()
        {
            HashSet<Vector2> normalVectorSet = new HashSet<Vector2>();

            foreach (KeyValuePair<Collider2D, HashSet<Vector2>> colliderVectorSetPair in _touchingColliders)
            {
                normalVectorSet.UnionWith(colliderVectorSetPair.Value);
            }

            return normalVectorSet;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            addCollision(collision);
        }
        
        void OnCollisionStay2D(Collision2D collision)
        {
            addCollision(collision);
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            removeCollider(collision.collider);
        }

        /// <summary>
        /// Adds the relevant information of an occurring collision to the Dictionary.
        /// In case the collider is not yet contained in the dictionary, the normalvectors from the collision are added to the dictionary.
        /// If the collider was already contained in the dictionary, and the normalvectors from the collision are different to the ones that are already assigned to this collider,
        /// the normalvectors of the collision are assigned as the value of the collider.
        /// Observers are only informed if some or all of the normalvectors from the collision were not already included in the dictionary.
        /// </summary>
        /// <param name="collision">The currently occuring collision.</param>
        private void addCollision(Collision2D collision)
        {
            HashSet<Vector2> newNormalVectors = getNormalVectorHashSet(collision);
            HashSet<Vector2> oldNormalVectors;
            bool newVectorsAdded = containsNotIncludedVectors(newNormalVectors);

            if (!_touchingColliders.TryGetValue(collision.collider, out oldNormalVectors))
            {
                addTouchingCollider(collision);
            }
            else if (!newNormalVectors.SetEquals(oldNormalVectors))
            {
                _touchingColliders[collision.collider] = newNormalVectors;
            }

            if (newVectorsAdded)
            {
                informOfChanges();
            }
        }

        /// <summary>
        /// Removes a collider from the dictionary.
        /// If all normalvectors that were associated with that collider are still included in the dictionary after their removal, the Observers are not informed.
        /// </summary>
        /// <param name="collider"></param>
        private void removeCollider(Collider2D collider)
        {
            HashSet<Vector2> oldNormalVectors;

            if (!_touchingColliders.TryGetValue(collider, out oldNormalVectors))
            {
                return;
            }

            _touchingColliders.Remove(collider);

            if (containsNotIncludedVectors(oldNormalVectors))
            {
                informOfChanges();
            }
        }

        private HashSet<Vector2> getNormalVectorHashSet(Collision2D collision)
        {
            HashSet<Vector2> normalVectors = new HashSet<Vector2>();
            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                normalVectors.Add(contactPoint.normal);
            }
            return normalVectors;
        }

        private void addTouchingCollider(Collision2D collision)
        {
            HashSet<Vector2> normalVectors = new HashSet<Vector2>();

            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                normalVectors.Add(contactPoint.normal);
            }

            _touchingColliders.Add(collision.collider, normalVectors);
        }
        
        /// <param name="vectors">A HashSet of vectors.</param>
        /// <returns>Wether the vectors contained in the HashSet are already included in the dictionary.</returns>
        private bool containsNotIncludedVectors(HashSet<Vector2> vectors)
        {
            return !getTouchingNormalvectors().IsSupersetOf(vectors);
        }
    }
}