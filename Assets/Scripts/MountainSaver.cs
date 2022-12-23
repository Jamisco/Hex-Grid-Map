using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GridManager;
using UnityEngine;
using Unity.VisualScripting;
using static LandScapes;

namespace Assets.Scripts
{
    public static class MountainSaver
    {
        public struct MountainData
        {
            public float _mountainLevelScale;

            public float xOffset;
            public float yOffset;

            public float tempBorderFractal;

            public int minMtnFractal;

            public float mountainScale;

            public float planetFractal;

            public int HexCountX;
            public int HexCountY;
        }

        private static MountainData Data;

        private static PerlinNoise noise;

        private static float SetMountainNoise(int x, int y)
        {
            float multiplier = 0;

            float tempNoise = 0;

            float adder = 0;

            float fractal = Data.planetFractal;

            float divisor = 0;

            fractal += Data.minMtnFractal;

            fractal = Mathf.Clamp(fractal, Data.minMtnFractal, 25);

            adder = fractal * 2;

            // we do this because we want bigger planets to have more water inbetween them
            multiplier = Data._mountainLevelScale;

            for (int i = 1; i <= fractal; i++)
            {
                multiplier *= 2;

                // so we can normalize the values later
                adder /= 2;

                tempNoise += adder * (noise.GetNoise(multiplier * x / Data.HexCountX + Data.xOffset,
                    multiplier * y / Data.HexCountY + Data.yOffset) * Data.mountainScale);
                divisor += adder;

            }

            // this is to normalize values between 0 and 1
            tempNoise = tempNoise / divisor;

            return tempNoise;

        }

        public static float[,] mtnLvlNoiseMap;

        public static float[,] ComputeNoise(MountainData MData)
        {
            Data = MData;

            mtnLvlNoiseMap = new float[Data.HexCountX, Data.HexCountY];

            // multi-threads the loop in order to calculate multiple values simultaenuosly

            Parallel.For(0, Data.HexCountX, x =>
            {
                for (int y = 0; y < Data.HexCountY; y++)
                {
                    // loop through x and y in the 

                    SetMountainNoise(x, y);
                }
            });

            return mtnLvlNoiseMap;
        }

        // YOU ARE GONNA HAVE TO SET THE MOUNTAIN LEVELS TO YOUR DESIRED HEIGHTS
        private static float[] MountainLevel = new float[4];
        public static GroundLevel GetGroundLevel(float levelNoise)
        {
            GroundLevel level;

            //Debug.Log("Level: " + levelNoise);

            if (levelNoise < MountainLevel[0])
            {
                level = GroundLevel.BelowGround;
            }
            else if (levelNoise < MountainLevel[1])
            {
                level = GroundLevel.Flat;
            }
            else if (levelNoise < MountainLevel[2])
            {
                level = GroundLevel.Hills;
            }
            else if (levelNoise < MountainLevel[3])
            {
                level = GroundLevel.Highlands;
            }
            else
            {
                level = GroundLevel.Mountain;
            }

            return level;
        }
    }

    public class PerlinNoise
    {
        public float GetNoise(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}
