using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive.Player
{
    public class pInterface : MonoBehaviour
    {

        public UILabel txtDisplayname;
        public UILabel txtMessage;
        public UILabel txtPermanentMessage;
        public float timeDisplayMessage;

        PlayerClient m_player;
        public void Init(PlayerClient player)
        {
            if (player == null) 
            {
                gameObject.SetActive(false);
                return;
            }
            m_player = player;
            txtDisplayname.text = m_player.PlayerData.NickName;
            Clean();
        }
        public void OnVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        public void Clean() 
        {
            //Clear Message
            if (coroMessage != null)
                StopCoroutine(coroMessage);

            txtDisplayname.text = string.Empty;
            txtMessage.gameObject.SetActive(false);
            OnPermanentMessage(null);
        }





        //**  Header Message
        Coroutine coroMessage;
        public void OnMessage(string message)
        {
            if (coroMessage != null)
                StopCoroutine(coroMessage);
            coroMessage = StartCoroutine(IEMessage(message));
        }
        IEnumerator IEMessage(string message) 
        {
            txtMessage.gameObject.SetActive(false);
            txtMessage.text = message;
            yield return new WaitForEndOfFrame( );
            txtMessage.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeDisplayMessage);
            txtMessage.gameObject.SetActive(false);
        }
        public void OnPermanentMessage(string message = null)
        {
            if (message.notnull())
            {
                txtPermanentMessage.text = message;
                txtPermanentMessage.gameObject.SetActive(true);
            }
            else 
            {
                txtPermanentMessage.gameObject.SetActive(false);
            }
        }



    }
}
