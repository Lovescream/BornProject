using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {
    Empty,
    Border,
    Edge,
    Wall,
    Floor,
}

[CreateAssetMenu]
public class DTile : RuleTile<DTile.Neighbour> {

    public TileType type;

    public class Neighbour : TilingRuleOutput.Neighbor {
        public const int NeedBorder = 3;
        public const int NotBorderEdgeWall = 4;
        public const int NeedWall = 5;
        public const int NeedWallOrEdge = 6;
        public const int ThisOrEmpty = 7;
        public const int BorderOrEmpty = 8;
    }

    public override bool RuleMatch(int neighbour, TileBase tile) {
        DTile dTile = (tile as DTile);
        switch (neighbour) {
            case Neighbour.NeedBorder:
                if (dTile == null) return false;
                return dTile.type == TileType.Border;
            case Neighbour.NotBorderEdgeWall:
                if (dTile == null) return true;
                return dTile.type != TileType.Border && dTile.type != TileType.Edge && dTile.type != TileType.Wall;
            case Neighbour.NeedWall:
                if (dTile == null) return false;
                return dTile.type == TileType.Wall;
            case Neighbour.NeedWallOrEdge:
                if (dTile == null) return false;
                return dTile.type == TileType.Edge || dTile.type == TileType.Wall;
            case Neighbour.ThisOrEmpty:
                if (dTile == null) return false;
                return dTile.type == TileType.Empty || tile == this;
            case Neighbour.BorderOrEmpty:
                if (dTile == null) return false;
                return dTile.type == TileType.Border || dTile.type == TileType.Empty;
        }
        return base.RuleMatch(neighbour, tile);
    }







    //public override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform) {
    //    if (RuleMatches(rule, position, tilemap, 0)) {
    //        transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
    //        return true;
    //    }

    //    // Check rule against rotations of 0, 90, 180, 270
    //    if (rule.m_RuleTransform == TilingRuleOutput.Transform.Rotated) {
    //        for (int angle = m_RotationAngle; angle < 360; angle += m_RotationAngle) {
    //            if (RuleMatches(rule, position, tilemap, angle)) {
    //                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
    //                return true;
    //            }
    //        }
    //    }
    //    // Check rule against x-axis, y-axis mirror
    //    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorXY) {
    //        if (RuleMatches(rule, position, tilemap, true, true)) {
    //            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, -1f, 1f));
    //            return true;
    //        }
    //        if (RuleMatches(rule, position, tilemap, true, false)) {
    //            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
    //            return true;
    //        }
    //        if (RuleMatches(rule, position, tilemap, false, true)) {
    //            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
    //            return true;
    //        }
    //    }
    //    // Check rule against x-axis mirror
    //    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorX) {
    //        if (RuleMatches(rule, position, tilemap, true, false)) {
    //            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
    //            return true;
    //        }
    //    }
    //    // Check rule against y-axis mirror
    //    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorY) {
    //        if (RuleMatches(rule, position, tilemap, false, true)) {
    //            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
    //            return true;
    //        }
    //    }
    //    // Check rule against x-axis mirror with rotations of 0, 90, 180, 270
    //    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.RotatedMirror) {
    //        for (int angle = 0; angle < 360; angle += m_RotationAngle) {
    //            if (angle != 0 && RuleMatches(rule, position, tilemap, angle)) {
    //                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
    //                return true;
    //            }
    //            if (RuleMatches(rule, position, tilemap, angle, true)) {
    //                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), new Vector3(-1f, 1f, 1f));
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}
    //private bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle, bool mirrorX = false) {
    //    var minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
    //    for (int i = 0; i < minCount; i++) {
    //        var neighbor = rule.m_Neighbors[i];
    //        var neighborPosition = rule.m_NeighborPositions[i];
    //        if (mirrorX)
    //            neighborPosition = GetMirroredPosition(neighborPosition, true, false);
    //        var positionOffset = GetRotatedPosition(neighborPosition, angle);
    //        var other = tilemap.GetTile(GetOffsetPosition(position, positionOffset));
    //        if (!RuleMatch(neighbor, other)) {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //private bool Matches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle, bool mirrorX = false) {
    //    var minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
    //    for (int i=0;i < minCount; i++) {
    //        int neighbour = rule.m_Neighbors[i];
    //        Vector3Int neighbourPosition = rule.m_NeighborPositions[i];
    //        if (mirrorX) neighbourPosition = GetMirroredPosition(neighbourPosition, true, false);
    //        Vector3Int positionOffset = GetRotatedPosition(neighbourPosition, angle);

    //        TileBase other = tilemap.GetTile(GetOffsetPosition(position, positionOffset));
    //        if (!RuleMatch(neighbour, other)) return false;
    //    }
    //    return true;
    //}
    //private bool Match(int neighbour, ITilemap tilemap, Vector3Int position) {
    //    tilemap.
    //}
}