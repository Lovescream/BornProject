using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_VolumeController : UI_Base {

    private readonly Color DefaultColor = Color.white;
    private readonly Color DisableColor = (Color)new Color32(100, 100, 100, 255);

    #region Enums

    enum Buttons {
        btnMute
    }
    enum Texts {
        txtBtnMute
    }
    enum Sliders {
        Slider
    }

    #endregion

    #region Properties

    public AudioType Type { get; protected set; }
    public bool IsMute {
        get => _isMute;
        set {
            _isMute = value;
            Main.Game.Current[Type, isMuteInfo: true] = value;
            GetButton((int)Buttons.btnMute).GetComponent<Image>().color = value ? DisableColor : DefaultColor;
            GetText((int)Texts.txtBtnMute).color = value ? DisableColor : DefaultColor;
        }
    }

    #endregion

    #region Fields

    private bool _isMute;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));
        GetButton((int)Buttons.btnMute).onClick.AddListener(OnBtnMute);
        EventTrigger eventTrigger = GetSlider((int)Sliders.Slider).GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new() {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener(OnSliderPointUp);
        eventTrigger.triggers.Add(entry);

        return true;
    }

    public void SetInfo(AudioType type) {
        this.Type = type;
        GetText((int)Texts.txtBtnMute).text = $"{Type}";
        GetSlider((int)Sliders.Slider).value = Main.Game.Current[Type];
        IsMute = Main.Game.Current[Type, isMuteInfo: true];
    }

    #endregion

    #region OnButtons

    private void OnBtnMute() {
        IsMute = !IsMute;
    }

    private void OnSliderPointUp(BaseEventData data) {
        Main.Game.Current[Type] = GetSlider((int)Sliders.Slider).value;
    }

    #endregion
}