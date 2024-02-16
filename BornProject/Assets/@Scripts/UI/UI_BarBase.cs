using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BarBase : UI_Base {

    #region Properties

    public bool ShowValueText {
        get => _showValueText;
        set {
            if (_txtValue == null) _showValueText = false;
            _showValueText = value;
        }
    }
    public bool ShowValueTextFull {
        get => _showValueTextFull;
        set {
            if (ShowValueText == false) _showValueTextFull = false;
            _showValueTextFull = value;
        }
    }
    public bool ShowTitleText {
        get => _showTitleText;
        set {
            if (_txtTitle == null) _showTitleText = false;
            _showTitleText = value;
        }
    }

    #endregion

    #region Fields

    private bool _showValueText;
    private bool _showValueTextFull;
    private bool _showTitleText;

    private TextMeshProUGUI _txtTitle;
    private TextMeshProUGUI _txtValue;
    private Slider _slider;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _slider = this.GetComponentInChildren<Slider>();
        _txtTitle = this.gameObject.FindChild<TextMeshProUGUI>("txtTitle");
        _txtValue = this.gameObject.FindChild<TextMeshProUGUI>("txtValue");

        return true;
    }

    public void SetTitle(string title) {
        ShowTitleText = true;
        if (!ShowTitleText) return;
        _txtTitle.text = title;
    }

    public void SetAmount(float current, float max) {
        Initialize();

        _slider.value = Mathf.Clamp01(current / max);

        if (ShowValueText)
            _txtValue.text = ShowValueTextFull ? $"{current} / {max}" : $"{current}";
    }

    #endregion

}