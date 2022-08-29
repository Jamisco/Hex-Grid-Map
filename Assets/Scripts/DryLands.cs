using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    [Serializable]
    internal class DryLands : LandScapes, IPlateau
    {
        [SerializeField] protected TileContainer veryHotTiles;

        internal override void Instantiate(float hexScale)
        { 
            veryHotTiles.Instantiate(hexScale, tileAsset, Temperature.VeryHot);
            hotTiles.Instantiate(hexScale, tileAsset, Temperature.Hot);
            warmTiles.Instantiate(hexScale, tileAsset, Temperature.Warm);
            coldTiles.Instantiate(hexScale, tileAsset, Temperature.Cold);
           
        }

        public override LandScapeTile GetRandomTile(Temperature temp)
        {
            switch (temp)
            {
                case Temperature.VeryHot:
                    return veryHotTiles.GetRandomTile();
                case Temperature.Hot:
                    return hotTiles.GetRandomTile();
                case Temperature.Warm:
                    return warmTiles.GetRandomTile();
                case Temperature.Cold:
                    return coldTiles.GetRandomTile();
                default:
                    return warmTiles.GetRandomTile();
            }
        }

        private Mountains _mountains;
        private Highlands _highlands;
        private Hills _hills;

        public virtual void SetPlateaus(Mountains mountains, Highlands highlands, Hills hills)
        {
            _mountains = mountains;
            _highlands = highlands;
            _hills = hills;

        }

        public LandScapeTile GetRandomTile(Temperature temp, HeightLevel height)
        {
            switch (height)
            {
                case HeightLevel.Flat:
                    return GetRandomTile(temp);
                case HeightLevel.Hills:
                    return HillTiles.GetRandomTile(temp);
                case HeightLevel.Highlands:
                    return HighlandTiles.GetRandomTile(temp);
                case HeightLevel.Mountain:
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
