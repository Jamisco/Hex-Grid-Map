using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LandScapes;

namespace Assets.Scripts
{
    [Serializable]
    internal class Plains : LandScapes, IPlateau
    {
        [SerializeField] protected TileContainer veryHotTiles;
        [SerializeField] protected TileContainer veryColdTiles;
        [SerializeField] protected TileContainer freezingTiles;

        internal override void Instantiate(float hexScale)
        {
            veryHotTiles.Instantiate(hexScale, tileAsset, Temperature.VeryHot);
            hotTiles.Instantiate(hexScale, tileAsset, Temperature.Hot);
            warmTiles.Instantiate(hexScale, tileAsset, Temperature.Warm);
            coldTiles.Instantiate(hexScale, tileAsset, Temperature.Cold);
            veryColdTiles.Instantiate(hexScale, tileAsset, Temperature.VeryCold);
            freezingTiles.Instantiate(hexScale, tileAsset, Temperature.Freezing);     
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
                case Temperature.VeryCold:
                    return veryColdTiles.GetRandomTile();
                case Temperature.Freezing:
                    return freezingTiles.GetRandomTile();
                default:
                    return hotTiles.GetRandomTile();
            }
        }

        private Mountains _mountains;
        private Highlands _highlands;
        private Hills _hills;

        public void SetPlateaus(Mountains mountains, Highlands highlands, Hills hills)
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