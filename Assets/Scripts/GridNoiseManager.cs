using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GridManager;

namespace Assets.Scripts
{
    public class GridNoiseManager : MonoBehaviour
    {
        [SerializeField] private float _precipitationScale;
        [SerializeField] private float _temperatureScale;
        [SerializeField] private float _surfaceLevelScale;

        private float preOffsetX;
        private float preOffsetY;

        private float tempOffsetX;
        private float tempOffsetY;

        [SerializeField] private float surLvlOffsetX;
        [SerializeField] private float surLvlOffsetY;

        [SerializeField] private float elevationScale;
        [SerializeField] private int minSurfaceFractal;

        private float planetTemperature;
        private float tempBorderFractal;
        [SerializeField] private float initialTemperature;

        [SerializeField] private int equatorPercentSize;
        [SerializeField] private int polePercentSize;

        [SerializeField] private float equatorTempMultiplier;
        [SerializeField] private float poleTempMultiplier;

        [SerializeField] private int waterBarrierX;
        [SerializeField] private int waterBarrierY;

        [SerializeField] private float wb_XMultiplier;
        [SerializeField] private int wb_YMultiplier;


        [SerializeField] private int minTemperatureFractal;

        // using 1 event for all scales, Im lazy
        public delegate void ChangeScale(float scale); //I do declare!
        public static event ChangeScale changeScale;  // create an event variable 


        float[,] preNoiseMap;
        float[,] tempNoiseMap;
        float[,] surLvlNoiseMap;

        List<float> noiseValues = new List<float>();

        private PerlinNoise noise;
        private void Awake()
        {
            noise = new PerlinNoise();

            // scale is frequency across the map
            // aka how many times does the value repeat across the map

            _precipitationScale = Random.Range(1f, 5f);
            _temperatureScale = Random.Range(1.2f, 3f);

            _surfaceLevelScale = Random.Range(1f, 2);

            planetTemperature = initialTemperature * 1.25f;
            equatorTempMultiplier = initialTemperature * 3;
            tempBorderFractal = (equatorPercentSize + polePercentSize) / 2;
        }

        private void Start()
        {
            //preOffsetX = Random.Range(-1000f, 1000f);
            //preOffsetY = Random.Range(-1000f, 1000f);

            //tempOffsetX = Random.Range(-1000f, 1000f);
            //tempOffsetY = Random.Range(-1000f, 1000f);

            //surLvlOffsetX = Random.Range(-1000f, 1000f);
            //surLvlOffsetY = Random.Range(-1000f, 1000f);

            preOffsetX = Random.Range(-1000f, 1000f);
            preOffsetY = Random.Range(-1000f, 1000f);

            tempOffsetX = Random.Range(-1000f, 1000f);
            tempOffsetY = Random.Range(-1000f, 1000f);

            surLvlOffsetX = Random.Range(-1000f, 1000f);
            surLvlOffsetY = Random.Range(-1000f, 1000f);

        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _precipitationScale = Random.Range(1f, 5f);
                _temperatureScale = Random.Range(1.2f, 3f);
                _surfaceLevelScale = Random.Range(1f, 2);

               // changeScale(_surfaceLevelScale); // just wanna raise the event to generate grid, nothing more
            }

            if (Input.GetMouseButtonDown(1))
            {
                preOffsetX = Random.Range(-1000f, 1000f);
                preOffsetY = Random.Range(-1000f, 1000f);

                tempOffsetX = Random.Range(-1000f, 1000f);
                tempOffsetY = Random.Range(-1000f, 1000f);

                surLvlOffsetX = Random.Range(-10000f, 10000f);
                surLvlOffsetY = Random.Range(-10000f, 10000f);
            }

            //changeScale(_surfaceLevelScale); // just wanna raise the event to generate grid, nothing more
        }
        public float PrecipitationScale
        {
            get
            {
                return _precipitationScale;
            }
            set
            {
                // unity inspector change variabvle directly, 
                // you can detect that change using if statements
                _precipitationScale = value;

                if (changeScale != null) // checking if anyone is on the other line.
                {
                    // triggers the event
                    changeScale(_precipitationScale);
                }
            }
        }

        public float TemperatureScale
        {
            get
            {
                return _temperatureScale;
            }
            set
            {
                // unity inspector change variabvle directly, 
                // you can detect that change using if statements
                _temperatureScale = value;

                if (changeScale != null) // checking if anyone is on the other line.
                {
                    // triggers the event
                    changeScale(_temperatureScale);
                }
            }
        }

        public float SurfaceLevelScale
        {
            get
            {
                return _surfaceLevelScale;
            }
            set
            {
                // unity inspector changes variable values directly, 
                // you cant detect that change using if statements
                _surfaceLevelScale = value;

                if (changeScale != null) // checking if anyone is on the other line.
                {
                    // triggers the event
                    changeScale(_surfaceLevelScale);
                }
            }
        }

