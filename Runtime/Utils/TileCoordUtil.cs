using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    public static class TileCoordUtil
    {
        private static Vector3 offsetP2TC = new Vector3 (0, -0.5f, 0);
        private static Vector3 offsetP2W = new Vector3(0.5f, 0.5f, 0.5f);
        
        // for 3D tiles ==> temp TODO: fit 2D
        // trans pos of pivot to top center
        // using when visit top center pos of tiles (for gaming)
        public static Vector3 TilePovitToTopCenter(Vector3 pos)
        {
            pos += offsetP2TC;
            return pos;
        }
        // trans pivot of piovt to world (corner) 
        // using when generating tiles in world
        public static Vector3 TilePivotToWorld(Vector3 pos)
        {
            pos += offsetP2W;
            return pos;
        }
    }
}

