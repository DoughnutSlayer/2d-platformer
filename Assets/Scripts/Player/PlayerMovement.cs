namespace Player
{
    using UnityEngine;
    using Hitboxes;
    using System.Collections;

    /// <summary>
    /// Takes the player's input and makes the player object move accordingly.
    /// </summary>
    /// 
    /// <author>Nikolai Elich</author>
    /// <version>19.06.2016</version>
    public class PlayerMovement : MonoBehaviour
    {
        public int _movementSpeed; // The Player's movement speed.
        public int _jumpingPower; // The Player's jumping Power.
        public int _inputBuffer; // The number of fixed Updates for which the jump input will be buffered. Intentionally does not work for walljumps.
        public int _wallDropSpeed; // How fast the player drops when pressing against a wall.
        public int _jumpStopForceMultiplier; // Determines how strong the force is, that cuts a jump short.

        private Rigidbody2D _rb2D; // The Player GameObject's Rigidbody.

        private RectangleHitbox _hitbox; // The player's hitbox.

        /// <summary>
        /// Initializes the fields. The public fields are to be initialized in the Inspector.
        /// </summary>
        void Start()
        {
            _rb2D = GetComponentInParent<Rigidbody2D>();
            _hitbox = _rb2D.GetComponentInChildren<RectangleHitbox>();
        }

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine("attemptJump");
            }
        }

        void FixedUpdate()
        {
            setVelocity();
        }

        private void setVelocity()
        {
            int horizontalInput = (int)Input.GetAxisRaw("Horizontal");

            if ((_hitbox._touchingRight && horizontalInput > 0) || (_hitbox._touchingLeft && horizontalInput < 0))
            {
                setWallVelocity(horizontalInput);
            }
            else
            {
                _rb2D.velocity = new Vector2(horizontalInput * _movementSpeed, _rb2D.velocity.y);
            }
        }

        /// <summary>
        /// Makes the player fall more slowly when pressing against a wall.
        /// </summary>
        private void setWallVelocity(int horizontalInput)
        {
            if (_rb2D.velocity.y <= -_wallDropSpeed)
            {
                _rb2D.velocity = new Vector2(0, -_wallDropSpeed);
            }
        }

        private IEnumerator attemptJump()
        {
            if (_hitbox._touchingFloor || _hitbox.touchingWall())
            {
                jump();
                yield break;
            }
            for (int i = 0; i < _inputBuffer; i++)
            {
                if (_hitbox._touchingFloor)
                {
                    jump();
                    yield break;
                }
                if (i == _inputBuffer - 1)
                {
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void jump()
        {
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0);
            _rb2D.AddForce(new Vector2(0, _jumpingPower));
            StartCoroutine("stopJump");
        }

        /// <summary>
        /// Cuts the player's jump short, when he releases the "Jump" button while still ascending.
        /// </summary>
        private IEnumerator stopJump()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitWhile(() => { return Input.GetButton("Jump") && _rb2D.velocity.y > 0; });
            yield return new WaitForFixedUpdate();

            if (_rb2D.velocity.y > 0)
            {
                _rb2D.AddForce(new Vector2(0, -_rb2D.velocity.y * _jumpStopForceMultiplier));
            }
        }
    }
}