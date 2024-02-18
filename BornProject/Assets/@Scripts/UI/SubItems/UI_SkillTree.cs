using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_SkillTree : UI_Base {

    public static readonly Color DefaultColor = Color.white;
    public static readonly Color DeactivatedColor = (Color)new Color32(39, 65, 98, 255);
    public static readonly Color ActivatedColor = (Color)new Color32(12, 241, 221, 255);
    public static readonly float SlotSize = 125f;
    public static readonly float HorizontalSpacing = 25f;
    public static readonly float VerticalLineLength = 75f;
    public static readonly float PointSize = 12.5f;

    public UI_SkillSlot Parent { get; protected set; }

    private List<UI_SkillSlot> _childrenSlots = new();

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        return true;
    }

    private UI_SkillTreeLine CreateLine(UI_SkillTreeLine parentLine, Vector2 position, Vector2 size) {
        UI_SkillTreeLine newLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine == null ? this.transform : parentLine.transform, pooling: true);
        newLine.Parent = parentLine;
        newLine.Position = position;
        newLine.Size = size;
        return newLine;
    }
    private UI_SkillSlot CreateSlot(UI_SkillTreeLine parentLine, Vector2 position, SkillData data) {
        UI_SkillSlot newSlot = Main.UI.CreateSubItem<UI_SkillSlot>(parentLine == null ? this.transform : parentLine.transform, pooling: true);
        newSlot.Parent = parentLine;
        newSlot.SetInfo(data);
        newSlot.Position = position;
        return newSlot;
    }

    public void Temp(UI_SkillSlot parent) {
        // #1. 위치 및 컬렉션 초기화.
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        List<UI_SkillTreeLine> points = new();
        List<UI_SkillTreeLine> children = new();
        List<UI_SkillTreeLine> horizontals = new();

        // #2. 데이터 설정.
        this.Parent = parent;
        List<SkillData> dataList;
        if (Parent.Data == null) dataList = Main.Data.Skills.Values.Where(x => x.Level == SkillLevel.Base && x.Type == Parent.Type).ToList();
        else if (Parent.Data.Level != SkillLevel.Rare) dataList = Main.Data.Skills.Values.Where(x => x.Key.Contains(Parent.Data.Key.Split('_')[0]) && x.Level == Parent.Data.Level + 1).ToList();
        else return;

        // #3. 부모 라인 생성.
        float parentX = Parent.X;
        float parentY = Parent.Y;
        float parentLineY = parentY - (Parent.Size.y / 2 + VerticalLineLength / 2);
        UI_SkillTreeLine parentLine = CreateLine(null, new(parentX, parentLineY), new(PointSize, VerticalLineLength));

        // #4. 자식 라인 위치 계산.
        float pointY = -(VerticalLineLength / 2 + PointSize / 2);
        float childLineY = -(PointSize / 2 + VerticalLineLength / 2);
        float slotY = -(VerticalLineLength / 2 + SlotSize / 2);

        // #5. 자식 라인 생성.
        int count = dataList.Count;
        float x = -(count - 1) / 2 * (SlotSize + HorizontalSpacing);
        for (int i = 0; i < count; i++) {
            points.Add(CreateLine(parentLine, new(x, pointY), new(PointSize, PointSize)));
            children.Add(CreateLine(points[^1], new(0, childLineY), new(PointSize, VerticalLineLength)));
            _childrenSlots.Add(CreateSlot(children[^1], new(0, slotY), dataList[i]));
            x += (SlotSize + HorizontalSpacing);
        }

        if (points.Count == 1) {
            parentLine.Bottom = points[0];
            points[0].Bottom = children[0];

            parentLine.Type = SkillTreeLineType.Vertical;
            points[0].Type = SkillTreeLineType.Vertical;
            children[0].Type = SkillTreeLineType.Vertical;
        }
        else {
            for (int i = 0; i < points.Count - 1; i++) {
                UI_SkillTreeLine left = points[i];
                UI_SkillTreeLine right = points[i + 1];
                if (left.Position.x < parentX && parentX < right.Position.x) {
                    UI_SkillTreeLine leftHorizontalLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine.transform, pooling: true);
                    leftHorizontalLine.Position = new((parentX + left.X) / 2, left.Y);
                    leftHorizontalLine.Size = new(parentX - left.Position.x - PointSize, PointSize);
                    horizontals.Add(leftHorizontalLine);

                    UI_SkillTreeLine childPoint = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine.transform, pooling: true);
                    childPoint.Position = new(parentX, left.X);
                    childPoint.Size = new(PointSize, PointSize);
                    horizontals.Add(childPoint);

                    UI_SkillTreeLine rightHorizontalLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine.transform, pooling: true);
                    rightHorizontalLine.Position = new((right.X + parentX) / 2, left.Y);
                    rightHorizontalLine.Size = new(right.Position.x - parentX - PointSize, PointSize);
                    horizontals.Add(rightHorizontalLine);

                    left.Right = leftHorizontalLine;
                    leftHorizontalLine.Left = left;
                    leftHorizontalLine.Right = childPoint;
                    childPoint.Left = leftHorizontalLine;
                    childPoint.Right = rightHorizontalLine;
                    rightHorizontalLine.Left = childPoint;
                    rightHorizontalLine.Right = right;
                    right.Left = rightHorizontalLine;
                    parentLine.Bottom = childPoint;
                    continue;
                }
                UI_SkillTreeLine horizontalLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine.transform, pooling: true);
                horizontalLine.Position = (right.Position + left.Position) / 2;
                horizontalLine.Size = new(right.Position.x - left.Position.x - PointSize, PointSize);
                horizontals.Add(horizontalLine);

                left.Right = horizontalLine;
                horizontalLine.Left = left;
                horizontalLine.Right = right;
                right.Left = horizontalLine;
            }
            parentLine.Type = SkillTreeLineType.Vertical;
            for (int i = 0; i < points.Count; i++) {
                points[i].Bottom = children[i];
                if (i == 0) points[i].Type = SkillTreeLineType.LeftCorner;
                else if (i == points.Count - 1) points[i].Type = SkillTreeLineType.RightCorner;
                else points[i].Type = SkillTreeLineType.Point;
                children[i].Type = SkillTreeLineType.Vertical;
            }
            for (int i = 0; i < horizontals.Count; i++) {
                if (parentLine.Bottom == horizontals[i])
                    horizontals[i].Type = SkillTreeLineType.Point;
                else
                    horizontals[i].Type = SkillTreeLineType.Horizontal;
            }
        }
    }

}

public enum SkillTreeLineType {
    Horizontal,
    Vertical,
    LeftCorner,
    RightCorner,
    Point,
}

public interface ISkillTreeElement {

    public ISkillTreeElement ActivatedParent { get; }
    public ISkillTreeElement ActivatedChild { get; }

    public void Activate(bool isActivated);

}