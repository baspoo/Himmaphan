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
        //public BoxCollider2D collider2d;
        /*internal new*/
        //public AudioSource audioSource;
        public bool controlEnabled = true;

        [SerializeField] bool jump;
        [SerializeField] bool holdslide;
        [SerializeField] Vector2 move;

        [SerializeField] StateModify stateModify;
        [System.Serializable]
        class StateModify 
        {
            public State jump;
            public State ground;
            public State slide;

            [System.Serializable]
            public class State
            {
                public JumpState state;
                public Vector2 colliderOffset;
                public Vector2 colliderSize;
                public void OnModifyCollider(BoxCollider2D collider) {
                    collider.offset = colliderOffset;
                    collider.size = colliderSize;
                }
            }
        }


        //SpriteRenderer spriteRenderer;
        //internal Animator animator;

        //public Bounds Bounds => collider2d.bounds;
        PlayerData playerdata;
        public void Init(PlayerData playerdata)
        {
            this.playerdata = playerdata;
            //audioSource = GetComponent<AudioSource>();
            //collider2d = GetComponent<BoxCollider2D>();
            //spriteRenderer = GetComponent<SpriteRenderer>();
            //animator = GetComponent<Animator>();
            OnRun();
        }

        public bool IsCanJump
        {
            get {


                if (jumpStep == 0 && jumpState == JumpState.Grounded)
                    return true;
                else if (jumpStep > 0 && jumpStep < maxJumpStep && jumpState == JumpState.InFlight)
                    return true;

                return false;
            }
        
        }



        public void OnJump()
        {
            if (IsCanJump)
            {
                jumpState = JumpState.PrepareToJump;
            }
        }
        public void OnSlide()
        {
            if (jumpState == JumpState.Grounded)
            {
                holdslide = true;
            }
        }
        public void OnStopSlide()
        {
            holdslide = false;
        }

        public void OnRun() 
        {
            IsRun = true;
        }
        public void OnStopRun()
        {
            IsRun = false;
        }


        public bool IsRun;

        protected override void Update()
        {
            if (Input.GetButtonDown("Jump"))
                OnJump();
            if (Input.GetButtonDown("Jump"))
                OnJump();

            if (controlEnabled)
            {
                if (IsRun)
                {
                    move.x = 1.0f;
                }
                else
                {
                    move.x = 0.0f;
                }
            }
            else
            {
                //move.x = 0;
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
                    jumpState = holdslide? JumpState.SlideGround : JumpState.Grounded;
                    jumpStep = 0;
                    break;
                case JumpState.Grounded:
                case JumpState.SlideGround:
                    jumpState = holdslide ? JumpState.SlideGround : JumpState.Grounded;
                    if (velocity.y != 0) jumpState = JumpState.InFlight;
                    break;
            }
            StateHandle(jumpState);
        }




        void StateHandle(JumpState state)
        {
            switch (jumpState)
            {
                case JumpState.InFlight:
                    if(velocity.y >= 0) stateModify.jump.OnModifyCollider(playerdata.handle.collider);
                    else stateModify.ground.OnModifyCollider(playerdata.handle.collider);
                    break;
                case JumpState.Grounded:
                    stateModify.ground.OnModifyCollider(playerdata.handle.collider);
                    break;
                case JumpState.SlideGround:
                    stateModify.slide.OnModifyCollider(playerdata.handle.collider);
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
                    Debug.Log(velocity.y);
                }
            }

            playerdata.anim.VelocityRender( jumpState , velocity , maxSpeed, IsGrounded , holdslide );
            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            SlideGround,
            PrepareToJump,
            Jumping,
            InFlight,
            Flying,
            Landed
        }
    }

}