using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LandScapeTile;


[Serializable]
public abstract class LandScapes
{
    [SerializeField] protected LandScapeTile tileAsset;

    [SerializeField] protected TileContainer hotTiles;
    [SerializeField] protected TileContainer warmTiles;
    [SerializeField] protected TileContainer coldTiles;

    public virtual LandScapeTile GetRandomTile(Temperature temp)
    {
        try
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
        catch (Exception)
        {
            // most likely error is that you didnt fill in the sprites for this temperature in editor
           return hotTiles.GetRandomTile();
        }

    }
    internal static Biome GetBiome(Temperature temp, Precipitation rain)
    {
        return BiomeTable[(int)temp, (int)rain];
    }
    public enum ElevationLevel { Ocean, Sea, Ground };

    public enum GroundLevel { BelowGround, Flat, Hills, Highlands, Mountain };

    /// <summary>
    /// DO NOT CHANGE THE ORDER OF THIS
    /// </summary>
    public enum Temperature
    {
        // DO NOT CHANGE THE ORDER OF THIS
        // IF YOU ARE GOING TO, CHANGE ORDER OF BIOME TABLE TOO

        // VeryHot ,Hot ,Warm ,Cold ,VeryCold ,Freezing  

        Freezing, VeryCold, Cold, Warm, Hot, VeryHot
    }

    /// <summary>
    /// DO NOT CHANGE THE ORDER OF THIS
    /// </summary>
    public enum Precipitation { NoRain, LightRain, ModerateRain, HeavyRain, VolientRain };

    // goes by ordering of TEMPERATURE AND PRECIPITATION
    /// the x axis -> top to down correspondes to preciptation <summary>
    /// the y axis -> left to right correspondes to Temperature
    /// </summary>
    private static readonly Biome[,] BiomeTable = new Biome[6, 5]
    {
        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },
        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },
        { Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands, Biome.WetLands },
      
        { Biome.DryLands, Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands },
        { Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.WetLands },
        { Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.Forest },


        //        { Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.Forest },
        //{ Biome.Desert, Biome.DryLands, Biome.Plains, Biome.Forest, Biome.WetLands },
        //{ Biome.DryLands, Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands },

        //{ Biome.Plains, Biome.Plains, Biome.Plains, Biome.WetLands, Biome.WetLands },
        //{ Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },
        //{ Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains, Biome.Plains },

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

    public enum LandScape
    {
        Ocean,
        Sea,
        Lake,

        Plains,
        Forests,

        Hills,
        Highlands,
        Mountains,

        Miscellaneous
    }


}
