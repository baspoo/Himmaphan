using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Interactive.Player.Avatar
{
    public class AvatarAnim : MonoBehaviour
    {

        public enum Acting
        {
            None,
            IdleAction,
            Sit, SitObj, Jump,   // <--Unloop
            Dance_1, Dance_2, Dance_3, Dance_4,
            Watering, Pickup, Roll, FreeKick,
            Hello, Gesture, Cheer, Hey, Talk
        }


        public Animator anim;
        private static readonly int Standing = Animator.StringToHash("IsStanding");
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int SubIdle = Animator.StringToHash("SubIdle");
        private static readonly int isCarry = Animator.StringToHash("isCarry");
        private static readonly int IsJump = Animator.StringToHash("isJump");

        public pMovement.CharacterState currentstate { get; private set; }
        bool IsActing;



        AvatarObj avatar;
        AnimatorOverrideController animatorOverrideController;
        public void Init(AvatarObj avatar)
        {
            this.avatar = avatar;
            animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
            anim.runtimeAnimatorController = animatorOverrideController;
        }


        public void StartAnimation( pMovement.CharacterState state )
        {
            //Debug.Log("StartAnimation:" + state);
            if (anim == null) return;
           
            ResetStateAnimations();
            switch (state)
            {
                case pMovement.CharacterState.Idle:
                    break;
                case pMovement.CharacterState.Moving:
                    anim.SetBool(IsMoving, true);
                    break;
                case pMovement.CharacterState.Standing:
                    anim.SetBool(Standing, true);
                    break;
                case pMovement.CharacterState.Stunned:
                    anim.SetBool(IsStunned, true);
                    break;
                case pMovement.CharacterState.Jump:
                    OnJump(0);
                    break;
                case pMovement.CharacterState.Sit:
                    OnActing(Acting.SitObj, true);
                    break;
            }
            currentstate = state;
        }
        public void OnActing(Acting acting , bool loop)
        {
            if (anim.GetBool(IsMoving))
                return;
            DoActing(acting, loop);
        }

        string m_animUniq = string.Empty;
        public void OnUpdateAnim(string animUniq , float value)
        {
            if (animUniq.notnull())
            {
                var split = animUniq.Split('-');
                var animname = split[0];
                var loop = value == -1;
                var canplay = m_animUniq != animUniq;

                //Debug.Log($"Network-OnUpdateAnim : DoActing [{animUniq} {m_animUniq}] [canplay:{canplay}] [loop:{loop}]");
                if (canplay || loop)
                {
                    DoActing(animname, loop);
                    m_animUniq = animUniq;
                }
            }
            else 
            {
                ResetStateAnimations();
            }
        }









        void ResetStateAnimations()
        {
            if (IsActing)
                StopAnim();
            anim.SetBool(IsMoving, false);
            anim.SetBool(Standing, false);
            anim.SetBool(IsStunned, false);
            anim.SetBool(IsJump, false);
        }
        void LateUpdate()
        {
            if (currentstate == pMovement.CharacterState.Idle) 
            {
                IdelAnim();
            }
        }
        void StopAnim()
        {
            IsActing = false;
            anim.Play("Idle", -1, 0.0f);
        }
        void PlayAnim(string str)
        {
            anim.Play(str, -1, 0.0f);
        }


        public AnimationClips animationClips;
        AnimationClip mCurrentClip;
        [System.Serializable]
        public class AnimationClips
        {
            public List<AnimationClip> clip_subIdle;
            public List<AnimationClip> clip_acting;
        }
        void DoActing( Acting acting, bool loop) => DoActing(acting.ToString(), loop);
        void DoActing(string actName, bool loop)
        {
            var actClip = animationClips.clip_acting.Find(x=>x.name == actName);
            if (actClip!=null && !loop) IsActing = false;
            DoActing(actClip, loop);
        }
        void DoActing(AnimationClip actClip, bool loop) 
        {
            if (actClip != null)
            {
                if (IsActing) 
                {
                    if (mCurrentClip == actClip)
                        return;
                }
                IsActing = true;
                string state = loop ? "ActingLoop" : "Acting";
                animatorOverrideController[state] = actClip;
                mCurrentClip = actClip;
                PlayAnim(state);
            }
        }





        public enum JumpState { Jump, JumpHold ,JumpAir, JumpGround }
        public void OnJump(JumpState state)
        {
            if (state == JumpState.Jump)
            {
                anim.SetBool(IsJump, false);
                PlayAnim("Jump");
            }
            if (state == JumpState.JumpHold)
            {
                anim.SetBool(IsJump, true);
                PlayAnim("Jump");
            }
            if (state == JumpState.JumpAir) 
            {
                anim.SetBool(IsJump, true);
                PlayAnim("JumpAir");
            }
            if (state == JumpState.JumpGround)
            {
                anim.SetBool(IsJump, false);
                PlayAnim("JumpGround");
            }
        }
        public void StopJump( )
        {
            IsActing = false;
            anim.SetBool(IsJump, false);
            ResetStateAnimations();
        }












        #region Idle
        public bool isIdelAnim => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        public IdleHandle idleHandle;
        [System.Serializable]
        public class IdleHandle
        {
            public bool IgnoreSubIdle;
            public float IdelAnimRuntime = 0.0f;
            public float IdelAnimMaxtime = 5.0f;
            public float[] Ideltimes = new float[2] { 3f, 7.0f };
        }
        void IdelAnim()
        {
            if (idleHandle.IgnoreSubIdle)
            {
                idleHandle.IdelAnimRuntime = 0.0f;
                return;
            }
            if (isIdelAnim)
            {
                idleHandle.IdelAnimRuntime += Time.deltaTime;
            }
            else
                idleHandle.IdelAnimRuntime = 0.0f;


            if (idleHandle.IdelAnimRuntime >= idleHandle.IdelAnimMaxtime)
            {
                idleHandle.IdelAnimRuntime = 0.0f;
                idleHandle.IdelAnimMaxtime = Random.Range(idleHandle.Ideltimes[0], idleHandle.Ideltimes[1]);
                StartCoroutine(StartSupIdle());
            }
        }
        IEnumerator StartSupIdle( ) 
        {
            IsActing = true;
            int clipIndex = Random.Range(0, animationClips.clip_subIdle.Count);
            anim.SetInteger(SubIdle, clipIndex);
            yield return new WaitForSeconds(0.05f);
            anim.SetInteger(SubIdle, 0);
        }
        #endregion

    }






























#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AvatarAnim))]
    public class UIAvatarAnim : Editor
    {
        AvatarAnim.Acting Acting;
        AvatarAnim service => (AvatarAnim)target;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            Acting = (AvatarAnim.Acting)EditorGUILayout.EnumPopup("ActingType", Acting);
            if (GUILayout.Button("OnActing"))
            {
                service.OnActing(Acting , false);
            }
            if (GUILayout.Button("OnActingLoop"))
            {
                service.OnActing(Acting , true);
            }
            if (GUILayout.Button("Jump"))
            {
                service.OnJump(AvatarAnim.JumpState.Jump);
            }
            if (GUILayout.Button("JumpHold"))
            {
                service.OnJump(AvatarAnim.JumpState.JumpHold);
            }
            if (GUILayout.Button("StopJump"))
            {
                service.StopJump();
            }
        }
    }
#endif


}






