using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_Scene : UI_Base {

    #region Fields

    protected Canvas _canvas;

    #endregion

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _canvas = this.SetCanvas();
        SetOrder();

        return true;
    }

    protected override void SetOrder() => _canvas.sortingOrder = 0;

}
