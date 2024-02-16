using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_Popup : UI_Base
{
    protected Canvas _canvas;
    protected Transform _panel;

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        this.SetCanvas();
        _canvas = this.GetComponent<Canvas>();
        SetOrder();

        _panel = this.transform.Find("Panel");

        return true;
    }

    protected override void SetOrder() => _canvas.sortingOrder = Main.UI.OrderUpPopup();

    public void SetOrder(int order) => _canvas.sortingOrder = order;

    public virtual void ClosePopup() => Main.UI.ClosePopup(this);

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_panel == null) return;

        this.SetPopupToFront();
    }
}
