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
        public int _movementSpeed;          // The Player's movement speed.
        public int _jumpingPower;           // The Player's jumping Power.
        public int _wallDropSpeed;          // How fast the player drops when pressing against a wall.

        private bool _jumping;              // Wether the player is in mid-jump.
        private bool _jumpPressed;          // Becomes true when the player presses the Jump-Button, and false again, when the jump is started in the next FixedUpdate().

        private Rigidbody2D _rb2D;          // The Player GameObject's Rigidbody.

        private RectangleHitbox _hitbox;    // The player's hitbox.

        /// <summary>
        /// Initializes the fields. The public fields are to be initialized in the Inspector.
        /// </summary>
        void Start()
        {
            _jumping = false;
            _jumpPressed = false;

            _rb2D = GetComponentInParent<Rigidbody2D>();
            _hitbox = _rb2D.GetComponentInChildren<RectangleHitbox>();
        }

        void Update()
        {
            checkIfStillJumping();
            setJumpInput();
        }

        private void checkIfStillJumping()
        {
            if (_jumping)
            {
                _jumping = !(_hitbox._touchingFloor || _hitbox.touchingWall());
            }
        }

        private void setJumpInput()
        {
            if (!(_jumping || _jumpPressed))
            {
                _jumpPressed = Input.GetButtonDown("Jump") && (_hitbox._touchingFloor || _hitbox.touchingWall());   //Walljumping is enabled.
            }
        }

        void FixedUpdate()
        {
            setVelocity();
        }

        private void setVelocity()
        {
            if (_jumpPressed)
            {
                Jump();
            }
            setHorizontalVelocity();
        }

        private void setHorizontalVelocity()
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

        private void Jump()
        {
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0);
            _rb2D.AddForce(new Vector2(0, _jumpingPower));
            _jumpPressed = false;
            StartCoroutine("stopJump");
        }

        /// <summary>
        /// Cuts the player's jump short, when he releases the "Jump" button while still ascending.
        /// </summary>
        private IEnumerator stopJump()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            _jumping = true;
            yield return new WaitWhile(() => { return _jumping && (Input.GetButton("Jump") || _rb2D.velocity.y <= 0); } );
            yield return new WaitForFixedUpdate();

            if (_rb2D.velocity.y > 0 && _jumping)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0);
            }
        }
    }
}