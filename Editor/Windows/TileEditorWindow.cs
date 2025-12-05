using UnityEngine;
using UnityEditor;
using TacticsRPGEkros.Game;
using System;
using Codice.Client.BaseCommands;
using System.Runtime.CompilerServices;
using Codice.CM.Common;
using UnityEditor.TerrainTools;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TacticsRPGEkros.Editor{

    // TODO: TileDatabase null检测
    public class TileEditorWindow : EditorWindow
    {
        // for edit mode
        private TacticsMapData currentMap;
        private TacticsMapData copyMap;

        private TileDatabase currentTileDatabase;
        private MapRoot editMapRoot;
        private MapRoot mapRoot;

        private int tempWidth;
        private int tempLength;
        private int tempHeight;
        private MapDimension tempDimension;

        // is edit mode?
        private bool editMode = false;
        private int editLevel;

        // edit mode下选中的格子的tile position
        private Vector3 currentTilePos;
        // 当前选中的TileIndex
        private int selectedTileIndex;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [MenuItem("Tactics Tools -Ekros/Tile Editor")]
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

            // 放入map数据
            var tempMap = (TacticsMapData)EditorGUILayout.ObjectField(
                "Current Map",
                currentMap,
                typeof(TacticsMapData),
                false);

            // 放入tiledatabase数据
            var tempTileDatabase = (TileDatabase)EditorGUILayout.ObjectField(
                "Current Tile Database",
                currentTileDatabase,
                typeof(TileDatabase),
                false);

            if (tempMap != currentMap)
            {
                currentMap = tempMap;
                UpdateDataFromCurrentMap();
            }

            if (tempTileDatabase != currentTileDatabase)
            {
                currentTileDatabase = tempTileDatabase;
            }

            // in edit
            // 隐藏MapRoot
            // Set MapRoot_Edit
            if (tempMap != null && tempTileDatabase != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);

                tempWidth = EditorGUILayout.IntField("Width", tempWidth);
                tempLength = EditorGUILayout.IntField("Length", tempLength);
                tempHeight = EditorGUILayout.IntField("Height", tempHeight);
                tempDimension = (MapDimension)EditorGUILayout.EnumPopup("Dimension", tempDimension);

                if (GUILayout.Button("Apply", GUILayout.Height(24), GUILayout.Width(72)))
                {
                    OnClickApplyChangesOnMap();
                }

                //Edit Mode

                if(GUILayout.Button("Edit", GUILayout.Height(24), GUILayout.Width(72)))
                {
                    // set edit level = 0 then start to edit map
                    OnEditStart();
                    editLevel = 0;
                    editMode = true;
                }

                if (editMode)
                {
                    TilePanel();
                    if (selectedTileIndex >= 0)
                    {
                        var tempTile = currentTileDatabase.TileList[selectedTileIndex];
                        EditorGUILayout.LabelField("Selected Tile：", tempTile.name);
                    }

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Upper Level", GUILayout.Height(24), GUILayout.Width(108)))
                    {
                        if (editLevel < tempHeight - 1) editLevel++;
                        SceneView.RepaintAll();
                    }

                    if (GUILayout.Button("Lower Level", GUILayout.Height(24), GUILayout.Width(108)))
                    {
                        if(editLevel > 0) editLevel--;
                        SceneView.RepaintAll();
                    }

                    if (GUILayout.Button("Save？暂时没用", GUILayout.Height(24), GUILayout.Width(72)))
                    {
                        OnEditEnd();
                        editMode = false;
                        SceneView.RepaintAll();
                    }

                    if (GUILayout.Button("Export Map Data", GUILayout.Height(24), GUILayout.Width(144)))
                    {
                        
                    }

                    EditorGUILayout.EndHorizontal();
                }

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
            OnEditEnd();
            SceneView.duringSceneGui -= OnSceneGUI; 
        }

        // ==========================================

        private void OnClickApplyChangesOnMap()
        {
            if (currentMap == null) return;

            Undo.RecordObject(currentMap, "Apply Map Settings");

            currentMap.SetAttributes(tempWidth, tempLength, tempHeight, tempDimension);

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
        // 暂时废弃
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
            tempDimension = currentMap.Dimension;
        }
        // 在Hierarchy中找到MapRoot.MapData == currentMap并返回，如果没有则创建一个
        private MapRoot GetMapRoot()
        {
            //if (currentMap == null)
            //{
            //    return null;
            //}

            ////找到所有的MapRoot
            //var roots = GameObject.FindObjectsOfType<MapRoot>();

            //foreach (var root in roots)
            //{
            //    if (root.mapData == currentMap)
            //    {
            //        return root;
            //    }
            //}

            if (mapRoot == null)
            {
                var mapRootGO = GameObject.Find("MapRoot");
                if (mapRootGO != null)
                {
                    mapRoot = mapRootGO.GetComponent<MapRoot>();
                }
                else
                {
                    GameObject rootObj = new GameObject("MapRoot");
                    var newRoot = rootObj.AddComponent<MapRoot>();
                    newRoot.mapData = currentMap;

                    var tileRootObj = new GameObject("Tiles");
                    tileRootObj.transform.SetParent(rootObj.transform, false);
                    newRoot.tileRoot = tileRootObj.transform;

                    EditorUtility.SetDirty(newRoot);

                    mapRoot = newRoot;
                }
            }
            //if(mapRoot == null)
            //{
            //    GameObject rootObj = new GameObject("MapRoot");
            //    var newRoot = rootObj.AddComponent<MapRoot>();
            //    newRoot.mapData = currentMap;

            //    var tileRootObj = new GameObject("Tiles");
            //    tileRootObj.transform.SetParent(rootObj.transform, false);
            //    newRoot.tileRoot = tileRootObj.transform;

            //    EditorUtility.SetDirty(newRoot);

            //    mapRoot = newRoot;
            //}

            return mapRoot;
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

                        float worldX = x * 1;
                        float worldY = y * 1;
                        float worldZ = z * 1;

                        tile.transform.position = new Vector3(worldX, worldY, worldZ);
                        tile.transform.localScale = Vector3.one;

                        var collider = tile.GetComponent<Collider>();
                        if (collider != null)
                        {
                            GameObject.DestroyImmediate(collider);
                        }
                    }
                }
            }
        }
        // 画线用 editmode
        private void OnSceneGUI(SceneView view)
        {
            GenerateGrid();
            GenerateHover();
        }
        // 隐藏MapRoot并Set MapRoot_Edit Build Map
        private void OnEditStart()
        {
            MapRoot tempRoot = GetMapRoot();
            tempRoot.gameObject.SetActive(false);
            GetEditMapRoot();
            MapBuilder.BuildMap(editMapRoot, currentTileDatabase);
        }
        private void OnEditEnd()
        {
            ClearEditMapRoot();
            if (mapRoot != null)
            {
                mapRoot.gameObject.SetActive(true);
                mapRoot = null;
            }
        }

        // 在Hierarchy创建名为MapRoot_Edit的组件并返回, set editMapRoot = newRoot
        private MapRoot GetEditMapRoot()
        {
            ClearEditMapRoot();
            GameObject rootObj = new GameObject("MapRoot_Edit");
            var newRoot = rootObj.AddComponent<MapRoot>();

            newRoot.mapData = currentMap;
            // TODO: copy currentMap, do not modify currentMap when in edit mode
            // copyMap = ScriptableObject.CreateInstance<TacticsMapData>();
            // copyMap < currentMap; newRoot.mapData = currentMap; when edit end: currentMap < copyMap

            var tileRootObj = new GameObject("Tiles");
            tileRootObj.transform.SetParent(rootObj.transform, false);
            newRoot.tileRoot = tileRootObj.transform;

            EditorUtility.SetDirty(newRoot);

            editMapRoot = newRoot;

            return newRoot;
        }
        // Delete MapRoot_Edit
        private void ClearEditMapRoot()
        {
            if (editMapRoot != null)
            {
                GameObject.DestroyImmediate(editMapRoot.gameObject);
                editMapRoot = null;
            }
        }
        // Generate Grid
        private void GenerateGrid()
        {
            if (!editMode) return;
            if (currentMap == null) return;

            // preprocess
            if (editLevel >= tempHeight) editLevel = tempHeight - 1;
            if (editLevel < 0) editLevel = 0;

            int tempX = currentMap.Width;
            int tempY = currentMap.Height;
            int tempZ = currentMap.Length;

            // draw X
            Handles.color = Color.red;
            Vector3 from = Vector3.zero;
            Vector3 to = new Vector3(tempX, 0, 0);
            /*
            for (int z = 0; z <= tempZ; z++)
            {
                for(int y = 0; y <= tempY; y++)
                {
                    from = new Vector3(0, y, z);
                    to = new Vector3(tempX, y, z);
                    Handles.DrawLine(from, to);
                }
            }
            */
            for (int z = 0; z <= tempZ; z++)
            {
                from = new Vector3(0, editLevel + 1, z);
                to = new Vector3(tempX, editLevel + 1, z);
                Handles.DrawLine(from, to);
            }
            // draw Z
            Handles.color = Color.blue;
            /*
            for (int x = 0; x <= tempX; x++)
            {
                for (int y = 0; y <= tempY; y++)
                {
                    from = new Vector3(x, y, 0);
                    to = new Vector3(x, y, tempZ);
                    Handles.DrawLine(from, to);
                }
            }
            */
            for (int x = 0; x <= tempX; x++)
            {
                from = new Vector3(x, editLevel + 1, 0);
                to = new Vector3(x, editLevel + 1, tempZ);
                Handles.DrawLine(from, to);
            }
            // draw Y
            Handles.color = Color.green;
            /*
            for (int x = 0; x <= tempX; x++)
            {
                for (int z = 0; z <= tempZ; z++)
                {
                    from = new Vector3(x, 0, z);
                    to = new Vector3(x, tempY, z);
                    Handles.DrawLine(from, to);
                }
            }
            */
            for (int x = 0; x <= tempX; x++)
            {
                for (int z = 0; z <= tempZ; z++)
                {
                    from = new Vector3(x, 0, z);
                    to = new Vector3(x, editLevel + 1, z);
                    Handles.DrawLine(from, to);
                }
            }

            //Vector3 from = Vector3.zero;
            //Vector3 to = new Vector3(5, 0, 0);
            //Handles.DrawLine(from, to);
        }
        // Ray & Hover
        private void GenerateHover()
        {
            if (!editMode) return;
            if (currentMap == null) return;

            if (Event.current.type == EventType.MouseMove ||
                Event.current.type == EventType.MouseDrag ||
                Event.current.type == EventType.Layout)
            {
                SceneView.RepaintAll();
            }

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            int y = editLevel + 1;

            if (Mathf.Abs(ray.direction.y) < 0.0001f) return;

            float t = (y - ray.origin.y) / ray.direction.y;
            if (t < 0) return;

            Vector3 hitPos = ray.origin + ray.direction * t;

            int x = Mathf.FloorToInt(hitPos.x);
            int z = Mathf.FloorToInt(hitPos.z);

            // Debug.Log($"{x},{z}");

            //hight light (in range)
            if(x >= 0 && x < tempWidth && z >= 0 && z < tempLength)
            {
                Vector3 p0 = new Vector3(x, y, z);
                Vector3 p1 = new Vector3(x, y, z + 1);
                Vector3 p2 = new Vector3(x + 1, y, z + 1);
                Vector3 p3 = new Vector3(x + 1, y, z);

                currentTilePos = p0;

                Handles.DrawSolidRectangleWithOutline(
                    new Vector3[] { p0, p1, p2, p3 },
                    new Color(1f, 0f, 1f, 0.25f),
                    Color.yellow
                );

                // mouse click event
                if (e.type == EventType.MouseDown)
                {
                    // left
                    if (e.button == 0)
                    {
                        // TODO：mapData.Tiles null 检测
                        for (int i = 0; i < editMapRoot.mapData.Tiles.Count; i++)
                        {
                            TileData tempTileData = editMapRoot.mapData.Tiles[i];
                            if (tempTileData.x == x && tempTileData.y == y - 1 && tempTileData.z == z)
                            {
                                // 找到xyz和当前位置一样的tile
                                editMapRoot.mapData.Tiles.RemoveAt(i);
                            }
                        }
                        editMapRoot.mapData.Tiles.Add(new TileData(x, y - 1, z, currentTileDatabase.TileList[selectedTileIndex].ID));
                        // TODO: 后面再优化
                        OnEditStart();
                        // Debug.Log($"{x},{y},{z}");
                    }
                    // right
                    else if (e.button == 1)
                    {
                        for (int i = 0; i < editMapRoot.mapData.Tiles.Count; i++)
                        {
                            TileData tempTileData = editMapRoot.mapData.Tiles[i];
                            if (tempTileData.x == x && tempTileData.y == y - 1 && tempTileData.z == z)
                            {
                                // 找到xyz和当前位置一样的tile
                                editMapRoot.mapData.Tiles.RemoveAt(i);
                            }
                        }
                        // TODO: 后面再优化
                        OnEditStart();
                        // Debug.Log($"{x},{y},{z}");
                    }
                }
            }


        }
        // Tiles select panel
        private void TilePanel()
        {
            Vector2 tileListScroll = new Vector2(48, 24);
            EditorGUILayout.LabelField("Tile List:", EditorStyles.boldLabel);

            tileListScroll = EditorGUILayout.BeginScrollView(tileListScroll, GUILayout.Height(200));

            for (int i = 0; i < currentTileDatabase.TileList.Count; i++)
            {
                TileBase tile = currentTileDatabase.TileList[i];

                Rect rect = EditorGUILayout.BeginHorizontal();

                // 判断是否选中
                bool isSelected = (i == selectedTileIndex);

                // 背景高亮
                if (isSelected)
                {
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.5f, 1f, 0.3f));
                }

                // 图标
                // GUILayout.Label(iconTex, GUILayout.Width(40), GUILayout.Height(40));

                // 名字按钮
                if (GUILayout.Button(tile.DisplayName, GUILayout.Height(24)))
                {
                    selectedTileIndex = i;
                    Repaint();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}