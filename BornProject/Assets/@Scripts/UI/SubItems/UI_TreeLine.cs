using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_TreeLine : UI_Base {

    #region Const

    public static readonly Color DefaultColor = Color.white;
    public static readonly Color DeactivatedColor = (Color)new Color32(39, 65, 98, 255);
    public static readonly Color ActivatedColor = (Color)new Color32(12, 241, 221, 255);
    public static readonly float SlotSize = 125f;
    public static readonly float HorizontalSpacing = 25f;
    public static readonly float VerticalLineLength = 50f;
    public static readonly float HorizontalLineLength = 137.5f;
    public static readonly float HalfHorizontalLineLength = 62.5f;
    public static readonly float PointSize = 12.5f;

    #endregion

    #region Properties

    public TreeLineType Type { get; set; }

    public UI_SkillSlot Slot { get; protected set; } // Connected Slot.

    #region Neighbours Info

    public UI_TreeLine Parent { get; protected set; }
    public UI_TreeLine Top => this[TreeDirection.Top];
    public UI_TreeLine Right => this[TreeDirection.Right];
    public UI_TreeLine Bottom => this[TreeDirection.Bottom];
    public UI_TreeLine Left => this[TreeDirection.Left];
    private UI_TreeLine this[TreeDirection direction] => _neighbours.TryGetValue(direction, out var line) ? line : null;

    #endregion

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

    // Collections.
    private Dictionary<TreeDirection, UI_TreeLine> _neighbours = new();

    // Components.
    private RectTransform _rect;
    private Image _line;
    private Image _fill;
    private GameObject _block;
    private RectTransform _blockRect;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rect = this.GetComponent<RectTransform>();
        _line = this.GetComponent<Image>();
        _fill = this.transform.GetChild(0).GetComponent<Image>();
        _block = this.transform.GetChild(1).gameObject;
        _blockRect = _block.GetComponent<RectTransform>();
        return true;
    }

    public void SetInfo(UI_SkillSlot slot) {
        this.Type = TreeLineType.Vertical;
        Rect.anchoredPosition = new(0, -(slot.Size.y + VerticalLineLength) / 2f);
        Rect.sizeDelta = new(PointSize, VerticalLineLength);
        RefreshImage();
    }

    public void SetInfo(UI_TreeLine parent, TreeDirection parentDirection, TreeLineType type) {
        this.Parent = parent;
        this.Type = type;
        _neighbours[parentDirection] = parent;

        Vector2 size = type switch {
            TreeLineType.Horizontal => new(HorizontalLineLength, PointSize),
            TreeLineType.HalfHorizontal => new(HalfHorizontalLineLength, PointSize),
            TreeLineType.Vertical => new(PointSize, VerticalLineLength),
            _ => new(PointSize, PointSize),
        };
        Rect.sizeDelta = size;
        Rect.anchoredPosition = parentDirection switch {
            TreeDirection.Top => new(0, -(size.y + parent.Size.y) / 2f),
            TreeDirection.Right => new(-(size.x + parent.Size.x) / 2f, 0),
            TreeDirection.Left => new((size.x + parent.Size.x) / 2f, 0),
            _ => Vector2.zero
        };

        RefreshImage();
    }

    #endregion

    #region Line / Slot

    public UI_TreeLine CreateNewLine(TreeDirection direction, TreeLineType type) {
        if (_neighbours.TryGetValue(direction, out UI_TreeLine line)) return line;

        int directionIndex = (int)direction + 2;
        if (directionIndex > 3) directionIndex -= 4;
        TreeDirection offDirection = (TreeDirection)directionIndex;

        line = Main.UI.CreateSubItem<UI_TreeLine>(this.transform, pooling: false);
        _neighbours[direction] = line;
        line.SetInfo(this, offDirection, type);

        RefreshImage();

        return line;
    }

    public UI_SkillSlot ConnectSlot(UI_SkillSlot parent, SkillData data) {
        if (Slot == null) Slot = Main.UI.CreateSubItem<UI_SkillSlot>(this.transform, pooling: false);
        Slot.SetInfo(this, parent, data);
        return Slot;
    }

    #endregion

    #region Activate / Deactivate

    public void Activate(UI_TreeLine subLine) {
        Initialize();
        if (Type == TreeLineType.Point) {
            int index = 0b0000;
            _neighbours.Where(x => x.Value == subLine || x.Value == Parent).Select(x => (int)x.Key).ToList().ForEach(x => index |= 1 << x);
            string endKey = index switch {
                3 => "Point_Left",
                5 => "VerticalFill",
                6 => "RightCornerFill",
                9 => "Point_Right",
                10 => "HorizontalFill",
                12 => "LeftCornerFill",
                _ => ""
            };
            _fill.sprite = Main.Resource.LoadSprite($"UI_SkillTreeLines_{endKey}");
        }
        _fill.color = ActivatedColor;
        _line.color = Type == TreeLineType.Point && !(Left == null && Right == null) ? DeactivatedColor : DefaultColor;
    }
    public void Deactivate() {
        Initialize();
        RefreshImage();
    }

    #endregion

    private string GetSpriteKey(bool isLine = true) {
        string key = $"UI_SkillTreeLines_";
        string mid;
        string end = isLine ? "Line" : "Fill";
        if (Type == TreeLineType.HalfHorizontal) return $"{key}Horizontal{end}";
        else if (Type == TreeLineType.Point) {
            if (Left != null && Right != null) return $"{key}{(isLine ? "Point_3" : "Point_Empty")}";
            else if (Left != null) mid = "RightCorner";
            else if (Right != null) mid = "LeftCorner";
            else mid = "Vertical";
        }
        else {
            mid = $"{Type}";
        }
        return $"{key}{mid}{end}";
    }

    private void SetBlock(bool active) {
        if (!active) {
            _block.SetActive(false);
            return;
        }
        float blockAnchor = Top == null ? 1 : 0;
        if ((Top == null && Bottom == null) || (Top != null && Bottom != null)) {
            _block.SetActive(false);
        }
        else {
            _block.SetActive(true);
            _blockRect.anchorMax = new(1, blockAnchor);
            _blockRect.anchorMin = new(0, blockAnchor);
            _blockRect.pivot = new(0.5f, blockAnchor);
            _blockRect.sizeDelta = new(0, 3.125f);
        }
    }

    private void RefreshImage() {
        _line.sprite = Main.Resource.LoadSprite(GetSpriteKey(true));
        _fill.sprite = Main.Resource.LoadSprite(GetSpriteKey(false));
        SetBlock(Type == TreeLineType.Point);
        _line.color = Type == TreeLineType.Point && Left != null && Right != null ? DeactivatedColor : DefaultColor;
        _fill.color = DeactivatedColor;
    }

}

public enum TreeDirection {
    Top,
    Left,
    Bottom,
    Right,
}
public enum TreeLineType {
    Horizontal,
    HalfHorizontal,
    Vertical,
    LeftCorner,
    RightCorner,
    Point,
}