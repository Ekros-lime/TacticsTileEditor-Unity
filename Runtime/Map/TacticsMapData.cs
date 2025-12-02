using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    public enum MapDimension
    {
        ThreeD,
        TwoD
    };
    [System.Serializable]
    [CreateAssetMenu(menuName = "TacticsED/Map Data", fileName = "NewMap")]
    public class TacticsMapData : ScriptableObject
    {
        // x Axis
        public int Width { get => this._width; }
        // z Axis
        public int Length { get => this._length; }
        // y Axis
        public int Height { get => this._height; }
        public MapDimension Dimension { get => this._dimension; }
        public List<TileData> Tiles { get => this._tiles; }

        [SerializeField, Min(0)] private int _width;
        [SerializeField, Min(0)] private int _length;
        [SerializeField, Min(0)] private int _height;
        [SerializeField] private MapDimension _dimension;
        [SerializeField] private List<TileData> _tiles;

        private void OnEnable()
        {
            if (this._tiles == null) this._tiles = new List<TileData>();
        }

        public void InitializeDefault()
        {
            this._width = 10;
            this._length = 10;
            this._height = 10;
            this._dimension = MapDimension.ThreeD;
            this._tiles = new List<TileData>();
        }

#if UNITY_EDITOR
        public void SetAttributes(int width, int length, int height, MapDimension dimension)
        {
            this._width = width;
            this._length = length;
            this._height = height;
            this._dimension = dimension;
        }
#endif
    }
}