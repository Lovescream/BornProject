using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillSlot : UI_Base, ISkillTreeElement {

    #region Properties

    public ISkillTreeElement ActivatedParent { get; set; }
    public ISkillTreeElement ActivatedChild { get; set; }

    public SkillData Data { get; protected set; }
    public SkillType Type => Data != null ? Data.Type : _type;
    public UI_SkillTree Tree { get; protected set; }

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

    [SerializeField]
    private SkillType _type;

    // Components.
    private RectTransform _rect;

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rect = this.GetComponent<RectTransform>();

        return true;
    }

    public void SetInfo(SkillData data) {
        Initialize();

        this.Data = data;


    }

    public void GenerateTree() {
        Tree = Main.UI.CreateSubItem<UI_SkillTree>(this.transform);
        Tree.Temp(this);
    }

    public void Activate(bool isActivated) {

    }
}