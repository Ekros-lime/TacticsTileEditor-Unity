using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        [SerializeField] private MapRoot mapRoot;

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
            //TODO: 清空MapRoot > Tiles
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