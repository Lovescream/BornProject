using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotBase : UI_Base {

    #region Fields

    private Image _imgSlot;
    private Image _imgContent;
    private TextMeshProUGUI _txtContent;
    private Button _button;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _imgSlot = this.GetComponent<Image>();
        _imgContent = this.gameObject.FindChild<Image>("imgContent");
        _txtContent = this.gameObject.FindChild<TextMeshProUGUI>("txtContent");
        if (this.TryGetComponent(out _button)) _button.onClick.AddListener(OnClickSlot);

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
    public void SetText(string text) {
        _txtContent.text = text;
    }

    #endregion

    public virtual void OnClickSlot() {
        if (_button == null) return;
    }

}