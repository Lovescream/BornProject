using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ZerolizeDungeon {

    public enum TileType {
        Empty,
        Border,
        Edge,
        Wall,
        Floor,
    }

    [CreateAssetMenu(fileName = "DungeonTile", menuName = "Dungeon/DungeonTile")]
    public class Tile : RuleTile {

        public override Type m_NeighborType => typeof(Neighbour);

        public TileType type;

        public class Neighbour : TilingRuleOutput.Neighbor {
            public const int Border = 3;
            public const int Wall = 4;
            public const int Edge = 5;
            public const int EmptyOrThis = 6;
            public const int EmptyOrBorder = 7;
            public const int WallOrEdge = 8;
            public const int NotBorderNorEdgeNorWall = 9;
        }
        public static readonly string[] RuleDescription = {
            "",
            "완전히 일치하는 타일",
            "일치하지 않는 타일",
            "벽의 안쪽 (까만 부분)",
            "벽",
            "벽의 바깥쪽 (가장자리)",
            "빈 공간 또는 완전히 일치하는 타일",
            "빈 공간 또는 벽의 안 쪽 (까만 부분)",
            "벽 또는 벽의 바깥쪽 (가장자리)",
            "벽의 안쪽도 바깥쪽도 벽도 아님",
        };

        public override bool RuleMatch(int neighbour, TileBase tile) {
            Tile dTile = (tile as Tile);
            switch (neighbour) {
                case Neighbour.Border:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Border;
                case Neighbour.Wall:
                    if (dTile == null) return true;
                    return dTile.type == TileType.Wall;
                case Neighbour.Edge:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Edge;
                case Neighbour.EmptyOrThis:
                    if (dTile == null) return true;
                    return dTile.type == TileType.Empty || tile == this;
                case Neighbour.EmptyOrBorder:
                    if (dTile == null) return true;
                    return dTile.type == TileType.Border || dTile.type == TileType.Empty;
                case Neighbour.WallOrEdge:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Wall || dTile.type == TileType.Edge;
                case Neighbour.NotBorderNorEdgeNorWall:
                    if (dTile == null) return false;
                    return dTile.type != TileType.Border && dTile.type != TileType.Edge && dTile.type != TileType.Wall;
            }
            return base.RuleMatch(neighbour, tile);
        }

    }
}