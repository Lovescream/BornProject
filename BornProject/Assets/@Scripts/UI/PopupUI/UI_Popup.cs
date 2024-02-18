using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base {

    #region Properties

    public virtual bool IsPause => false;

    #endregion

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

    public virtual void Close() => Main.UI.ClosePopup(this);

}