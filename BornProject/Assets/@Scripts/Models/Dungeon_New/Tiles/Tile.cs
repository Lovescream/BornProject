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

        public override bool RuleMatch(int neighbour, TileBase tile) {
            Tile dTile = (tile as Tile);
            switch (neighbour) {
                case Neighbour.Border:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Border;
                case Neighbour.Wall:
                    if (dTile == null) return true;
                    return dTile.type != TileType.Border && dTile.type != TileType.Edge && dTile.type != TileType.Wall;
                case Neighbour.Edge:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Wall;
                case Neighbour.EmptyOrThis:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Edge || dTile.type == TileType.Wall;
                case Neighbour.EmptyOrBorder:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Empty || tile == this;
                case Neighbour.WallOrEdge:
                    if (dTile == null) return false;
                    return dTile.type == TileType.Border || dTile.type == TileType.Empty;
                case Neighbour.NotBorderNorEdgeNorWall:
                    if (dTile == null) return false;
                    return dTile.type != TileType.Border && dTile.type != TileType.Edge && dTile.type != TileType.Wall;
            }
            return base.RuleMatch(neighbour, tile);
        }

    }
}