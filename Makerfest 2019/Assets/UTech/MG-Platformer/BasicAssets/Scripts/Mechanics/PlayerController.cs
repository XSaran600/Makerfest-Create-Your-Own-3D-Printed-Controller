using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public ArduinoInputs arduinoInput;

        // Sprint
        bool sprintPressed = false;

        // Jump
        bool jumpPressed = false;

        // Power Up
        bool powerUp = false;
        bool resetPowerUp;
        bool powerUpPos = false;

        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                
                // Move
                if (arduinoInput.GetHIDInput() == 2) // Move right
                {
                    move.x = 1;
                }
                else if (arduinoInput.GetHIDInput() == 3) // Move left
                {
                    move.x = -1;
                }
                else if (arduinoInput.GetHIDInput() == 1) // Stop movement
                {
                    move.x = 0;
                }
                if (arduinoInput.GetHIDInput() == 8) // Sprint
                {
                    sprintPressed = true;
                }
                else if(arduinoInput.GetHIDInput() == 7) // End Sprint
                {
                    sprintPressed = false;
                }


                // Jump
                if (arduinoInput.GetHIDInput() == 5) // Jump
                {
                    jumpPressed = true;
                }
                else if(arduinoInput.GetHIDInput() == 4) // End Jump
                {
                    jumpPressed = false;
                }
                /*
                if (arduinoInput.GetHIDInput() == 6) // Light Sensor Power Up
                {
                    StartCoroutine(PowerUpTimer());
                }
                */
                if (sprintPressed)
                {
                    maxSpeed = 7;
                }
                else
                {
                    maxSpeed = 3;
                }
                /*
                if (powerUp)
                {
                    transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                    if (!powerUpPos)
                    {
                        powerUpPos = true;
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                    }
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                */
                if (jumpState == JumpState.Grounded && jumpPressed)
                    jumpState = JumpState.PrepareToJump;
                else if (!jumpPressed)
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                /*

                if (Input.GetKeyDown(KeyCode.T))
                {
                    StartCoroutine(PowerUpTimer());
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    sprintPressed = true;
                }
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    sprintPressed = false;
                }

                if(sprintPressed)
                {
                    maxSpeed = 7;
                }
                else
                {
                    maxSpeed = 3;
                }

                if (powerUp)
                {
                    if (!powerUpPos)
                    {
                        powerUpPos = true;
                        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                        jumpTakeOffSpeed = 10;
                    }
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    jumpTakeOffSpeed = 7;
                }

                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }*/
                
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        private IEnumerator PowerUpTimer()
        {
            if (powerUp == false)
            {
                powerUp = true;
                float duration = 5f;

                float timeStamp = Time.time;

                while (Time.time < timeStamp + duration)
                {
                    if (resetPowerUp)
                    {
                        resetPowerUp = false;
                        timeStamp = Time.time;
                    }

                    yield return null;
                }
                powerUp = false;
                powerUpPos = false;
            }
            else
            {
                resetPowerUp = true;
            }
        }
        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}