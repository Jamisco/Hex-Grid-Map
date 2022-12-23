using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    internal class Lake : LandScapes
    {
        [SerializeField] protected TileContainer veryColdTiles;
        [SerializeField] protected TileContainer freezingTiles;

        internal override void Instantiate(float hexScale)
        {
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
    }
}
