using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGenerate {
    public class TileMap {
        public Tilemap Map { get; private set; }

        private Dictionary<string, UnityEngine.Tilemaps.Tile> _tiles;

        public TileMap(string name, Transform parent, Dictionary<string, UnityEngine.Tilemaps.Tile> tiles, int order, bool needCollider) {
            Map = new GameObject(name).GetOrAddComponent<Tilemap>();
            Map.gameObject.GetOrAddComponent<TilemapRenderer>().sortingOrder = order;
            Map.transform.SetParent(parent);
            Map.transform.localPosition = Vector2.zero;
            if (needCollider) {
                Map.gameObject.GetOrAddComponent<TilemapCollider2D>().usedByComposite = true;
                Map.gameObject.GetOrAddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                Map.gameObject.GetOrAddComponent<CompositeCollider2D>();
            }

            this._tiles = tiles;
        }

        public void SetTileset(Dictionary<string, UnityEngine.Tilemaps.Tile> tiles) => this._tiles = tiles;
        public void Clear() {
            Map.ClearAllTiles();
        }

        public void SetTile(int x, int y, string tileName) {
            if (!_tiles.TryGetValue(tileName, out UnityEngine.Tilemaps.Tile tile)) tile = null;

            Map.SetTile(new(x, y), tile);
            //Debug.Log($"[TileMap : {Map.name}] SetTile({x}, {y}, {tileName}): tile = {tile}");
        }
        
    }
}