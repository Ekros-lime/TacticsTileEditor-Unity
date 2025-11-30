using UnityEngine;
using UnityEditor;
using TacticsRPGEkros.Game;
using System;
using Codice.Client.BaseCommands;

namespace TacticsRPGEkros.Editor{
    public class TileEditorWindow : EditorWindow
    {
        private TacticsMapData currentMap;

        private int tempWidth;
        private int tempLength;
        private int tempHeight;
        private float tempBlockSize;
        private MapDimension tempDimension;

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
                EditorGUILayout.Space();
            }

            //TODO: button
            if (GUILayout.Button("Create New Map", GUILayout.Height(24)))
            {
                // Debug.Log("Create New Map Clicked");
                OnClickCreateNewMap();
            }

            if(GUILayout.Button("Load Map Asset", GUILayout.Height(24)))
            {
                Debug.Log("Load Map Asset Clicked");
            }

            if(GUILayout.Button("Rebuild Preview", GUILayout.Height(24)))
            {
                Debug.Log("Rebuild Preview Clicked");
            }

            EditorGUILayout.EndVertical();
        }

        private void OnClickApplyChangesOnMap()
        {
            if (currentMap == null) return;

            Undo.RecordObject(currentMap, "Apply Map Settings");

            currentMap.Width = tempWidth;
            currentMap.Length = tempLength;
            currentMap.Height = tempHeight;
            currentMap.BlockSize = tempBlockSize;
            currentMap.Dimension = tempDimension;

            EditorUtility.SetDirty(currentMap);
            AssetDatabase.SaveAssets();
        }

        private void OnClickCreateNewMap()
        {
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

            UpdateDataFromCurrentMap();
            Selection.activeObject = currentMap;
            EditorGUIUtility.PingObject(currentMap);
        }

        private void UpdateDataFromCurrentMap()
        {
            if (currentMap == null) return;
            tempWidth = currentMap.Width;
            tempLength = currentMap.Length;
            tempHeight = currentMap.Height;
            tempBlockSize = currentMap.BlockSize;
            tempDimension = currentMap.Dimension;
        }
    }
}