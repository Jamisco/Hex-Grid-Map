using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LandScapes;
using static GridManager;

namespace Assets.Scripts
{
    [Serializable]
    internal class Desert : LandScapes
    {
        [SerializeField] protected TileContainer veryHotTiles;

        [SerializeField] protected TileContainer desertHills;
        [SerializeField] protected TileContainer desertHighLands;
        [SerializeField] protected TileContainer desertMountains;


        public override void Instantiate(float hexScale)
        {
            veryHotTiles.Instantiate(hexScale, tileAsset, Temperature.VeryHot);
            hotTiles.Instantiate(hexScale, tileAsset, Temperature.Hot);
            warmTiles.Instantiate(hexScale, tileAsset, Temperature.Warm);
            coldTiles.Instantiate(hexScale, tileAsset, Temperature.Cold);

            desertHills.Instantiate(hexScale, tileAsset, Temperature.Hot);
            desertHighLands.Instantiate(hexScale, tileAsset, Temperature.Hot);
            desertMountains.Instantiate(hexScale, tileAsset, Temperature.Hot);
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

        public override LandScapeTile GetRandomTile(Temperature temp, GroundLevel height)
        {
            switch (height)
            {
                case GroundLevel.Flat:

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
                    }

                    break;

                case GroundLevel.Hill:
                    return desertHills.GetRandomTile();
                case GroundLevel.Highland:
                    return desertHighLands.GetRandomTile();
                case GroundLevel.Mountain:
                    return desertMountains.GetRandomTile();
            }

            return hotTiles.GetRandomTile();
        }
    }
}
