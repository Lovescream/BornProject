using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_Scene : UI_Base {
 
   public override bool Initialize() {
        if (!base.Initialize()) return false;

        this.SetCanvas();
        SetOrder();

        return true;
    }

    protected override void SetOrder() {
          this.GetComponent<Canvas>().sortingOrder = 0;
    }

   /* public override bool Init()
    {
        if (!base.Init()) return false;

        Main.UI.SetCanvas(gameObject, false);

        return true;
    }
  */
}

