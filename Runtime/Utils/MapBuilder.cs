using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    public static class MapBuilder
    {
        public static void BuildMap(MapRoot mapRoot, TileDatabase tileDatabase)
        {
            if (mapRoot == null)
            {
                Debug.LogError("MapManager: MapRoot not set.");
                return;
            }
            if (mapRoot.mapData == null)
            {
                Debug.LogError("MapManager: mapData is null.");
                return;
            }
            //TODO: 根据mapRoot.mapData 在mapRoot.tileRoot下生成地图
            // 清空MapRoot > Tiles
            var deleteList = new List<GameObject>();
            foreach (Transform child in mapRoot.tileRoot)
            {
                deleteList.Add(child.gameObject);
            }
            foreach (var go in deleteList)
            {
                GameObject.DestroyImmediate(go);
            }
            // 生成地图 TODO: fit 2D
            foreach (TileData tile in mapRoot.mapData.Tiles)
            {
                // Get position
                Vector3 tempPos = new Vector3(tile.x, tile.y, tile.z);
                tempPos = TileCoordUtil.TilePivotToWorld(tempPos);
                // Get Prefab by ID
                GameObject tempGO = GameObject.Instantiate(tileDatabase.GetTilePrefab(tile.ID));
                tempGO.transform.SetParent(mapRoot.tileRoot, false);
                tempGO.transform.position = tempPos;
                tempGO.transform.localScale = Vector3.one;
            }
            //TODO: 优化显示
        }

        //public static void ClearMap(MapRoot mapRoot)
        //{

        //}
    }
}