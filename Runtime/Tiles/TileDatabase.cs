using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    [CreateAssetMenu(menuName = "TacticsED/Tile Database")]
    // 存储Tiles数据，用于获取对应ID的TileBase
    public class TileDatabase : ScriptableObject
    {
        public List<TileBase> TileList = new List<TileBase>();
        private Dictionary<string, TileBase> Tiles;

        public TileBase GetTileBase(string id)
        {
            if (Tiles == null) BuildTileDictionary();
            Tiles.TryGetValue(id, out TileBase tile);
            return tile;
        }

        public GameObject GetTilePrefab(string id)
        {
            TileBase tile = GetTileBase(id);
            if (tile == null) return null;
            return tile.Prefab;
        }

        private void OnEnable()
        {
            BuildTileDictionary();
        }

        private void BuildTileDictionary()
        {
            Tiles = new Dictionary<string, TileBase>();
            foreach (TileBase tile in TileList)
            {
                if (Tiles.ContainsKey(tile.ID)) Debug.LogWarning($"Duplicate Tile ID: {tile.ID}");

                Tiles.Add(tile.ID, tile);
            }
        }
    }
}

