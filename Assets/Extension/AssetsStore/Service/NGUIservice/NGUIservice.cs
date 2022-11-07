using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NGUIservice 
{

	public enum AspectRatio
	{
		Height, Width
	}
	public static void UITextureFixSizeByKeepAspectRatio(UITexture uiTexture, AspectRatio aspectRatio)
	{
		if (aspectRatio == AspectRatio.Height)
		{
			int Hight = uiTexture.height;
			uiTexture.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			uiTexture.MakePixelPerfect();

			uiTexture.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
			uiTexture.height = Hight;
		}
		if (aspectRatio == AspectRatio.Width)
		{
			int Width = uiTexture.width;
			uiTexture.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			uiTexture.MakePixelPerfect();

			uiTexture.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
			uiTexture.width = Width;
		}
	}
	//#if ngui
	public static void UITextureFixSize(UITexture image, int fixLength, AspectRatio type)
	{
		if (image != null)
		{
			image.MakePixelPerfect();
			if (type == AspectRatio.Height)
				SetTextureFixHight(image, fixLength);
			else
				SetTextureFixWidth(image, fixLength);
		}
	}
	public static void UITextureFixSize(UITexture image, int fixHight, int fixWidth, AspectRatio firstfix)
	{
		if (image != null)
		{
			image.MakePixelPerfect();
			if (firstfix == AspectRatio.Height)
			{
				SetTextureFixHight(image, fixHight);
				if (image.width > fixWidth) SetTextureFixWidth(image, fixWidth);

			}
			else
			{
				SetTextureFixWidth(image, fixWidth);
				if (image.height > fixHight) SetTextureFixHight(image, fixHight);
			}
		}
	}
	static void SetTextureFixHight(UITexture image, int fixHight)
	{
		int my_width = image.width;
		int my_height = image.height;

		float changeper = (float)fixHight / (float)my_height;
		image.height = fixHight;
		image.width = (int)(my_width * changeper);
	}
	static void SetTextureFixWidth(UITexture image, int fixWidth)
	{
		int my_width = image.width;
		int my_height = image.height;

		float changeper = (float)fixWidth / (float)my_width;
		image.height = (int)(my_height * changeper);
		image.width = fixWidth;
	}
}






public static class NGUIserviceFunction
{



    public static UIButton DisableActive(this List<UIButton> btns, string btnName) {
        btns.ForEach(x => x.isEnabled = true);
        var btn = btns.Find(x => x.name == btnName);
        if (btn != null) btn.isEnabled = false;
        return btn;
    }
    public static UIButton DisableActive(this List<UIButton> btns, UIButton btnName)
    {
        btns.ForEach(x => x.isEnabled = true);
        var btn = btns.Find(x => x == btnName);
        if (btn != null) btn.isEnabled = false;
        return btn;
    }


    public static void OnChangeColor( this UIButton btn , Color? normal , Color? hover, Color? pressed, Color? disabledColor)
    {
        if(normal!=null) btn.defaultColor = (Color)normal;
        if (hover != null) btn.hover = (Color)hover;
        if (pressed != null) btn.pressed = (Color)pressed;
        if (disabledColor != null) btn.disabledColor = (Color)disabledColor;
        btn.UpdateColor(true);
    }
    public static void OnChangeColor(this UIButton btn, Color[] colors )
    {
        if (colors.Length > 0) btn.defaultColor = colors[0];
        if (colors.Length > 1) btn.hover = colors[1];
        if (colors.Length > 2) btn.pressed = colors[2];
        if (colors.Length > 3) btn.disabledColor = colors[3];
        btn.UpdateColor(true);
    }

}