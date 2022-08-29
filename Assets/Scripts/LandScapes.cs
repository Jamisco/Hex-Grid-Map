using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LandScapeTile;


public abstract class LandScapes
{
    [SerializeField] protected LandScapeTile tileAsset;

    [SerializeField] protected TileContainer hotTiles;
    [SerializeField] protected TileContainer warmTiles;
    [SerializeField] protected TileContainer coldTiles;

    public virtual LandScapeTile GetRandomTile(Temperature temp)
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
            default:
                return warmTiles.GetRandomTile();
        }
    }
    internal static Biome GetBiome(Temperature temp, Precipitation rain)
    {
        return BiomeTable[(int)temp, (int)rain];
    }

    /// <summary>
    /// DO NOT CHANGE THE ORDER OF THIS
    /// </summary>
    public enum Temperature
    {
        VeryHot, Hot, Warm, Cold, VeryCold, Freezing
    }

    public enum ElevationLevel { Ocean, Sea, Ground };

    public enum HeightLevel { BelowGround, Flat, Hills, Highlands, Mountain };

    /// <summary>
    /// DO NOT CHANGE THE ORDER OF THIS
    /// </summary>
    public enum Precipitation { NoRain, LightRain, ModerateRain, HeavyRain, VolientRain };

    private static readonly Biome[,] BiomeTable = new Biome[6, 5]
    {
        { Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.Forest },
        { Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.WetLands },
        { Biome.DryLands, Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands },

        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands, Biome.WetLands },
        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },
        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },

    };
    internal virtual void Instantiate(float hexScale)
    {
        hotTiles.Instantiate(hexScale, tileAsset, Temperature.Hot);
        warmTiles.Instantiate(hexScale, tileAsset, Temperature.Warm);
        coldTiles.Instantiate(hexScale, tileAsset, Temperature.Cold);
    }

    internal enum Biome
    {
        Desert,
        Plains,
        Forest,
        DryLands,
        WetLands,
    };

    internal enum LandScape
    {
        Ocean,
        Sea,
        Lake,

        Plains,
        Forests,

        Hills,
        Highlands,
        Mountains
    }


}
