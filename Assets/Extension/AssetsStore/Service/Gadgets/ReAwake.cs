using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReAwake : MonoBehaviour {




	public static void ReAllTween(  Transform root, bool isForceToBegin = false)
	{
		foreach (UITweener gettween in root.gameObject.GetComponents<UITweener> ()) {
			ReTween(gettween, isForceToBegin);
		}
	}
	public static void ReTween(UITweener tween , bool isForceToBegin = false )
	{
		tween.ResetToBeginning();
		tween.PlayForward();
		if (isForceToBegin)
			FoceToBegin(tween);
	}
	public static void EnableAll( Transform root , bool isEnable)
	{
		foreach (UITweener gettween in root.gameObject.GetComponents<UITweener>()){
			gettween.enabled = isEnable;
		}
	}


	public static void FoceToBegin(UITweener tween)
	{

		var tweenalpha = tween.GetComponent<TweenAlpha>();
		if (tweenalpha != null)
		{
			if (tween.GetComponent<UIPanel>() != null) tween.GetComponent<UIPanel>().alpha = tweenalpha.from;
			if (tween.GetComponent<UILabel>() != null) tween.GetComponent<UILabel>().alpha = tweenalpha.from;
			if (tween.GetComponent<UITexture>() != null) tween.GetComponent<UITexture>().alpha = tweenalpha.from;
		}

		var tweenposition = tween.GetComponent<TweenPosition>();
		if (tweenposition != null)
		{
			tween.transform.localPosition = tweenposition.from;
		}

		var tweenscale = tween.GetComponent<TweenScale>();
		if (tweenscale != null)
		{
			tween.transform.localScale = tweenscale.from;
		}
	}



	public UITweener tween;
	public bool isReAllTween;
	public bool isReAllParent;
	public bool isForceToBegin;
	public bool isTypeWritting;
	public ParticleSystem particleSystem;
	public float autoDeactive;
	void OnEnable()
	{
		if (tween!=null) 
		{
			ReTween(tween, isForceToBegin);
		}
		if (isReAllTween)
		{
			ReAllTween(transform, isForceToBegin);
			if (tween != null) {
				ReAllTween(tween.transform, isForceToBegin);
			}
		}
		if (isReAllParent)
		{
			Service.GameObj.GetAllParent(transform).ForEach(x => {
				ReAllTween(x.transform, isForceToBegin);
			});
		}

		if (isTypeWritting)
		{
			var t = GetComponent<TypewriterEffect>();
			t.ResetToBeginning();
		}
		if (particleSystem != null)
		{
			particleSystem.Stop();
			particleSystem.Play();
		}
		runtime = 0.0f;
	}

	float runtime = 0.0f;
    private void Update()
    {
		if (autoDeactive == 0) return;

		runtime += Time.deltaTime;
		if (runtime >= autoDeactive)
		{
			runtime = 0.0f;
			gameObject.SetActive(false);
		}
	}




    public void OnReEnable()
	{
		OnEnable();
	}

	public void OnBtnRetween() {
		ReAllTween(transform, isForceToBegin);
	}


}
