using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotBase : UI_Base {

    #region Fields

    private Image _image;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _image = this.gameObject.FindChild<Image>("imgContent");

        return true;
    }

    public void SetImage(Sprite sprite) {
        Initialize();

        _image.sprite = sprite;
        _image.color = new(1, 1, 1, sprite != null ? 1 : 0);
    }

    #endregion

}