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

    public void Temp(UI_SkillSlot parent) {
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        this.Parent = parent;
        List<SkillData> dataList;
        if (Parent.Data == null) {
            dataList = Main.Data.Skills.Values.Where(x => x.Level == SkillLevel.Base && x.Type == Parent.Type).ToList();
        }
        else if (Parent.Data.Level != SkillLevel.Rare) {
            string key = Parent.Data.Key.Split('_')[0];
            dataList = Main.Data.Skills.Values.Where(x => x.Key.Contains(key) && x.Level == Parent.Data.Level + 1).ToList();
        }
        else return;
        float parentX = Parent.X;
        float parentY = Parent.Y;

        float parentLineY = parentY - (Parent.Size.y / 2 + VerticalLineLength / 2);
        UI_SkillTreeLine parentLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(this.transform);
        parentLine.Position = new(parentX, parentLineY);
        parentLine.Size = new(PointSize, VerticalLineLength);

        //float pointY = parentLineY - (VerticalLineLength / 2 + PointSize / 2);
        float pointY = -(VerticalLineLength / 2 + PointSize / 2);
        //float childLineY = pointY - (PointSize / 2 + VerticalLineLength / 2);
        float childLineY = -(PointSize / 2 + VerticalLineLength / 2);
        //float slotY = childLineY - (VerticalLineLength / 2 + SlotSize / 2);
        float slotY = -(VerticalLineLength / 2 + SlotSize / 2);

        int count = dataList.Count;
        float x = -(count - 1) / 2 * (SlotSize + HorizontalSpacing);
        List<UI_SkillTreeLine> points = new();
        List<UI_SkillTreeLine> children = new();
        List<UI_SkillTreeLine> horizontals = new();
        for (int i = 0; i < count; i++) {
            UI_SkillTreeLine pointLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(parentLine.transform);
            pointLine.Position = new(x, pointY);
            pointLine.Size = new(PointSize, PointSize);
            points.Add(pointLine);

            UI_SkillTreeLine childLine = Main.UI.CreateSubItem<UI_SkillTreeLine>(pointLine.transform);
            childLine.Position = new(0, childLineY);
            childLine.Size = new(PointSize, VerticalLineLength);
            children.Add(childLine);

            UI_SkillSlot newSlot = Main.UI.CreateSubItem<UI_SkillSlot>(childLine.transform, pooling: true);
            newSlot.SetInfo(dataList[i]);
            newSlot.Position = new Vector2(0, slotY);
            _childrenSlots.Add(newSlot);

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