using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.Player
{
    public class PlayerMove : KinematicObject
    {
        //public AudioClip jumpAudio;
        //public AudioClip respawnAudio;
        //public AudioClip ouchAudio;
        public float maxSpeed = 7;
        public int maxJumpStep => jumpTakeOffSpeeds.Length;
        public float[] jumpTakeOffSpeeds = new float[2] { 4.0f, 6.0f };
        [SerializeField] float jumpModifier = 1f;
        [SerializeField] float jumpDeceleration = 1f;
        [SerializeField] int jumpStep = 0;
        [SerializeField] JumpState jumpState = JumpState.Grounded;
        bool stopJump;
        bool IsRun;
        bool jump;
        bool holdslide;
        float holdslideTime;
        Vector2 move;

        [SerializeField] StateModify stateModify;
        [System.Serializable]
        class StateModify 
        {
            public State ground;
            public State jump;
            public State slide;

            [System.Serializable]
            public class State
            {
                public JumpState state;
                public Vector2 colliderOffset;
                public Vector2 colliderSize;
                public void OnModifyCollider(CapsuleCollider2D collider) {
                    collider.offset = colliderOffset;
                    collider.size = colliderSize;
                }
            }
        }


        #if UNITY_EDITOR
        public RuntimeBtn SaveGround = new RuntimeBtn((r) => {
            PlayerData player = r.Gameobject.GetComponent<PlayerData>();
            player.move.saveload(player.move.stateModify.ground , player.handle.collider , (int) r.Double );
        });
        public RuntimeBtn SaveJump = new RuntimeBtn((r) => {
            PlayerData player = r.Gameobject.GetComponent<PlayerData>();
            player.move.saveload(player.move.stateModify.jump, player.handle.collider, (int)r.Double);
        });
        public RuntimeBtn SaveSlide= new RuntimeBtn((r) => {
            PlayerData player = r.Gameobject.GetComponent<PlayerData>();
            player.move.saveload(player.move.stateModify.slide, player.handle.collider, (int)r.Double);
        });
        void saveload(StateModify.State state , CapsuleCollider2D collider , int act)
        {
            if (act == 0)
            {
                state.colliderOffset = collider.offset;
                state.colliderSize = collider.size;
            }
            else 
            {
                collider.offset = state.colliderOffset;
                collider.size = state.colliderSize;
            }
        }
        #endif


        public bool IsCanJump
        {
            get
            {
                if (!isReay) return false;

                if (jumpStep == 0 && jumpState == JumpState.Grounded)
                    return true;
                else if (jumpStep > 0 && jumpStep < maxJumpStep && jumpState == JumpState.InFlight)
                    return true;

                return false;
            }
        }

        public bool IsFlying
        {
            get
            {
                if (jumpState == JumpState.InFlight)
                    return true;
                if (velocity.y != 0)
                    return true;
                return false;
            }
        }







        public void Init(PlayerData playerdata)
        {
            this.playerdata = playerdata;
            StateHandle(JumpState.Grounded);
        }


        



        public void OnJump()
        {
            if (!isReay) return;

            if (IsCanJump)
            {
                jumpState = JumpState.PrepareToJump;
            }
        }
        public void OnSlide()
        {
            if (!isReay) return;

            if (!IsFlying)
            {
                holdslide = true;
                holdslideTime = 0.0f;
            }
        }
        public void OnStopSlide()
        {
            //holdslide = false;
        }

        public void OnRun() 
        {
            if (!isReay) return;

            IsRun = true;
        }
        public void OnStopRun()
        {
            IsRun = false;
            enabled = false;
        }


      

        protected override void Update()
        {
            if (!isReay) return;

            if (IsRun)
            {
                move.x = 1.0f;
            }
            else
            {
                move.x = 0.0f;
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

            if (holdslide)
            {
                holdslideTime += Time.deltaTime;
                if (holdslideTime > 0.05f)
                    holdslide = false;
            }
            else holdslideTime = 0.0f;
        }












        protected override void ComputeVelocity()
        {
            if (!isReay) return;

            //Debug.Log(jump);
            //Debug.Log(jumpSpte);

            if (jump && jumpStep <= maxJumpStep)
            {
                var hight = jumpTakeOffSpeeds[jumpStep - 1];
                velocity.y = hight * jumpModifier;
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

            var _maxSpeed = this.maxSpeed * playerdata.stat.Speed;
            playerdata.anim.VelocityRender( jumpState , velocity , _maxSpeed, IsGrounded , holdslide );
            targetVelocity = move * _maxSpeed;
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