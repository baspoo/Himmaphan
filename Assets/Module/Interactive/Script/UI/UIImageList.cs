using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIImageList : MonoBehaviour
{

    public UITexture Texture;
    public List<Texture> Images;
    public Transform btnLeft,btnRight;
    public bool Loop;
    public System.Action<int> OnChange;

    public int GetIndex => Index;
    int Index;


    public void Init( List<string> images )
    {
        
    }

    public void OnReset()
    {
        Index = 0;
        UpdateImage();
    }
    public void OnJumpToIndex(int index)
    {
        if (index >= Images.Count)
        {
            return;
        }
        if (index < 0)
        {
            return;
        }
        Index = index;
        UpdateImage();
    }
    public void OnRight()
    {
        Index++;
        if (Index >= Images.Count) 
        {
            if (Loop) Index = 0;
            else Index--;
        }
        UpdateImage();
    }
    public void OnLeft()
    {
        Index--;
        if (Index < 0)
        {
            if (Loop) Index = Images.Count-1;
            else Index++;
        }
        UpdateImage();
    }



    void UpdateImage() 
    {
        if (Index < Images.Count) 
        {
            Texture.mainTexture = Images[Index];
        }

        btnRight?.SetActive(Index < (Images.Count-1) || Loop);
        btnLeft?.SetActive(Index > 0 || Loop);

        OnChange?.Invoke(Index);

    }








}
