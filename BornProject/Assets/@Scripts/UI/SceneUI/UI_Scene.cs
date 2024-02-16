using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_Scene : UI_Base {

    #region Fields

    protected Canvas _canvas;

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

<<<<<<< HEAD
        this.SetCanvas();
=======
        _canvas = this.SetCanvas();
>>>>>>> Develop1.0
        SetOrder();

        return true;
    }

    protected override void SetOrder() => _canvas.sortingOrder = 0;

<<<<<<< HEAD
   /* public override bool Init()
    {
        if (!base.Init()) return false;

        Main.UI.SetCanvas(gameObject, false);

        return true;
    }
  */
=======
>>>>>>> Develop1.0
}

