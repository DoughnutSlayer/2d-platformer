namespace Hitboxes
{
    using UnityEngine;
    using System.Collections.Generic;
    using Observer_Observable;

    /// <summary>
    /// Keeps an updated account of what sides of a collider are currently touching other colliders.
    /// It uses a HitboxStateChecker to accomplish this. All colliders are being observed, which are attached to the same GameObject as the HitboxStateChecker.
    /// The sum of these colliders is considered to be the hitbox.
    /// </summary>
    /// 
    /// <author>Nikolai Elich</author>
    /// <version>19.06.2016</version>
    public class RectangleHitbox : MonoBehaviour, ServiceObserver
    {
        public bool _touchingCeiling;                       // Wether the hitbox's top is touching another collider.
        public bool _touchingRight;                         // Wether the hitbox's right side is touching another collider.
        public bool _touchingFloor;                         // Wether the hitbox's bottom is touching another collider.
        public bool _touchingLeft;                          // Wether the hitbox's left side is touching another collider.

        public float _maxFloorSlope = 1;                   // The maximum slope a surface can have and still be considered as a floor. A value of 1 Equals 45 degrees.

        private HashSet<Vector2> _touchingNormalvectors;    // A HashSet containing all different normalvectors of the surfaces the hitbox is touching.

        private HitboxStateChecker _stateChecker;           // This object constantly checks what other colliders the hitbox is touching.

        /// <summary>
        /// Initializes the fields and registers this object as an observer of the HitboxStateChecker.
        /// The maximum floor slope is to be initialized in the Inspector.
        /// </summary>
        void Start()
        {
            _touchingCeiling = false;
            _touchingRight = false;
            _touchingFloor = false;
            _touchingLeft = false;

            _touchingNormalvectors = new HashSet<Vector2>();
            _stateChecker = gameObject.AddComponent<HitboxStateChecker>();
            _stateChecker.registerObserver(this);
        }

        public bool touchingWall()
        {
            return _touchingLeft || _touchingRight;
        }

        /// <summary>
        /// Gets an IEnumerable from the HitboxStateChecker and checks if it contains a different set of vectors than this object's _touchingNormalvectors.
        /// In this case the information on which sides are being touched is updated.
        /// </summary>
        /// <param name="observedObject">The object that called this method. The method returns immediately if this is not the _stateChecker.</param>
        public void reactToChange(ObservableService observedObject)
        {
            if(observedObject != _stateChecker)
            {
                return;
            }

            IEnumerable<Vector2> normalVectors = _stateChecker.getTouchingNormalvectors();

            if (!_touchingNormalvectors.SetEquals(normalVectors))
            {
                updateTouchingSides( normalVectors);
            }
        }

        private void updateTouchingSides(IEnumerable<Vector2> normalVectors)
        {
            HashSet<Vector2> newNormalvectors = new HashSet<Vector2>();

            foreach (Vector2 normalvector in normalVectors)
            {
                newNormalvectors.Add(normalvector);
            }
            _touchingNormalvectors = newNormalvectors;

            _touchingCeiling = _touchingNormalvectors.Contains(Vector2.down);
            _touchingRight = _touchingNormalvectors.Contains(Vector2.left);
            _touchingFloor = touchingFloor();
            _touchingLeft = _touchingNormalvectors.Contains(Vector2.right);
        }

        private bool touchingFloor()
        {
            foreach (Vector2 vector in _touchingNormalvectors)
            {
                if (vector.y != 0 && Mathf.Abs(vector.x/vector.y) <= _maxFloorSlope)
                {
                    return true;
                }
            }
            return false;
        }
    }
}