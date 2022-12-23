using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    [Serializable]
    internal class WetLands : LandScapes, IPlateau
    {

        private Mountains _mountains;
        private Highlands _highlands;
        private Hills _hills;

        public virtual void SetPlateaus(Mountains mountains, Highlands highlands, Hills hills)
        {
            _mountains = mountains;
            _highlands = highlands;
            _hills = hills;

        }

        public LandScapeTile GetRandomTile(Temperature temp, GroundLevel height)
        {
            switch (height)
            {
                case GroundLevel.Flat:
                    return GetRandomTile(temp);
                case GroundLevel.Hills:
                    return HillTiles.GetRandomTile(temp);
                case GroundLevel.Highlands:
                    return HighlandTiles.GetRandomTile(temp);
                case GroundLevel.Mountain:
                    return MountainTiles.GetRandomTile(temp);
                default:
                    return GetRandomTile(temp);
            }
        }

        public Mountains MountainTiles => _mountains;

        public Highlands HighlandTiles => _highlands;

        public Hills HillTiles => _hills;
    }
}
