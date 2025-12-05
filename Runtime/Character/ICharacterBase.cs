using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TacticsRPGEkros.Game
{
    // needed if use GameUtil
    public interface ICharacterBase
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public int MoveRange { get; set; }

        public int AttackRangeMin { get; set; }
        public int AttackRangeMax { get; set; }
    }
}