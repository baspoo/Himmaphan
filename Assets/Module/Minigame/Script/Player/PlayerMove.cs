using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.Player
{
    public class PlayerMove : KinematicObject
    {
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
        public int maxJumpStep => jumpTakeOffSpeeds.Length;
        public float[] jumpTakeOffSpeeds = new float[2] { 4.0f, 6.0f };
        public float jumpModifier = 1f;
        public float jumpDeceleration = 1f;

        [SerializeField] JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/
        public Collider2D collider2d;
        /*internal new*/
        public AudioSource audioSource;
        public bool controlEnabled = true;

        [SerializeField] bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if ((jumpStep < maxJumpStep) && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    //stopJump = true;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }


        [SerializeField] int jumpStep = 0;
        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpStep++;
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    jumpStep = 0;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            //Debug.Log(jump);
            //Debug.Log(jumpSpte);

            if (jump && jumpStep <= maxJumpStep)
            {
                var hight = jumpTakeOffSpeeds[jumpStep - 1];
                velocity.y = hight * jumpModifier;
                Debug.Log(velocity.y);
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * jumpDeceleration;
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

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Flying,
            Landed
        }
    }

}