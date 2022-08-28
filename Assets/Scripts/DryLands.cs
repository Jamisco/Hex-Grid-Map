using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    [Serializable]
    internal class DryLands : LandScapes
    {
        [SerializeField] protected TileContainer veryHotTiles;

        [SerializeField] protected TileContainer dryHills;
        [SerializeField] protected TileContainer dryHighlands;
        [SerializeField] protected TileContainer dryMountains;


        public override void Instantiate(float hexScale)
        {
            veryHotTiles.Instantiate(hexScale, tileAsset, Temperature.VeryHot);
            hotTiles.Instantiate(hexScale, tileAsset, Temperature.Hot);
            warmTiles.Instantiate(hexScale, tileAsset, Temperature.Warm);
            coldTiles.Instantiate(hexScale, tileAsset, Temperature.Cold);

            dryHills.Instantiate(hexScale, tileAsset, Temperature.Hot);
            dryHighlands.Instantiate(hexScale, tileAsset, Temperature.Hot);
            dryMountains.Instantiate(hexScale, tileAsset, Temperature.Hot);
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
                    return dryHills.GetRandomTile();
                case GroundLevel.Highland:
                    return dryHighlands.GetRandomTile();
                case GroundLevel.Mountain:
                    return dryMountains.GetRandomTile();
            }

            return hotTiles.GetRandomTile();
        }
    }
}
