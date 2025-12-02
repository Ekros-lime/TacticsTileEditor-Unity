using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    [System.Serializable]
    public class TileData
    {
        public int x;
        public int y;
        public int z;
        public string ID;

        public TileData(int x, int y, int z, string iD)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            ID = iD;
        }
    }
}