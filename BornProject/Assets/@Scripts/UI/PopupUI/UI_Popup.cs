using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{

    #region Fields

    protected Canvas _canvas;

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        _canvas = this.SetCanvas();
        SetOrder();

        return true;
    }

    protected override void SetOrder() => _canvas.sortingOrder = Main.UI.OrderUpPopup();
    public void SetOrder(int order) => _canvas.sortingOrder = order;


}