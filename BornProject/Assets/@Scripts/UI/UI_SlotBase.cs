using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotBase : UI_Base {

    #region Fields

    private Image _imgSlot;
    private Image _imgContent;
    private Button _button;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _imgSlot = this.GetComponent<Image>();
        _imgContent = this.gameObject.FindChild<Image>("imgContent");
        _button = this.GetComponent<Button>();
        if (_button != null) _button.onClick.AddListener(OnClickSlot);

        return true;
    }

    public void SetSlotImage(Sprite sprite) {
        Initialize();

        _imgSlot.sprite = sprite;
    }
    public void SetImage(Sprite sprite) {
        Initialize();

        _imgContent.sprite = sprite;
        _imgContent.color = new(1, 1, 1, sprite != null ? 1 : 0);
    }

    #endregion

    public virtual void OnClickSlot() {
        if (_button == null) return;
    }

}