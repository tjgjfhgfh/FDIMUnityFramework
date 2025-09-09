using System.Collections;
using System.Collections.Generic;
using Txx.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button2D : Button
{
    private ButtonAnimationBase _buttonAnimation;

    protected override void Awake()
    {
        base.Awake();
        _buttonAnimation = GetComponent<ButtonAnimationBase>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (interactable)
            _buttonAnimation.SelcetAnimation();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (interactable)
            _buttonAnimation.PressAnimation();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (interactable)
            _buttonAnimation.LeaveAnimation();
    }
}