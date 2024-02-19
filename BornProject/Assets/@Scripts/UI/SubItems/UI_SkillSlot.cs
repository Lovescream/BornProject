using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillSlot : UI_SlotBase {

    #region Properties

    public UI_Popup_Skill Panel { get; protected set; }
    public UI_SkillSlot Parent { get; protected set; }
    public SkillData Data { get; protected set; }
    public SkillType Type => Data != null ? Data.Type : _type;
    public UI_TreeLine Line { get; protected set; } // Connected Line.
    public bool DefaultSlot { get; protected set; }
    public bool Available {
        get => _available;
        set {
            _available = value;
            SetSlotImage(Main.Resource.LoadSprite($"UI_SkillSlot_{(value ? "Available" : "Unavailable")}"));
        }
    }
    public bool Activated {
        get => _activated;
        set {
            _activated = value;
            SetSlotImage(Main.Resource.LoadSprite($"UI_SkillSlot_{(value ? "Selected" : "Available")}"));
        }
    }

    #region Rect Info

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

    #endregion

    #region Fields

    [SerializeField]
    private SkillType _type;
    private bool _activated;
    private bool _available;

    // Collections.
    private List<SkillData> _subs;
    private List<UI_SkillSlot> _slots = new();

    // Components.
    private RectTransform _rect;

    #endregion

    #region Indexer

    public UI_SkillSlot this[SkillData data] {
        get {
            if (_slots.Count == 0) return null;
            return _slots.Where(x => x.Data == data).FirstOrDefault();
        }
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rect = this.GetComponent<RectTransform>();

        return true;
    }

    public void SetInfo(UI_Popup_Skill panel) {
        Initialize();

        this.Panel = panel;
        this.DefaultSlot = true;

        if (Line == null) GenerateTree();
    }
    public void SetInfo(UI_TreeLine line, UI_SkillSlot parent, SkillData data) {
        Initialize();

        Line = line;
        Parent = parent;
        Rect.sizeDelta = new(UI_TreeLine.SlotSize, UI_TreeLine.SlotSize);
        Rect.anchoredPosition = new(line.X, -(line.Size.y + this.Size.y) / 2f);
        this.DefaultSlot = Parent.DefaultSlot;

        SetInfo(Parent.Panel, data);
    }

    public void SetInfo(UI_Popup_Skill panel, SkillData data) {
        Initialize();

        this.Panel = panel;
        this.Data = data;
        SetImage(Main.Resource.LoadSprite($"Icon_{data.Key}"));
        SetText(data.Name);

        Available = true;
        Activated = false;
    }

    public void SetInfoSkillTree(UI_Popup_Skill panel, SkillData data) {
        Initialize();

        this.Panel = panel;
        this.Data = data;
        SetImage(Main.Resource.LoadSprite($"Icon_{data.Key}"));
        SetText(data.Name);

        Available = true;
        Activated = false;

        RemoveTree();
        GenerateTree();
    }

    #endregion

    #region Activate / Deactivate

    public void Activate() {
        Available = true;
        Activated = true;

        if (Parent != null) Parent.DeactivateChildren();

        UI_TreeLine subLine = null;
        UI_TreeLine parentLine = Line;
        while (parentLine != null) {
            parentLine.Activate(subLine);
            subLine = parentLine;
            parentLine = parentLine.Parent;
        }

        Main.Skill.GetSkill(Data);
    }
    public void Deactivate() {
        Activated = false;
        Available = true;

        UI_TreeLine line = Line;
        while (line != null) {
            line.Deactivate();
            line = line.Parent;
        }
    }
    public void DeactivateChildren() {
        _slots.ForEach(x => x.Deactivate());
    }

    #endregion

    public void RemoveTree() {
        if (Line != null) Main.Resource.Destroy(Line.gameObject);
        _subs = null;
        _slots.Clear();
    }

    public void GenerateTree() {
        if (_subs == null) SetSubSkillData();

        Line = Main.UI.CreateSubItem<UI_TreeLine>(this.transform, pooling: false);
        Line.SetInfo(this);
        UI_TreeLine basePoint = Line.CreateNewLine(TreeDirection.Bottom, TreeLineType.Point);

        int mid = _subs.Count / 2;
        UI_TreeLine leftBase = basePoint;
        UI_TreeLine rightBase = basePoint;
        if (_subs.Count % 2 == 1)
            _slots.Add(basePoint.CreateNewLine(TreeDirection.Bottom, TreeLineType.Vertical).ConnectSlot(this, _subs[mid]));
        for (int i = 0; i < mid; i++) {
            TreeLineType horizontal = _subs.Count % 2 == 0 && i == 0 ? TreeLineType.HalfHorizontal : TreeLineType.Horizontal;

            leftBase = leftBase.CreateNewLine(TreeDirection.Left, horizontal).CreateNewLine(TreeDirection.Left, i == mid - 1 ? TreeLineType.LeftCorner : TreeLineType.Point);
            _slots.Add(leftBase.CreateNewLine(TreeDirection.Bottom, TreeLineType.Vertical).ConnectSlot(this, _subs[mid - i - 1]));

            rightBase = rightBase.CreateNewLine(TreeDirection.Right, horizontal).CreateNewLine(TreeDirection.Right, i == mid - 1 ? TreeLineType.RightCorner : TreeLineType.Point);
            _slots.Add(rightBase.CreateNewLine(TreeDirection.Bottom, TreeLineType.Vertical).ConnectSlot(this, _subs[mid + i + (_subs.Count % 2 == 1 ? 1 : 0)]));
        }
    }


    private void SetSubSkillData() {
        if (Data == null)
            _subs = Main.Data.Skills.Values.Where(x => x.Level == SkillLevel.Base && x.Type == Type).ToList();
        else if (Data.Level != SkillLevel.Rare)
            _subs = Main.Data.Skills.Values.Where(x => x.Key.Split('_')[0] == Data.Key.Split('_')[0] && x.Level == Data.Level + 1).ToList();
        else
            _subs = new();
    }

    #region OnButtons

    public override void OnClickSlot() {
        base.OnClickSlot();

        if (!Available) return;

        if (!Activated) {
            Activate();
            if (DefaultSlot && Data != null && Data.Level == SkillLevel.Base)
                Panel.SelectSkill(Data);
        }
        else {
            Deactivate();
        }
    }

    #endregion

}