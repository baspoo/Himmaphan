using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerAnimation : PlayerBase
    {

        public SpriteRenderer spriterender;
        public Animator animGraphic;
        public Animation animRoot;


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

            animGraphic.SetBool("grounded", isGround);
            animGraphic.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            if(isSlide) spriterender.color = Color.white;
            else spriterender.color = Color.yellow;
        }
        public void OnDead(  ) 
        {
            animRoot.Play("playerdead");
            animGraphic.SetBool("grounded", true);
            animGraphic.SetFloat("velocityX", 0.0f );
        }

    }
}



