using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerAnimation : PlayerBase
    {

        public SpriteRenderer spriterender;
        public Animator anim;
        public void Init(PlayerData playerdata)
        {
            this.playerdata = playerdata;
        }


        public void VelocityRender(PlayerMove.JumpState state ,  Vector2 velocity , float maxSpeed , bool isGround, bool isSlide) 
        {
            //if (move.x > 0.01f)
            //    spriterender.flipX = false;
            //else if (move.x < -0.01f)
            //    spriterender.flipX = true;

            anim.SetBool("grounded", isGround);
            anim.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        }
        public void OnDead() 
        {
            anim.SetBool("grounded", true);
            anim.SetFloat("velocityX", 0.0f );
        }

    }
}



