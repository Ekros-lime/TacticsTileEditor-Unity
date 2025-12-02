using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        [SerializeField] private MapRoot mapRoot;

        // for test attributes TODO: 设置 or 自动找到 tileDatabase
        public TileDatabase tileDatabase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // for test
        private void Start()
        {
            // TacticsMapData tempMapData = ScriptableObject.CreateInstance<TacticsMapData>();
            SetMapRoot();
            Debug.Log($"{mapRoot.name}, {mapRoot.transform.position}");
            mapRoot.mapData.Tiles.Clear();
            TileData g1 = new TileData(0, 0, 0, "0");
            TileData s1 = new TileData(1, 0, 1, "1");
            mapRoot.mapData.Tiles.Add(g1);
            mapRoot.mapData.Tiles.Add(s1);
            BuildMapFromMapRoot();
        }

        public void BuildMapFromMapData(TacticsMapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("MapManager: mapData is null.");
                return;
            }

            SetMapRoot();

            //覆盖mapRoot上原先的数据
            mapRoot.mapData = mapData;
            BuildMapFromMapRoot();
        }

        public void BuildMapFromMapRoot()
        {
            SetMapRoot();
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
            foreach(Transform child in mapRoot.tileRoot)
            {
                deleteList.Add(child.gameObject);
            }
            foreach(var go in deleteList)
            {
                GameObject.DestroyImmediate(go);
            }
			// 生成地图 TODO: fit 2D
            foreach(TileData tile in mapRoot.mapData.Tiles)
            {
                // Get position
                Vector3 tempPos = new Vector3(tile.x, tile.y, tile.z);
                // Get Prefab by ID
                GameObject tempGO = GameObject.Instantiate(tileDatabase.GetTilePrefab(tile.ID));
                tempGO.transform.SetParent(mapRoot.tileRoot, false);
                tempGO.transform.position = tempPos;
                tempGO.transform.localScale = Vector3.one;
            }
			//TODO: 优化显示
        }

        private void SetMapRoot()
        {
            if (mapRoot == null) mapRoot = FindObjectOfType<MapRoot>();

            if (mapRoot == null)
            {
                GameObject rootObj = new GameObject("MapRoot");
                var newRoot = rootObj.AddComponent<MapRoot>();

                var tileRootObj = new GameObject("Tiles");
                tileRootObj.transform.SetParent(rootObj.transform, false);
                newRoot.tileRoot = tileRootObj.transform;

                mapRoot = newRoot;
            }
        }
    }
}