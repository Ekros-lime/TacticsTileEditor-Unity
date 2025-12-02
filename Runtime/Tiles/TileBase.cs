using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "TacticsED/Tile Base", fileName = "NewTile")]
    public class TileBase : ScriptableObject
    {
        public string DisplayName;
        public string ID { get => this._id; }
        public MapDimension Dimension { get => this._dimension; }

        public int MoveCost { get => this._moveCost; }

        [SerializeField] private string _id;
        [SerializeField] private int _moveCost;
        [SerializeField] private MapDimension _dimension;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Object _prefab;
    }
}