        private void SetTemperatureNoise(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float correctTemperature = 0;

            float tempNoise = 0;

            float fractal = planet.Fractal;

            int equDistance = (int)planet.HexCountY * equatorPercentSize / 100;
            int poleDistance = (int)planet.HexCountY * polePercentSize / 100;

            float divisor = 0;
            float adder = 0;
            float multiplier = 0;

            // gets distance from poles in fractions    
            // the closer the value to the poles, the closer to 0 the number is
            float position = Mathf.Sin(Mathf.PI * ((float)y / planet.HexCountY));

            bool withinEquator = position > Mathf.Sin(Mathf.PI * ((float)(planet.Equator() + equDistance) / planet.HexCountY));

            bool withinPoles = position < Mathf.Sin(Mathf.PI * ((float)(planet.SouthPole + poleDistance) / planet.HexCountY));

            // we add .2f because the polar border sizes are simply too big, 
            correctTemperature = (position + .2f) * planetTemperature;

            if (withinEquator)
            {
                float distance = (float)(equDistance - planet.DistanceFromEquator(y)) / equDistance;

                // the closer to equator, the more we beef up the value, the furthur from the equator, the less intense the value
                correctTemperature += distance * equatorTempMultiplier;
            }

            if (withinPoles)
            {
                float distance = (float)planet.DistanceFromPole(y) / poleDistance;

                // the closer to pole, the more cold it is, 
                correctTemperature *= distance;
            }

            fractal += minTemperatureFractal;

            fractal = Mathf.Clamp(fractal, minTemperatureFractal, 25);

            adder = fractal * 2;

            multiplier = _temperatureScale * tempBorderFractal; // multiply by something to increase border fractality

            for (int i = 1; i <= fractal; i++)
            {
                multiplier *= 2;

                adder /= 2;

                // so we can normalize the values later
                divisor += adder;

                tempNoise += adder * noise.GetNoise(multiplier * x / planet.HexCountX + xOffset, multiplier * y / planet.HexCountY + yOffset);

            }

            // this is to normalize values between 0 and 1
            tempNoise = tempNoise / divisor;

            tempNoise = Mathf.Pow(tempNoise, initialTemperature);

            tempNoise = correctTemperature * tempNoise;

            tempNoiseMap[x, y] = tempNoise;
        }
        private void SetSurfaceNoise(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float tempNoise = 0;

            float divisor = 0;
            float adder = 0;
            float multiplier = 0;
            float correctHeight = 1;

            float fractal = planet.Fractal;

            float position = Mathf.Sin(Mathf.PI * ((float) x / planet.HexCountX));

            bool withinEdgeX = position <= Mathf.Sin(Mathf.PI * ((float)waterBarrierX / planet.HexCountX));


            fractal += minSurfaceFractal;

            fractal = Mathf.Clamp(fractal, minSurfaceFractal, 30);

            adder = fractal * 2;

            // we do this because we want bigger planets to have more water inbetween them
            multiplier = _surfaceLevelScale + planet.Fractal / 6; // 6 is just some random number

            // fractal noise doubles the frequency, halves the octaves

            for (int i = 1; i <= fractal; i++)
            {
                multiplier *= 2;

                adder /= 2;

                // so we can normalize the values later
                divisor += adder;

                tempNoise += adder * noise.GetNoise(multiplier * x / planet.HexCountX + xOffset, multiplier * y / planet.HexCountY + yOffset);
            }

            // this is to normalize values between 0 and 1
            tempNoise = tempNoise / divisor;

            // affects water
            // higher values = more elevation = less water
            // lower values = less elevation = more water

            // high elevation = more land, less water
            // low elevation = less land, more water
            // the numbers are just random numbers I tested - PLEASE DO NOT CHANGE WITHOUT CAREFUL CONSIDERATION

            tempNoise = Mathf.Pow(tempNoise * (elevationScale / 100 * 10), 15);

            if (withinEdgeX && waterBarrierX > 0)
            {
                float distance = 1 - (float)(planet.DistanceFromEdge(x)) / waterBarrierX;

                // the closer to edge, the more we beef up the value, the furthur from the edge, the less intense the value

                correctHeight = distance * wb_XMultiplier;

                // tempnoise is reduced as a percent of correctHeight,
                tempNoise -= correctHeight * tempNoise;
            }
         
            surLvlNoiseMap[x, y] = tempNoise;
        }

        DebugMenu logger = new DebugMenu();

        const float TAU = 2 * Mathf.PI;

        public void ComputeNoise3d(float x, float y, float z)
        {
            //float angle_x = TAU * x;
            ///* In "noise parameter space", we need nx and ny to travel the
            //   same distance. The circle created from nx needs to have
            //   circumference=1 to match the length=1 line created from ny,
            //   which means the circle's radius is 1/2π, or 1/tau */

            //return noise.PerlinNoise3D(Mathf.Cos(angle_x) / TAU, Mathf.Sin(angle_x) / TAU, y);
        }


        public void ComputeNoise(Planet aPlanet)
        {
            preNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];
            tempNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];
            surLvlNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];

            Parallel.For(0, aPlanet.HexCountX, x =>
            {
                for (int y = 0; y < aPlanet.HexCountY; y++)
                {
                    // loop through x and y in the 
                    
                    SetTemperatureNoise(x, y, tempOffsetX, tempOffsetY, aPlanet);

                    //tempNoiseMap[x, y] = noiseValue;

                    ////////////////////////////////////// Precipitation

                    // noiseValue = GetSurfaceNoise(x, y, preOffsetX, preOffsetY, aPlanet);

                    //  preNoiseMap[x, y] = noiseValue;

                    ///////////////////////////////////// Surface Level

                    SetSurfaceNoise(x, y, surLvlOffsetX, surLvlOffsetY, aPlanet);

                    //surLvlNoiseMap[x, y] = noiseValue;
                }
            });
        }

        public float GetTempNoiseValue(int x, int y)
        {
            return tempNoiseMap[x, y];
        }

        public float GetPrecipNoiseValue(int x, int y)
        {
            return preNoiseMap[x, y];
        }

        public float GetSurfaceLevelNoiseValue(int x, int y)
        {
            return surLvlNoiseMap[x, y];
        }
    }
}
