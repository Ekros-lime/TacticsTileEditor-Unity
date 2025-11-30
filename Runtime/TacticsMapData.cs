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
    public class TacticsMapData : ScriptableObject
    {
        public int Width 
        { 
            set
            { 
                if (value > 0)
                {
                    this._width = value;
                }
            } 
            get => this._width; 
        }
        public int Length 
        {
            set
            {
                if (value > 0)
                {
                    this._length = value;
                }
            }
            get => this._length; 
        }
        public int Height 
        {
            set 
            {
                if (value > 0)
                {
                    this._height = value;
                }
            }
            get => this._height; 
        }
        public float BlockSize 
        {
            set
            {
                if (value > 0)
                {
                    this._blockSize = value;
                }
            }
            get => this._blockSize;
        }
        public MapDimension Dimension { set => this._dimension = value; get => this._dimension; }
        public List<int> BlockList { set => this._blockList = value; get => this._blockList; }

        [SerializeField] private int _width;
        [SerializeField] private int _length;
        [SerializeField] private int _height;
        [SerializeField] private float _blockSize;
        [SerializeField] private MapDimension _dimension;
        [SerializeField] private List<int> _blockList;

        public void InitializeDefault()
        {
            this.Width = 10;
            this.Length = 10;
            this.Height = 10;
            this.BlockSize = 1f;
            this.Dimension = MapDimension.ThreeD;
            this.BlockList = new List<int>();
        }
    }
}