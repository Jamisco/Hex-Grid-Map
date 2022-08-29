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
        [SerializeField] private float _mountainLevelScale;

        private float preOffsetX;
        private float preOffsetY;

        private float tempOffsetX;
        private float tempOffsetY;

        private float surLvlOffsetX;
        private float surLvlOffsetY;

        private float mtnLvlOffsetX;
        private float mtnLvlOffsetY;

        private float tempBorderFractal;

        [SerializeField] private int minSurfaceFractal;
        [SerializeField] private int minTemperatureFractal;
        [SerializeField] private int minPrecipFractal;

        [SerializeField] private float mountainScale;

        [SerializeField] private float initialTemperature;

        [SerializeField] private float elevationMultiplier;
        [SerializeField] private float rainMultiplier;


        [SerializeField] private int waterBarrierX;
        [SerializeField] private int waterBarrierY;

        [SerializeField] private float wb_XMultiplier;
        [SerializeField] private int wb_YMultiplier;




        // using 1 event for all scales, Im lazy
        public delegate void ChangeScale(float scale); //I do declare!
        public static event ChangeScale changeScale;  // create an event variable 

        float[,] precipNoiseMap;
        float[,] tempNoiseMap;
        float[,] surLvlNoiseMap;
        float[,] mtnLvlNoiseMap;

        List<float> noiseValues = new List<float>();
        private float maxTempScale, maxPrecipScale, maxSurScale;



        private PerlinNoise noise;
        private void Awake()
        {
            noise = new PerlinNoise();
            maxTempScale = .5f;
            maxPrecipScale = 1f;
            maxSurScale = 2f;

            // scale is frequency across the map
            // aka how many times does the value repeat across the map

            _precipitationScale = Random.Range(0f, maxPrecipScale);
            _temperatureScale = Random.Range(0f, maxTempScale);
            _surfaceLevelScale = Random.Range(1f, maxSurScale);

            preOffsetX = Random.Range(-1000f, 1000f);
            preOffsetY = Random.Range(-1000f, 1000f);

            tempOffsetX = Random.Range(-1000f, 1000f);
            tempOffsetY = Random.Range(-1000f, 1000f);

            surLvlOffsetX = Random.Range(-1000f, 1000f);
            surLvlOffsetY = Random.Range(-1000f, 1000f);
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

            _mountainLevelScale = Random.Range(5f, 10f);
            mtnLvlOffsetX = Random.Range(-1000f, 1000f);
            mtnLvlOffsetY = Random.Range(-1000f, 1000f);

        }

        private void RefreshMap()
        {
            _precipitationScale = Random.Range(0f, maxPrecipScale);
            preOffsetY = Random.Range(-1000f, 1000f);

            _temperatureScale = Random.Range(0f, maxTempScale);
            tempOffsetX = Random.Range(-1000f, 1000f);
            tempOffsetY = Random.Range(-1000f, 1000f);

            _surfaceLevelScale = Random.Range(1f, maxSurScale);
            surLvlOffsetX = Random.Range(-1000f, 1000f);
            surLvlOffsetY = Random.Range(-1000f, 1000f);

            _mountainLevelScale = Random.Range(5f, 10f);
            mtnLvlOffsetX = Random.Range(-1000f, 1000f);
            mtnLvlOffsetY = Random.Range(-1000f, 1000f);

            changeScale(_surfaceLevelScale); // just wanna raise the event to generate grid, nothing more
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RefreshMap();
            }

            if (Input.GetMouseButtonDown(1))
            {
                preOffsetX = Random.Range(-1000f, 1000f);
                preOffsetY = Random.Range(-1000f, 1000f);

                tempOffsetX = Random.Range(-1000f, 1000f);
                tempOffsetY = Random.Range(-1000f, 1000f);

                surLvlOffsetX = Random.Range(-10000f, 10000f);
                surLvlOffsetY = Random.Range(-10000f, 10000f);

                mtnLvlOffsetX = Random.Range(-1000f, 1000f);
                mtnLvlOffsetY = Random.Range(-1000f, 1000f);


            }

           // changeScale(_surfaceLevelScale); // just wanna raise the event to generate grid, nothing more
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
        public float MountainLevelScale
        {
            get
            {
                return _mountainLevelScale;
            }
            set
            {
                // unity inspector changes variable values directly, 
                // you cant detect that change using if statements
                _mountainLevelScale = value;

                if (changeScale != null) // checking if anyone is on the other line.
                {
                    // triggers the event
                    changeScale(_mountainLevelScale);
                }
            }
        }

        private float GetRandomFloat()
        {
            return (float) (new System.Random().Next(-1, 1) / 20f);
        }

        float maxTemp = -10;
        private void SetTemperatureNoise(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float correctTemperature = 0;

            float tempNoise = 0;

            float fractal = planet.Fractal;

            // gets distance from poles in fractions    
            // the closer the value to the poles, the closer to 0 the number is
            float position = Mathf.Sin(Mathf.PI * ((float)y / planet.HexCountY));

            correctTemperature =
                (position) * initialTemperature + GetRandomFloat();
            // the random float is for artificial fractal

            if (tempNoise > maxTemp)
            {
                maxTemp = tempNoise;
            }

            tempNoiseMap[x, y] = correctTemperature;
        }
        float maxPrecip = -10;
        private void SetPrecipitation(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float tempNoise = 0;
            float divisor = 0;
            float adder = 0;
            float multiplier = 0;

            float fractal = 0;

            fractal += minPrecipFractal;

            fractal = Mathf.Clamp(fractal, minPrecipFractal, 25);

            adder = fractal * 2;

            multiplier = _precipitationScale;

            for (int i = 1; i <= fractal; i++)
            {
                multiplier *= 2;

                adder /= 2;

                // so we can normalize the values later
                divisor += adder;

                tempNoise += adder * noise.GetNoise(multiplier * x / planet.HexCountX + xOffset, multiplier * y / planet.HexCountY + yOffset) * rainMultiplier;
            }

            // this is to normalize values between 0 and 1
            tempNoise = tempNoise / divisor;


            if (tempNoise > maxPrecip)
            {
                maxPrecip = tempNoise;
            }

            precipNoiseMap[x, y] = tempNoise;
        }

        float maxSur = -10;
        float minSur = 10;
        private void SetSurfaceNoise(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float tempNoise = 0;

            float divisor = 0;
            float adder = 0;
            float multiplier = 0;
            float correctHeight = 1;

            float fractal = planet.Fractal;

            float position = Mathf.Sin(Mathf.PI * ((float)x / planet.HexCountX));

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

                // so we can normalize the values later

                tempNoise += adder * (noise.GetNoise(multiplier * x / planet.HexCountX + xOffset, multiplier * y / planet.HexCountY + yOffset) * elevationMultiplier);

                divisor += adder;

                adder /= 2;
            }

            // this is to normalize values between 0 and 1
            // These values are not well normalized,
            // depending on the noise, the max value can be .8 or .7 or .88 etc

            //tempNoise = tempNoise * elevationScale;

            tempNoise = tempNoise / (divisor);

            // if elevation scale less then 1 = more water
            // if elecation scale more than 1 = more land



            // this is so the horizontal edges of the map have water barriers
            if (withinEdgeX && waterBarrierX > 0)
            {
                float distance = 1 - (float)(planet.DistanceFromEdge(x)) / waterBarrierX;

                // the closer to edge, the more we beef up the value, the furthur from the edge, the less intense the value

                correctHeight = distance * wb_XMultiplier;

                // tempnoise is reduced as a percent of correctHeight,
                tempNoise -= correctHeight * tempNoise;
            }

            if (tempNoise < minSur)
            {
                minSur = tempNoise;
            }

            if (tempNoise > maxSur)
            {
                maxSur = tempNoise;
            }

            surLvlNoiseMap[x, y] = tempNoise;
        }
        private void SetMountainNoise(int x, int y, float xOffset, float yOffset, Planet planet)
        {
            float tempNoise = 0;

            float multiplier = 0;

            float fractal = planet.Fractal;

            // we do this because we want bigger planets to have more water inbetween them
            multiplier = _mountainLevelScale + planet.Fractal / 6; // 6 is just some random number

            tempNoise = noise.GetNoise(multiplier * x / planet.HexCountX + xOffset, multiplier * y / planet.HexCountY + yOffset) * mountainScale;

            //tempNoise -= (float) (1 - surLvlNoiseMap[x, y]) * tempNoise;

            mtnLvlNoiseMap[x, y] = tempNoise;

        }


        DebugMenu logger = new DebugMenu();

        bool loaded = false;
        public void ComputeNoise(Planet aPlanet)
        {
            // since arrays memory can be edited and change by any process
            // we do this so the array values in memory are not randomly deleted by other process
            if (!loaded)
            {


                loaded = true;
            }

            precipNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];
            tempNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];
            surLvlNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];
            mtnLvlNoiseMap = new float[aPlanet.HexCountX, aPlanet.HexCountY];

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

                    SetPrecipitation(x, y, preOffsetX, preOffsetY, aPlanet);

                    SetMountainNoise(x, y, mtnLvlOffsetX, mtnLvlOffsetY, aPlanet);
                }
            });

            // NormalizeScales();
        }

        public float GetTempNoiseValue(int x, int y)
        {
            return tempNoiseMap[x, y];
        }

        public float GetPrecipNoiseValue(int x, int y)
        {
            return precipNoiseMap[x, y];
        }

        public float GetSurfaceLevelNoiseValue(int x, int y)
        {
            return surLvlNoiseMap[x, y];
        }

        public float GetMountainLevelNoiseValue(int x, int y)
        {
            return mtnLvlNoiseMap[x, y];
        }

        private void NormalizeScales()
        {
            float preRatio = 1 / maxPrecip;
            float surRatio = 1 / maxSur;
            float tempRatio = 1 / maxTemp;

            // do not normalize temperate Noise map, some values are meant to be above 1

            for (int x = 0; x < precipNoiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < precipNoiseMap.GetLength(1); y++)
                {
                    precipNoiseMap[x, y] = precipNoiseMap[x, y] * preRatio;

                    surLvlNoiseMap[x, y] = surLvlNoiseMap[x, y] * surRatio;
                }
            }

            Debug.Log(surLvlNoiseMap[50, 50]);
        }
    }
}
