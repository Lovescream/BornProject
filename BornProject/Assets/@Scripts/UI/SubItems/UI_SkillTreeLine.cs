using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTreeLine : UI_Base, ISkillTreeElement {

    #region Properties

    public SkillTreeLineType Type {
        get => _type;
        set {
            Initialize();
            _type = value;

            string key = $"UI_SkillTreeLines_{value}";
            if (value == SkillTreeLineType.Point) {
                _line.sprite = Main.Resource.LoadSprite(Bottom == null ? $"{key}_2" : $"{key}_3");
                _fill.sprite = Main.Resource.LoadSprite($"{key}_Empty");
                _line.color = UI_SkillTree.DeactivatedColor;
                _fill.color = UI_SkillTree.DeactivatedColor;
            }
            else {
                _line.sprite = Main.Resource.LoadSprite($"{key}Line");
                _fill.sprite = Main.Resource.LoadSprite($"{key}Fill");
                _line.color = UI_SkillTree.DefaultColor;
                _fill.color = UI_SkillTree.DeactivatedColor;
            }
        }
    }

    public ISkillTreeElement ActivatedParent { get; set; }
    public ISkillTreeElement ActivatedChild { get; set; }

    public UI_SkillTreeLine Parent { get; set; }
    public UI_SkillTreeLine Left { get; set; }
    public UI_SkillTreeLine Bottom { get; set; }
    public UI_SkillTreeLine Right { get; set; }

    public RectTransform Rect {
        get {
            if (_rect == null) Initialize();
            return _rect;
        }
    }
    public float X => Rect.anchoredPosition.x;
    public float Y => Rect.anchoredPosition.y;
    public Vector2 Position {
        get => Rect.anchoredPosition;
        set => Rect.anchoredPosition = value;
    }
    public Vector2 Size {
        get => Rect.sizeDelta;
        set => Rect.sizeDelta = value;
    }
    #endregion

    #region Fields

    private SkillTreeLineType _type;

    // Components.
    private Image _line;
    private Image _fill;
    private RectTransform _rect;

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _line = this.GetComponent<Image>();
        _fill = this.transform.GetChild(0).GetComponent<Image>();
        _rect = this.GetComponent<RectTransform>();

        return true;
    }

    public void Activate(bool isActivated) {
        if (Type != SkillTreeLineType.Point) {
            _line.color = UI_SkillTree.DefaultColor;
            _fill.color = isActivated ? UI_SkillTree.ActivatedColor: UI_SkillTree.DeactivatedColor;
        }
        else {
            _line.color = isActivated ? UI_SkillTree.DefaultColor : UI_SkillTree.DeactivatedColor;
            _fill.color = UI_SkillTree.ActivatedColor;
        }
        if (ActivatedChild != null) ActivatedChild.Activate(isActivated);
    }

}