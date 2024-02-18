using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TreeLine : UI_Base {

    public static readonly Color DefaultColor = Color.white;
    public static readonly Color DeactivatedColor = (Color)new Color32(39, 65, 98, 255);
    public static readonly Color ActivatedColor = (Color)new Color32(12, 241, 221, 255);
    public static readonly float SlotSize = 125f;
    public static readonly float HorizontalSpacing = 25f;
    public static readonly float VerticalLineLength = 75f;
    public static readonly float HorizontalLineLength = 137.5f;
    public static readonly float HalfHorizontalLineLength = 62.5f;
    public static readonly float PointSize = 12.5f;

    #region Properties

    public TreeLineType Type {
        get => _type;
        set {
            _type = value;
        }
    }

    public UI_TreeLine Parent { get; protected set; }

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

    public UI_TreeLine Top {
        get {
            if (!_neighbours.TryGetValue(TreeDirection.Top, out var line)) return null;
            return line;
        }
    }
    public UI_TreeLine Right {
        get {
            if (!_neighbours.TryGetValue(TreeDirection.Right, out var line)) return null;
            return line;
        }
    }
    public UI_TreeLine Bottom {
        get {
            if (!_neighbours.TryGetValue(TreeDirection.Bottom, out var line)) return null;
            return line;
        }
    }
    public UI_TreeLine Left {
        get {
            if (!_neighbours.TryGetValue(TreeDirection.Left, out var line)) return null;
            return line;
        }
    }

    #endregion

    #region Fields

    private TreeLineType _type;

    // Collections.
    private Dictionary<TreeDirection, UI_TreeLine> _neighbours = new();

    // Components.
    private RectTransform _rect;
    private Image _line;
    private Image _fill;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _rect = this.GetComponent<RectTransform>();
        _line = this.GetComponent<Image>();
        _fill = this.transform.GetChild(0).GetComponent<Image>();

        return true;
    }

    public void SetInfo() {
        this.Type = TreeLineType.Vertical;
        Rect.anchoredPosition = new(0, -(SlotSize + VerticalLineLength) / 2f);
        Rect.sizeDelta = new(PointSize, VerticalLineLength);
        RefreshImage();
    }
    public void SetInfo(UI_TreeLine parent, TreeDirection parentDirection, TreeLineType type) {
        this.Parent = parent;
        this.Type = type;
        _neighbours[parentDirection] = this;

        Vector2 size = type switch {
            TreeLineType.Horizontal => new(HorizontalLineLength, PointSize),
            TreeLineType.HalfHorizontal => new(HalfHorizontalLineLength, PointSize),
            TreeLineType.Vertical => new(PointSize, VerticalLineLength),
            _ => new(PointSize, PointSize),
        };
        Vector2 position = parent.Position;
        switch (parentDirection) {
            case TreeDirection.Top: position.y -= (size.y + parent.Size.y) / 2f; break;
            case TreeDirection.Right: position.x -= (size.x + parent.Size.x) / 2f; break;
            case TreeDirection.Left: position.x += (size.x + parent.Size.x) / 2f; break;
        }
        Rect.anchoredPosition = position;
        Rect.sizeDelta = size;

        RefreshImage();
    }

    #endregion

    private void CreateNewLine(TreeDirection direction, TreeLineType type) {
        if (_neighbours.ContainsKey(direction)) return;
        TreeDirection offDirection = direction switch {
            TreeDirection.Top => TreeDirection.Bottom,
            TreeDirection.Left => TreeDirection.Right,
            TreeDirection.Bottom => TreeDirection.Top,
            TreeDirection.Right => TreeDirection.Left,
            _ => TreeDirection.Top
        };

        UI_TreeLine newLine = Main.UI.CreateSubItem<UI_TreeLine>(this.transform, pooling: true);
        _neighbours[direction] = newLine;
        newLine.SetInfo(this, offDirection, type);

        RefreshImage();
    }

    private void RefreshImage() {
        string key = $"UI_SkillTreeLines_{Type}";
        if (Type == TreeLineType.Point) {
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