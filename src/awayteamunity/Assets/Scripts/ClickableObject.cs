using UnityEngine;
using System.Collections;

public class ClickableObject : FSprite {
	public float xVelocity;
	public float yVelocity;
	public float defaultVelocity;
	public float currentVelocity;
	
	public ClickableObject() : base("ball.png") {
		defaultVelocity = 100.0f;
		currentVelocity = defaultVelocity;
	}
	
	public void Update(float dt) {
		
	}
	
}


/*

	
	public bool HandleSingleTouchBegan(FTouch touch)
	{
		Vector2 touchPos = _bg.GlobalToLocal(touch.position);
		
		if(_bg.textureRect.Contains(touchPos))
		{
			_bg.element = _downElement;
			
			if(_soundName != null) FSoundManager.PlaySound(_soundName);
			
			if(SignalPress != null) SignalPress(this);
			
			return true;	
		}
		
		return false;
	}
	
	public void HandleSingleTouchMoved(FTouch touch)
	{
		Vector2 touchPos = _bg.GlobalToLocal(touch.position);
		
		//expand the hitrect so that it has more error room around the edges
		//this is what Apple does on iOS and it makes for better usability
		Rect expandedRect = _bg.textureRect.CloneWithExpansion(expansionAmount);
		
		if(expandedRect.Contains(touchPos))
		{
			_bg.element = _downElement;	
		}
		else
		{
			_bg.element = _upElement;	
		}
	}
	
	public void HandleSingleTouchEnded(FTouch touch)
	{
		_bg.element = _upElement;
		
		Vector2 touchPos = _bg.GlobalToLocal(touch.position);
		
		//expand the hitrect so that it has more error room around the edges
		//this is what Apple does on iOS and it makes for better usability
		Rect expandedRect = _bg.textureRect.CloneWithExpansion(expansionAmount);
		
		if(expandedRect.Contains(touchPos))
		{
			if(SignalRelease != null) SignalRelease(this);
		}
		else
		{
			if(SignalReleaseOutside != null) SignalReleaseOutside(this);	
		}
	}
	
	public void HandleSingleTouchCanceled(FTouch touch)
	{
		_bg.element = _upElement;
		if(SignalReleaseOutside != null) SignalReleaseOutside(this);
	}*/