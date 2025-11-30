using UnityEngine;
using UnityEditor;
using TacticsRPGEkros.Game;
using System;
using Codice.Client.BaseCommands;
using System.Runtime.CompilerServices;
using Codice.CM.Common;
using UnityEditor.TerrainTools;

namespace TacticsRPGEkros.Editor{
    public class TileEditorWindow : EditorWindow
    {
        private TacticsMapData currentMap;

        private int tempWidth;
        private int tempLength;
        private int tempHeight;
        private float tempBlockSize;
        private MapDimension tempDimension;

        private bool editMode = false;

        [MenuItem("Tactics Tools -Ekros/Tile Editor")]

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void Open()
        {
            EditorWindow.GetWindow<TileEditorWindow>("Tile Editor");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Tactics Tile Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Map Control", EditorStyles.boldLabel);

            var tempMap = (TacticsMapData)EditorGUILayout.ObjectField(
                "Current Map",
                currentMap,
                typeof(TacticsMapData),
                false);

            if (tempMap != currentMap)
            {
                currentMap = tempMap;
                UpdateDataFromCurrentMap();
            }

            if (tempMap != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);

                tempWidth = EditorGUILayout.IntField("Width", tempWidth);
                tempLength = EditorGUILayout.IntField("Length", tempLength);
                tempHeight = EditorGUILayout.IntField("Height", tempHeight);
                tempBlockSize = EditorGUILayout.FloatField("Block Size", tempBlockSize);
                tempDimension = (MapDimension)EditorGUILayout.EnumPopup("Dimension", tempDimension);

                if (GUILayout.Button("Apply", GUILayout.Height(24), GUILayout.Width(72)))
                {
                    OnClickApplyChangesOnMap();
                }

                //Edit Mode
                EditorGUILayout.BeginHorizontal();

                if(GUILayout.Button("Edit", GUILayout.Height(24), GUILayout.Width(72)))
                {
                    editMode = true;
                }

                if(GUILayout.Button("Save", GUILayout.Height(24), GUILayout.Width(72)))
                {
                    editMode = false;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            //TODO: button
            if (GUILayout.Button("Create New Map", GUILayout.Height(24)))
            {
                // Debug.Log("Create New Map Clicked");
                OnClickCreateNewMap();
            }

            if (GUILayout.Button("Load Map Asset", GUILayout.Height(24)))
            {
                Debug.Log("Load Map Asset Clicked");
            }

            if (GUILayout.Button("Rebuild Preview", GUILayout.Height(24)))
            {
                //Debug.Log("Rebuild Preview Clicked");
                OnClickRebuildPreview();
            }

            EditorGUILayout.EndVertical();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI; 
        }

        // ==========================================

        private void OnClickApplyChangesOnMap()
        {
            if (currentMap == null) return;

            Undo.RecordObject(currentMap, "Apply Map Settings");

            currentMap.Width = tempWidth;
            currentMap.Length = tempLength;
            currentMap.Height = tempHeight;
            currentMap.BlockSize = tempBlockSize;
            currentMap.Dimension = tempDimension;

            //保存
            EditorUtility.SetDirty(currentMap);
            AssetDatabase.SaveAssets();
        }

        private void OnClickCreateNewMap()
        {
            // 选择路径
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Map",
                "New Map",
                "asset",
                "Choose a location to save the new map asset");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            Debug.Log("New map will be saved at: " + path);
            var map = ScriptableObject.CreateInstance<TacticsMapData>();
            map.InitializeDefault();

            AssetDatabase.CreateAsset(map, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentMap = map;

            //新建的地图数据同步到GUI面板上
            UpdateDataFromCurrentMap();

            Selection.activeObject = currentMap;
            EditorGUIUtility.PingObject(currentMap);
        }

        private void OnClickRebuildPreview()
        {
            if (currentMap == null)
            {
                Debug.Log("No selected MapData");
                return;
            }
            var mapRoot = GetMapRoot();
            ClearPreviewTiles(mapRoot);
            BuildPreviewTiles(mapRoot);
        }

        // 把当前选中的MapData信息同步到GUI面板中显示
        private void UpdateDataFromCurrentMap()
        {
            if (currentMap == null) return;
            tempWidth = currentMap.Width;
            tempLength = currentMap.Length;
            tempHeight = currentMap.Height;
            tempBlockSize = currentMap.BlockSize;
            tempDimension = currentMap.Dimension;
        }
        // 在Hierarchy中找到MapRoot == currentMap并返回，如果没有则创建一个
        private MapRoot GetMapRoot()
        {
            if (currentMap == null)
            {
                return null;
            }

            //找到所有的MapRoot
            var roots = GameObject.FindObjectsOfType<MapRoot>();
            
            foreach (var root in roots)
            {
                if (root.mapData == currentMap)
                {
                    return root;
                }
            }

            GameObject rootObj = new GameObject("MapRoot_" + currentMap.name);
            var newRoot = rootObj.AddComponent<MapRoot>();
            newRoot.mapData = currentMap;

            var tileRootObj = new GameObject("Tiles");
            tileRootObj.transform.SetParent(rootObj.transform, false);
            newRoot.tileRoot = tileRootObj.transform;

            EditorUtility.SetDirty(newRoot);

            return newRoot;
        }
        // 清除MapRoot>TileRoot下的GO,如果没有TileRoot则清除MapRoot下的GO
        private void ClearPreviewTiles(MapRoot root)
        {
            if (root == null) return;

            Transform tileRoot = root.tileRoot == null ? root.transform : root.tileRoot;

            var deleteList = new System.Collections.Generic.List<GameObject>();
            foreach(Transform child in tileRoot)
            {
                deleteList.Add(child.gameObject);
            }

            foreach (var go in deleteList)
            {
                Undo.DestroyObjectImmediate(go);
            }
        }
        // 根据MapData创建Cube TODO:做更多选择
        private void BuildPreviewTiles(MapRoot root)
        {
            if (root == null || root.mapData == null) return;

            var mapData = root.mapData;

            int width = mapData.Width;
            int length = mapData.Length;
            int height = mapData.Height;
            float blockSize = mapData.BlockSize;

            Transform tileRoot = root.tileRoot == null ? root.transform : root.tileRoot;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        tile.name = $"Tile_({x},{y},{z})";
                        tile.transform.SetParent(tileRoot, false);

                        float worldX = x * blockSize;
                        float worldY = y * blockSize;
                        float worldZ = z * blockSize;

                        tile.transform.position = new Vector3(worldX, worldY, worldZ);
                        tile.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

                        var collider = tile.GetComponent<Collider>();
                        if (collider != null)
                        {
                            GameObject.DestroyImmediate(collider);
                        }
                    }
                }
            }
        }
        // 画线用
        private void OnSceneGUI(SceneView view)
        {
            if (!editMode) return;
            if (currentMap == null) return;

            int tempX = currentMap.Width;
            int tempY = currentMap.Height;
            int tempZ = currentMap.Length;

            Handles.color = Color.red;
            Vector3 from = Vector3.zero;
            Vector3 to = new Vector3(tempX, 0, 0);
            for (int z = 0; z <= tempZ; z++)
            {
                for(int y = 0; y <= tempY; y++)
                {
                    from = new Vector3(0, y, z);
                    to = new Vector3(tempX, y, z);
                    Handles.DrawLine(from, to);
                }
            }
            Handles.color = Color.blue;
            for (int x = 0; x <= tempX; x++)
            {
                for (int y = 0; y <= tempY; y++)
                {
                    from = new Vector3(x, y, 0);
                    to = new Vector3(x, y, tempZ);
                    Handles.DrawLine(from, to);
                }
            }
            Handles.color = Color.green;
            for (int x = 0; x <= tempX; x++)
            {
                for (int z = 0; z <= tempZ; z++)
                {
                    from = new Vector3(x, 0, z);
                    to = new Vector3(x, tempY, z);
                    Handles.DrawLine(from, to);
                }
            }

            //Vector3 from = Vector3.zero;
            //Vector3 to = new Vector3(5, 0, 0);
            //Handles.DrawLine(from, to);
        }
    }
}