using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceButtonTexture : MonoBehaviour
{
    [SerializeField]
    public UIButton uiBtn;
    [SerializeField]
    public UITexture uiTexture;
    [SerializeField]
    public UITexture uiIcon;
    [SerializeField]
    public UILabel txtLabel;
    [Header("States")]
    public StateTexture BtnEnable = new StateTexture();
    public StateTexture BtnDisable = new StateTexture();

    [System.Serializable]
    public class StateTexture
    {
        public enum State
        {
            Normal,
            Hover,
            Pressed,
            Disabled,
            Custom
        }

        public Texture Texture;
        public string BtnName;
        public Color ColorLabel;
        public Color ColorLabelStroke;

        public Texture Icon;
        public Color ColorIcon;
    }

    int enableBtn = 0;
    private void Update()
    {
        if (uiBtn != null) 
        {
            if (uiBtn.isEnabled)
            {
                if (enableBtn != 1) 
                {
                    enableBtn = 1;
                    UpdateTexture(BtnEnable);
                }
            }
            else 
            {
                if (enableBtn != 2)
                {
                    enableBtn = 2;
                    UpdateTexture(BtnDisable);
                }
            }
        }
    }

    void UpdateTexture(StateTexture property)
    {
        if (uiTexture != null && property.Texture != null) uiTexture.mainTexture = property.Texture;
        if (txtLabel != null)
        {
            txtLabel.color = property.ColorLabel;
            txtLabel.effectColor = property.ColorLabelStroke;
            if (property.BtnName.notnull())
                txtLabel.text = property.BtnName;
        }
        if (uiIcon != null)
        {
            uiIcon.color = property.ColorIcon;
            if (property.Icon!=null)
                uiIcon.mainTexture = property.Icon;
        }
    }

}



