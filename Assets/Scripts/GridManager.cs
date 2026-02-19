using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static LandScapes;
using Debug = UnityEngine.Debug;

public class GridManager : MonoBehaviour
{

    // Start is called before the first frame update
    // Make sure you set the sprites in the set sprite method

    private Ocean OceanTiles;
    private Lake LakeTiles;
    private Plains PlainTiles;
    private Hills HillTiles;
    private Highlands HighlandTiles;
    private Mountains MountainTile;
    private Desert DesertTile;
    private Forest ForestTile;
    private DryLands DryLandsTile;
    private WetLands WetLandsTile;

    // Start is called before the first frame update

    // Make sure you set the sprites in the set sprite method

    [SerializeField] private Grid baseGrid;
    [SerializeField] private Tilemap baseTileMap;
    [SerializeField] private Tilemap coastTileMap;
    [SerializeField] private Tilemap riverTileMap;

    [SerializeField] private Camera mainCamera;
    [SerializeField] public float hexScale;

    [SerializeField] private float baseCellSize;

    [SerializeField] private int mapXHexCount, mapYHexCount;

    public readonly struct Planet
    {
        public readonly int HexCountX { get; }
        public readonly int HexCountY { get; }

        public readonly float Width { get; }
        public readonly float Height { get; }
        public readonly Tilemap PlanetMap { get; }
        public readonly GridNoiseManager NoiseManager { get; }

        public readonly float Fractal;

        public readonly List<List<Vector2>> planetMountains;

        private readonly List<Vector2> mountains;

        public Planet(int mapXCount, int mapYCount, float mapWidth, float mapHeight,
            float fractal, Tilemap planetMap, GridNoiseManager noiseManager)
        {
            HexCountX = mapXCount;
            HexCountY = mapYCount;

            Height = mapHeight;
            Width = mapWidth;

            Fractal = fractal;

            mountains = new List<Vector2>();
            planetMountains = new List<List<Vector2>>();

            PlanetMap = planetMap;
            NoiseManager = noiseManager;

            ///  SetMountainHexes();
        }


        //private void SetMountainHexes()
        //{
        //    int length;

        //    int xPos;
        //    int yPos;
        //    int direction = 0;

        //    List<Vector2> aMountain;
        //    Vector2 newPosition;

        //    for (int i = 0; i < NumOfMountains; i++)
        //    {
        //        aMountain = new List<Vector2>();

        //        xPos = Random.Range(0, HexCountX);
        //        yPos = Random.Range(0, HexCountY);

        //        newPosition = new Vector2(xPos, yPos);
        //        mountains.Add(newPosition);

        //        length = Random.Range(1, MountainHexLength);

        //        for (int x = 0; x < length; x++)
        //        {
        //            // if use a while loop because the direction we roll, 
        //            // might already have a mountain and so we want to start over

        //            while (true)
        //            {
        //                direction = Random.Range(1, 7);

        //                newPosition = GetNeighborHex(direction, newPosition);

        //                if (!mountains.Contains(newPosition))
        //                {
        //                    mountains.Add(newPosition);
        //                    break;
        //                }

        //                if (MtnHexIsEnclosed(newPosition))
        //                {
        //                    break;
        //                }
        //            }


        //            //width = yPos + Random.Range(1, MountainHexWidth);

        //            //for (int y = yPos; y < width; y++)
        //            //{
        //            //    mountains.Add(new Vector2(x, ));

        //            //}
        //        }
        //    }
        //}

        //private bool MtnHexIsEnclosed(Vector2 hexPos)
        //{
        //    Vector2 temp = new Vector2();

        //    for (int i = 1; i <= 6; i++)
        //    {
        //        temp = GetNeighborHex(i, hexPos);

        //        if (mountains.Contains(temp))
        //        {
        //            return false;
        //        }

        //    }

        //    return true;
        //}

        private Vector2 GetNeighborHex(int neighborSide, Vector2 pos)
        {
            switch (neighborSide)
            {
                case 1:
                    // increase both x and y by 1

                    if (pos.y % 2 == 1)
                    {
                        pos.x += 1;
                    }

                    pos.y += 1;

                    break;
                case 2:

                    pos.x += 1;

                    break;
                case 3:

                    if (pos.y % 2 == 1)
                    {
                        pos.x += 1;
                    }

                    pos.y -= 1;

                    break;
                case 4:

                    if (pos.y % 2 == 0)
                    {
                        pos.x += 1;
                    }

                    pos.y -= 1;

                    break;
                case 5:

                    pos.x -= 1;
                    break;

                default:
                    // case 6

                    if (pos.y % 2 == 0)
                    {
                        pos.x += 1;
                    }

                    pos.y += 1;
                    break;
            }

            return pos;
        }
        public int Equator()
        {
            // equator is 
            return HexCountY / 2;
        }

        public int DistanceFromPole(int y)
        {
            int tempEquator = Equator();

            if (y > tempEquator)
            {
                return NorthPole - y;
            }
            else
            {
                return y - SouthPole;
            }
        }

        public int DistanceFromEquator(int y)
        {
            int tempEquator = Equator();

            if (y > tempEquator)
            {
                return y - tempEquator;
            }
            else if (y < tempEquator)
            {
                return tempEquator - y;
            }

            return 0;
        }

        public int DistanceFromEdge(int x)
        {
            int center = HexCountX / 2;

            if (x > center)
            {
                return HexCountX - x;
            }
            else if (x < center)
            {
                return x - 0;
            }

            return center;
        }

        public bool HexIsMountain(int x, int y)
        {
            Vector2 pos = new Vector2(x, y);

            if (mountains.Contains(pos))
            {
                return true;
            }

            return false;
        }

        public int NorthPole { get { return HexCountY; } }
        public int SouthPole { get { return 0; } }

        public Vector2Int MapSize
        {
            get
            {
                return new Vector2Int(HexCountX, HexCountY);
            }

        }

    }

    SpriteManager baseTileSpriteManager;
    GridNoiseManager noiseManager;
    CoastSpriteManager coastSpriteManager;
    RiverSpriteManager riverSpriteManager;

    private float hexWidth, hexHeight;


    private readonly Vector3Int botLeftPos = new(0, 0, 0);
    private Vector2Int mapSize;
    private Vector3 mapCenterPos;
    private Vector3Int centerTile;
    private float mapWidth, mapHeight;


    [SerializeField] Planet MainPlanet;
    /// <summary>
    /// The size is the distance from the center to any corner.In the pointy orientation,
    /// </summary>
    float hexSize;

    /// <summary>
    /// Distance between the center of one hex to the center of its adjacent hex, respectively
    /// </summary>
    private float hexDistanceX, hexDistanceY;

    //  if your tiles arent anchoring properly, make sure the tilemap pivot is
    // x = -.5, y = -2/3
    void Start()
    {
        // we name these tiles so we can identify them later on
        // these name will be kept even if the game stop running - BEWARE!!!

        baseGrid = GetComponent<Grid>();

        foreach (Component map in baseGrid.GetComponentsInChildren<Tilemap>())
        {
            if (map.name.Equals("BaseTileMap"))
            {
                baseTileMap = map as Tilemap;
            }

            if (map.name.Equals("CoastTileMap"))
            {
                coastTileMap = map as Tilemap;
            }
            if (map.name.Equals("RiverTileMap"))
            {
                riverTileMap = map as Tilemap;
            }
        }

        baseTileSpriteManager = baseGrid.GetComponent<SpriteManager>();
        coastSpriteManager = coastTileMap.GetComponent<CoastSpriteManager>();
        riverSpriteManager = riverTileMap.GetComponent<RiverSpriteManager>();

        baseCellSize *= (int)hexScale;

        hexSize = baseCellSize;

        hexWidth = hexSize * 2;
        hexHeight = hexWidth * .75f; // at scale of 3 this should be 4.5

        hexDistanceX = hexHeight * 2; // at scale of 3 this should be 9
        hexDistanceY = hexDistanceX * .75f; // at scale of 3 this should be 6

        baseGrid.cellSize = new Vector2(baseCellSize, baseCellSize);

        baseTileMap.transform.localScale = new Vector2(hexScale, hexScale);

        noiseManager = baseGrid.GetComponent<GridNoiseManager>();

        GridNoiseManager.changeScale += GridNoiseManager_changeScale;

        SetMapValues();

        SetSprites();

        GenerateGrid();

        SetScrollVariables();

        CenterCamera();

      //  DrawEdgePositions();

    }

    Vector3Int prevTilePos;
    Color prevTileColor;

    Vector3Int curTilePos;
    Vector3 curMousePos;
    Vector3 prevCamPosition = new Vector3();
    public void Update()
    {
        curTilePos = GetTilePosAtMousePos();
        curMousePos = GetmousePosition();

        SetCameraMovement();

        bool hasTile = baseTileMap.HasTile(curTilePos);

             LandScapeTile sdsd = baseTileMap.GetTile(curTilePos) as LandScapeTile;

        // if the mouse is not pointing at a tile, then gettile at location x will be null
        if (hasTile == true)
        {
            // we dont want to rehighlight a tile if it is already highlighted
            if (!curTilePos.Equals(prevTilePos))
            {
                prevTileColor = HighLightTile(curTilePos, prevTilePos, prevTileColor);
                prevTilePos = curTilePos;
            }

            CheckZoomScroll(curMousePos);

            SideScroll();
        }

    }

    private void SetSprites()
    {
        coastTileMap.transform.localScale = new Vector2(hexScale, hexScale);

        riverTileMap.transform.localScale = new Vector2(hexScale, hexScale);

        baseTileSpriteManager.Instantiate(hexScale);
        coastSpriteManager.Instantiate(hexScale);
        riverSpriteManager.Instantiate(hexScale);

        // each tile must have its own unique sprite, this is because each tile is its own
        // object. The tilemap has one instance of each tile(object)
        // even if 100 tiles of that instance are being displayed, t
        // they still refer to thesame instance. Thus, changing the sprite/data of one instance
        // also changes the data/sprites in all the other 100 instances

        OceanTiles = baseTileSpriteManager.Oceans;
        LakeTiles = baseTileSpriteManager.Lakes;
        PlainTiles = baseTileSpriteManager.Plains;
        HillTiles = baseTileSpriteManager.Hills;
        HighlandTiles = baseTileSpriteManager.Highlands;
        MountainTile = baseTileSpriteManager.Mountains;
        DesertTile = baseTileSpriteManager.Deserts;

        DryLandsTile = baseTileSpriteManager.DryLands;
        WetLandsTile = baseTileSpriteManager.WetLands;
        ForestTile = baseTileSpriteManager.Forest;
    }

    // every time we change the map height, width etc, we wanna change respective values such as center etc
    private void SetMapValues()
    {
        mapSize = new Vector2Int(mapXHexCount, mapYHexCount);

        centerTile = new Vector3Int(mapXHexCount / 2, mapYHexCount / 2);

        mapCenterPos = baseTileMap.CellToWorld(centerTile);
        mapCenterPos.z = -1; /// yes it needs to be -1 ---> for the camera

        mapWidth = mapCenterPos.x * 2;
        mapHeight = mapCenterPos.y * 2;

        MainPlanet = new Planet(mapXHexCount, mapYHexCount, mapWidth, mapHeight,
            GetFractal(), baseTileMap, noiseManager);

        baseTileMap.size = new Vector3Int(mapXHexCount, mapYHexCount);

        MapRivers = new List<River>();
        MapCoasts = new List<Coast>();

        RiverTilePlacer.RiverTileMap = riverTileMap;
        CoastTilePlacer.CoastTileMap = coastTileMap;

    }
    private void GridNoiseManager_changeScale(float scale)
    {
        waterTilePositions.Clear();
        MapRivers.Clear();
        riverTileMap.ClearAllTiles();
        riverTileMap.RefreshAllTiles();

        plateauPositions.Clear();
        MapCoasts.Clear();
        coastTileMap.ClearAllTiles();
        coastTileMap.RefreshAllTiles();

        RiverTilePlacer.ResetRivers();
        CoastTilePlacer.ResetCoasts();

        SetMapValues();

        GenerateGrid();

        SetScrollVariables();

       // DrawEdgePositions();

    }

    DebugMenu logger = new DebugMenu();
    private void GenerateGrid()
    {
      //  logger.StartTimer();

        noiseManager.ComputeNoise(MainPlanet);

        //logger.LogTime("Compute Noise");

        List<Vector3Int> tilePositions = new List<Vector3Int>();
        List<LandScapeTile> tiles = new List<LandScapeTile>();

        for (int x = 0; x < mapXHexCount; x++)
        {
            tilePositions.Clear();
            tiles.Clear();

            for (int y = 0; y < mapYHexCount; y++)
            {
                tilePositions.Add(new Vector3Int(x, y));

                tiles.Add(GetBiomeToPlace(x, y));
            }

            PlaceTiles(tilePositions.ToArray(), tiles.ToArray(), false);
        }

        baseTileMap.RefreshAllTiles();

        PlaceRiverTiles();

       // logger.LogTime("River");

        PlaceCoastTiles();

        //logger.LogTime("Coast");
    }
    private Temperature GetTemperature(float tempNoise)
    {
        Temperature temp = Temperature.Warm;

        switch (tempNoise)
        {
            case < .15f:
                temp = Temperature.Freezing;
                break;
            case < .35f:
                temp = Temperature.VeryCold;
                break;
            case < .50f:
                temp = Temperature.Cold;
                break;
            case < .6f:
                temp = Temperature.Warm;
                break;
            case < .85f:
                temp = Temperature.Hot;
                break;
            default:
                temp = Temperature.VeryHot;
                break;
        }

        return temp;
    }
    private Precipitation GetPrecipitation(float precipNoise)
    {
        Precipitation precip;

        switch (precipNoise)
        {
            case < .15f:
                precip = Precipitation.NoRain;
                break;
            case < .35f:
                precip = Precipitation.LightRain;
                break;
            case < .65f:
                precip = Precipitation.ModerateRain;
                break;
            case < .85f:
                precip = Precipitation.HeavyRain;
                break;
            default:
                precip = Precipitation.VolientRain;
                break;
        }

        return precip;
    }
    private ElevationLevel GetElevationLevel(float levelNoise)
    {
        ElevationLevel level;

        //Debug.Log("Level: " + levelNoise);

        switch (levelNoise)
        {
            case < .3f:
                level = ElevationLevel.Ocean;
                break;
            case < .4f:
                level = ElevationLevel.Sea;
                break;
            default:
                level = ElevationLevel.Ground;
                break;
        }

        return level;
    }

    [SerializeReference]  float[] MountainLevel = new float[4];
    private GroundLevel GetGroundLevel(float levelNoise)
    {
        GroundLevel level;

        //Debug.Log("Level: " + levelNoise);

        if(levelNoise < MountainLevel[0])
        {
            level = GroundLevel.BelowGround;
        }
        else if(levelNoise < MountainLevel[1])
        {
            level = GroundLevel.Flat;
        }
        else if(levelNoise < MountainLevel[2])
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
    public void PlaceTiles(Vector3Int[] pos, LandScapeTile[] tiles, bool refreshTiles = false)
    {
        // PLACE TILES AS A SET IS MUCH FASTER THAN PLACETILES BLOCK
        // BLOCK MIGHT BE FASTER FOR LARGEEEEE TILES, BUT CANT CONFIRM
        // FOR THE MOMENT, PLACING AT SET MUCH MUCH X15 TIMES FASTER
        baseTileMap.SetTiles(pos, tiles);

        if (refreshTiles == true)
        {
            baseTileMap.RefreshAllTiles();
        }
    }
    public void PlaceTilesBlock(BoundsInt bounds, LandScapeTile[] tiles)
    {
        // BE ADVISED, SETTILES BLOCK SETS THE TILES TOP TO BOTTOM, COLUMN TO COLUMN
        // SO make sure the order of the tiles array is ordered in a column to column
        // so, when you are getting the landscapteTile, loop through the entire y axis,
        // then loop through enter x axis

        // If this isnt working, try setting the Z Axis to one
        // Also remember the positions (2, 2, 1) and (2, 2, 0) are different on the tile map
        // they may look thesame but they are different

        // PLACE TILES AS A SET IS MUCH FASTER THAN PLACETILES BLOCK
        // BLOCK MIGHT BE FASTER FOR LARGEEEEE TILES, BUT CANT CONFIRM
        // FOR THE MOMENT, PLACING AT SET MUCH MUCH X15 TIMES FASTER

        baseTileMap.SetTilesBlock(bounds, tiles);
        baseTileMap.RefreshAllTiles();
    }
    private List<River> MapRivers { get; set; }


    [SerializeField] float riverSpread, heightMinimum, DropVelocity, InitialSpeed, FlowChangeFriction;

    [SerializeField] float[] friction = new float[3];

    private void PlaceRiverTiles()
    {
        River.mainPlanet = MainPlanet;
        River.RiverFlowChangeFriction = FlowChangeFriction;
        River.PlanetGravityVelocity = DropVelocity;
        River.InitialSpeed = InitialSpeed;
        River.RiverSpread = riverSpread;
        River.Frictions = friction;

        if (plateauPositions.Count > 0)
        {
            foreach (Vector3Int source in River.GetSourceLocations(plateauPositions, heightMinimum))
            {
                MapRivers.Add(new River(source));
            }
        }

        if (MapRivers.Count > 0)
        {
            foreach (River river in MapRivers)
            {
                PlaceLakeTile(river.GetLakePosition);
            }

           // Debug.Log("We have " + MapRivers.Count + " Rivers");

            RiverTilePlacer.Instantiate(MapRivers, riverSpriteManager);
            RiverTilePlacer.DisplayRivers();
        }
    }
    private void PlaceLakeTile(Vector3Int pos)
    {
        float tempNoise = noiseManager.GetTempNoiseValue(pos.x, pos.y);
        Temperature temp = GetTemperature(tempNoise);
        LandScapeTile aTile = baseTileMap.GetTile(pos) as LandScapeTile;

        // we dont want to create a lake where there is already a sea or ocean
        if (!aTile.LandScapeIsSeaorOcean())
        {
            baseTileMap.SetTile(pos, baseTileSpriteManager.Lakes.GetRandomTile(temp));

            baseTileMap.RefreshTile(pos);
        }
    }
    private List<Coast> MapCoasts { get; set; }
    private void PlaceCoastTiles()
    {
        Coast.SetPlanet(MainPlanet);

        MapCoasts = Coast.CreateCoasts(waterTilePositions);

        CoastTilePlacer.Instantiate(MapCoasts, coastSpriteManager);
        CoastTilePlacer.DisplayCoasts();
    }

    List<Vector3Int> waterTilePositions = new List<Vector3Int>();
    List<(Vector3Int, float)> plateauPositions = new List<(Vector3Int, float)>();

    public LandScapeTile GetBiomeToPlace(int x, int y)
    {
        // based of Koppen climate classification, look up on wikipedia
        float tempNoise, preNoise, surlvlNoise, mtnLvlNoise;
        Vector3Int curPos = new Vector3Int(x, y, 0);

        tempNoise = noiseManager.GetTempNoiseValue(x, y);
        preNoise = noiseManager.GetPrecipNoiseValue(x, y);
        surlvlNoise = noiseManager.GetSurfaceLevelNoiseValue(x, y);
        mtnLvlNoise = noiseManager.GetMountainLevelNoiseValue(x, y);

        Temperature temp = GetTemperature(tempNoise);
        Precipitation rain = GetPrecipitation(preNoise);
        ElevationLevel elvationLvl = GetElevationLevel(surlvlNoise);
        GroundLevel grndLevel = GetGroundLevel(mtnLvlNoise);

        Biome aBiome = GetBiome(temp, rain);

        // LandScapeType biomeType = biomeTable[(int)temp, (int)precip];
        // first determine if land or water
        // if water -- check for mountauns...maybe
        // if land - check height map
        // if hills --> mountains == display with proper temp
        // if plains
        // - check precipitation
        // display woodlands with proper temp

        LandScapeTile returnTile = null;

        switch (elvationLvl)
        {
            case ElevationLevel.Ocean:
            case ElevationLevel.Sea:
                waterTilePositions.Add(curPos);

                returnTile = OceanTiles.GetRandomTile(temp);
                break;

            case ElevationLevel.Ground:

                if (grndLevel > GroundLevel.Hills)
                {
                    plateauPositions.Add((curPos, mtnLvlNoise));
                }

                switch (aBiome)
                {
                    case Biome.Desert:
                        returnTile = DesertTile.GetRandomTile(temp, grndLevel);
                        break;
                    case Biome.Plains:
                        returnTile = PlainTiles.GetRandomTile(temp, grndLevel);
                        break;
                    case Biome.Forest:
                        returnTile = ForestTile.GetRandomTile(temp, grndLevel);
                        break;
                    case Biome.DryLands:
                        returnTile = DryLandsTile.GetRandomTile(temp, grndLevel);
                        break;
                    case Biome.WetLands:
                        returnTile = WetLandsTile.GetRandomTile(temp, grndLevel);
                        break;
                    default:
                        returnTile = PlainTiles.GetRandomTile(temp, grndLevel);
                        break;
                }
                break;
        }

        returnTile.Elevation = surlvlNoise;

        return returnTile;
    }
    private void CenterCamera()
    {
        //The horizontal distance between adjacent hexagon centers is the hex width. The vertical distance between adjacent hexagon centers is height * 3/4.

        mainCamera.transform.position = mapCenterPos;
        //Debug.Log("Position = " + mainCamera.transform.position);
        mainCamera.orthographicSize = GetMaxViewOrthoSize();
    }
    private float GetMinimumOrthoSize()
    {
        return (minTileViewed * hexWidth) / 2 + hexSize;
    }
    private float GetMaxViewOrthoSize()
    {
        // look at setvariables method for variable declaration
        return (maxTileViewed * hexWidth) / 2 + hexSize;
    }
    /// <summary>
    /// Get the maximum orthographic size that the camera size can be given the current camera location
    /// Note - it can gives values bigger than the maximum allowed size - do use clamps
    /// </summary>
    /// <returns></returns>
    private float GetOrthoSize()
    {
        //default camera aspect = 1.7
        float posX = mainCamera.transform.position.x;
        float posY = mainCamera.transform.position.y;

        float orthoSize;

        float xDiff, yDiff;
        float aspectRatio;

        if (posX > mapWidth / 2)
        {
            // right of center point
            xDiff = mapWidth - posX;
        }
        else
        {
            xDiff = posX;
        }

        if (posY > mapHeight / 2)
        {
            // above center point
            yDiff = mapHeight - posY;
        }
        else
        {
            yDiff = posY;
        }

        aspectRatio = xDiff / yDiff;

        // code partly taken from here
        // https://pressstart.vip/tutorials/2018/06/14/37/understanding-orthographic-size.html

        if (aspectRatio >= mainCamera.aspect)
        {
            //set ortho size according to current height distance
            orthoSize = yDiff;
        }
        else
        {
            //set ortho size according to current width distance

            float sizeDiff = aspectRatio / mainCamera.aspect;

            orthoSize = yDiff * sizeDiff;
        }

        return orthoSize;

    }
    private float GetMaxOrthoSize(float posX, float posY)
    {
        float orthoSize;

        float xDiff, yDiff;
        float aspectRatio;

        if (posX > mapWidth / 2)
        {
            // right of center point
            xDiff = mapWidth - posX;
        }
        else
        {
            xDiff = posX;
        }

        if (posY > mapHeight / 2)
        {
            // above center point
            yDiff = mapHeight - posY;
        }
        else
        {
            yDiff = posY;
        }

        aspectRatio = xDiff / yDiff;
        //camAspectRatio = 1 / mainCamera.aspect; // 1 / aspect = 1 / 1.7778 = .562

        //work on zooming out, that is where all the bugs are

        // code partly taken from here
        // https://pressstart.vip/tutorials/2018/06/14/37/understanding-orthographic-size.html

        if (aspectRatio >= mainCamera.aspect)
        {
            //set ortho size according to current height distance

            orthoSize = yDiff;
        }
        else
        {
            //set ortho size according to current width distance

            float sizeDiff = aspectRatio / mainCamera.aspect;

            //Debug.Log("Y Diff: " + yDiff);
            //Debug.Log("YDiff x Size" + (yDiff * sizeDiff));

            orthoSize = yDiff * sizeDiff;

        }

        return orthoSize;
    }

    Vector3Int scrollTilePos = new Vector3Int(0, 0, 0);
    private Vector3 GetScrollPosition(Vector3 curMousePos)
    {
        Vector3 scrollPos = new Vector3();

        if (MouseMoved())
        {
            scrollTilePos = GetTilePosAtMousePos(curMousePos);
            //Debug.Log("Mouse has Moved " + curMousePos.ToString());
            return curMousePos;
        }
        else
        {
            scrollPos = baseTileMap.CellToWorld(scrollTilePos);

            //Debug.Log("Mouse Did not Move " + scrollTilePos.ToString() + " -- ");
            return scrollPos;
        }
    }

    enum ScrollDirection { Forward, Backwards, None }

    void CheckZoomScroll(Vector3 curMousePos)
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            ZoomScroll(ScrollDirection.Backwards, GetScrollPosition(curMousePos));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forwards
        {
            ZoomScroll(ScrollDirection.Forward, GetScrollPosition(curMousePos));
        }
    }

    //float zoomPos, posX, posY;
    //int zoomXSpeed = 5;
    //int zoomYSpeed = 5;

    float minOrthoSize; // create a formula to calculate this
    float maxOrthoSize;

    /// <summary>
    /// This represents the minimum/maximum number of tiles viewed at the least/most orthographic size
    /// </summary>
    int minTileViewed;
    int maxTileViewed;

    float barrierX, barrierY;

    private void SetScrollVariables()
    {
        minTileViewed = 10;
        maxTileViewed = MainPlanet.HexCountX;

        barrierX = hexDistanceX * 3;
        barrierY = hexDistanceY * 3;

        minOrthoSize = GetMinimumOrthoSize();
        maxOrthoSize = GetMaxViewOrthoSize();
    }
    private void ZoomScroll(ScrollDirection zoomType, Vector3 scrollPos)
    {
        if (zoomType == ScrollDirection.None)
        {
            return;
        }

        float zoomSpeed = .15f; // percent to zoom in by - so assume we have a distance of 100, .15 means zoom in 15 percent, after 1 zoom iteration, our distance is now 85

        float orthoZoomSpeed = zoomSpeed / 2; // divide by 2 cuz why the fuck now

        Vector2 zoomVectorSpeed = new(zoomSpeed, zoomSpeed);

        Vector3 currCamPos = mainCamera.transform.position;

        Vector3 posVectorDiff;

        float posX, posY, zoom;

        if (zoomType == ScrollDirection.Forward)
        {
            // we use vector2 because the z axis for camera is -1 meanwhile mouse z axis is 0
            float camMouseDistance = Vector2.Distance(currCamPos, scrollPos);

            // no point scrolling into hex the camera is already centered on
            if (camMouseDistance > hexWidth)
            {
                posVectorDiff = scrollPos - currCamPos;

                posVectorDiff *= zoomSpeed;

                posX = currCamPos.x + posVectorDiff.x;
                posY = currCamPos.y + posVectorDiff.y;

                posX = Mathf.Clamp(posX, barrierX, mapWidth - barrierX);
                posY = Mathf.Clamp(posY, barrierY, mapHeight - barrierY);

                //Debug.Log(posX + " --- " + posY);

                mainCamera.transform.position = new Vector3(posX, posY, -1);
            }
            else
            {
                // dont implement this block, it creates little glitches and its FUCKING ANNOYING
            }

            //float orthoSize = GetOrthoSize();

            zoom = mainCamera.orthographicSize * (1 - orthoZoomSpeed); // reduce current orthosize by percentage of zoom speed

            zoom = Mathf.Clamp(zoom, minOrthoSize, maxOrthoSize);

            mainCamera.orthographicSize = zoom;
        }
        else if (zoomType == ScrollDirection.Backwards)
        {
            //maxOrthoSize = GetOrthoSize();

            zoom = mainCamera.orthographicSize * (1 + orthoZoomSpeed * 2f); // increase current orthosize by percentage of zoom speed
            zoom = Mathf.Clamp(zoom, minOrthoSize, maxOrthoSize);

            mainCamera.orthographicSize = zoom;
        }
    }
    private void SideScroll()
    {
        Vector3 currCamPos = mainCamera.transform.position;

        float posX, posY;
        Vector2 scrollVector = GetScrollMultiplier();

        posX = currCamPos.x;
        posY = currCamPos.y;

        posX += scrollVector.x;
        posY += scrollVector.y;

        Vector3 position = new Vector3(posX, posY);

        float camHeight = CameraHeight;
        float camWidth = CameraHeight * mainCamera.aspect;
      
        switch (GetMapEdge())
        {
            case MapBounds.LeftBound:
        
                if (posX <= 0 - (camWidth / 2) + hexDistanceX)
                {
                    posX = mapWidth - camWidth / 2 + hexDistanceX;
                }

                break;
            case MapBounds.RightBound:

                if (posX >= mapWidth + camWidth / 2 - hexDistanceX)
                {
                    posX = 0 + camWidth / 2 - hexDistanceX;
                }

                break;

            case MapBounds.BottomBound:

                if (posY <= 0 - camHeight / 2 + hexDistanceY)
                {
                    posY = mapHeight - camHeight / 2 + hexDistanceY;
                }

                break;

            case MapBounds.TopBound:

                if (posY >= mapHeight + camHeight / 2 - hexDistanceY)
                {
                    posY = 0 + camHeight / 2 - hexDistanceY;
                }
                break;

            default:
                break;
        }


        mainCamera.transform.position = new Vector3(posX, posY, -1);
    }
    private bool MouseIsInsideCamera()
    {
        Vector3 mousePos = GetmousePosition();

        float camLeft = GetCamLeftEdgePosition;
        float camRight = GetCamRightEdgePosition;

        float camTop = GetCamTopEdgePosition;
        float camBot = GetCamBotEdgePosition;

        if (mousePos.y >= camBot && mousePos.y <= camTop)
        {
            if (mousePos.x >= camLeft && mousePos.x <= camRight)
            {
                return true;
            }
        }

        return false;
    }
    private Vector2 GetScrollMultiplier()
    {
        // we divide camera size because when zoomed in, the speed can be very fast/sensitive
        float scrollSpeed = 30f *
            MathFunctions.Normalize(CameraSize, minOrthoSize, maxOrthoSize, .05f, 1);

        // if the mouse is past this point, begin scrolling
        float scrollPoint = .5f;
        float percent;
        Vector2 multiplier = Vector2.zero;

        Vector3 mousePos = GetmousePosition();

        float camLeft = GetCamLeftEdgePosition;
        float camRight = GetCamRightEdgePosition;

        float camTop = GetCamTopEdgePosition;
        float camBot = GetCamBotEdgePosition;

        float leftDiff, rightDiff, botDiff, topDiff;

        leftDiff = Mathf.Abs(camLeft - mousePos.x);
        rightDiff = Mathf.Abs(camRight - mousePos.x);

        botDiff = Mathf.Abs(camBot - mousePos.y);
        topDiff = Mathf.Abs(camTop - mousePos.y);

        if (MouseIsInsideCamera())
        {
            if (mousePos.x >= camLeft && mousePos.x <= camRight)
            {
                //mouse if more left
                if (leftDiff < rightDiff)
                {
                    percent = Mathf.Sin((float)Math.PI * (leftDiff / CameraWidth));

                    if (percent <= scrollPoint)
                    {
                        multiplier.x = scrollSpeed * ((1 - percent) * -1 + scrollPoint);
                    }
                }
                else
                {
                    //mouse is more right
                    percent = Mathf.Sin((float)Math.PI * (rightDiff / CameraWidth));

                    if (percent <= scrollPoint)
                    {
                        multiplier.x = scrollSpeed * ((1 - percent) - scrollPoint);
                    }
                }
            }

            if (mousePos.y >= camBot && mousePos.y <= camTop)
            {
                //mouse is more bot
                if (botDiff < topDiff)
                {
                    percent = Mathf.Sin((float)Math.PI * (botDiff / CameraHeight));

                    if (percent <= scrollPoint)
                    {
                        multiplier.y = scrollSpeed * ((1 - percent) * -1 + scrollPoint);
                    }
                }
                else
                {
                    // mouse is more top
                    percent = Mathf.Sin((float)Math.PI * (topDiff / CameraHeight));

                    if (percent <= scrollPoint)
                    {
                        multiplier.y = scrollSpeed * ((1 - percent) - scrollPoint);
                    }
                }
            }
        }

        return multiplier;
    }
    private Color HighLightTile(Vector3Int curPos, Vector3Int prevPos, Color prevColor)
    {
        Color curColor = baseTileMap.GetColor(curPos);

        LandScapeTile curTile = baseTileMap.GetTile(curPos) as LandScapeTile;

        baseTileMap.SetColor(curPos, curTile.HighlightColor);

        try
        {
            // if prevTile is null or some random value that isnt on the map
            // this is because it is not yet initialized with a proper map value
            // color white is the transparency color for all sprites

            baseTileMap.SetColor(prevPos, Color.white);
        }
        catch (Exception)
        {
            return curColor;
        }

        return curColor;
    }
    private Vector3Int GetTilePosAtMousePos()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 1; // its a 2d map

        try
        {
            return baseTileMap.WorldToCell(mouseWorldPos);
        }
        catch (Exception)
        {

            Debug.Log("Error at GetTilePosAtMousePos - returning vector(0,0,0)");
            return new Vector3Int(-1, -1, 0);
        }

    }
    private Vector3Int GetTilePosAtMousePos(Vector3 mouseWorldPos)
    {
        mouseWorldPos.z = 0; // its a 2d map

        try
        {
            return baseTileMap.WorldToCell(mouseWorldPos);
        }
        catch (Exception)
        {

            Debug.Log("Error at GetTilePosAtMousePos - returning vector(0,0,0)");
            return new Vector3Int(-1, -1, 0);
        }

    }
    private bool TileIsLand(Vector3Int pos)
    {
        LandScapeTile atile = baseTileMap.GetTile(pos) as LandScapeTile;

        if (atile == null)
        {
            return false;
        }

        switch (atile.TileLandScape)
        {
            case LandScape.Ocean:
            case LandScape.Sea:
            case LandScape.Lake:
                return false;
            default:
                return true;
        }
    }
    private List<Vector3Int> GetTilesAtCameraPos()
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();

        Vector3 camPos = Camera.main.transform.position;

        int yDistance = (int)hexDistanceY * 2;

        float camHeight = 2 * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int posFromLeftEdge = Mathf.CeilToInt(camPos.x / hexDistanceX); // gets number of hexes from left side to center of cam

        int posfromBottom = Mathf.FloorToInt(camPos.y / hexDistanceY); // gets number of hexes from bottom side to center of cam

        int widthHexCount = Mathf.CeilToInt(camWidth / hexDistanceX); // number of hexes inside the camera from left to right
        int heightHexCount = Mathf.CeilToInt(camHeight / hexDistanceY); // number of hexes inside the camera from top to bottom

        // remember the camera is at the center - so we have to divide by 2
        int leftSide = posFromLeftEdge - widthHexCount / 2;
        int rightSide = posFromLeftEdge + widthHexCount / 2;
        int bottomSide = posfromBottom - heightHexCount / 2;

        for (int i = leftSide; i <= rightSide; i++)
        {
            for (int j = bottomSide; j <= bottomSide + heightHexCount; j++)
            {
                tileVectors.Add(new Vector3Int(i, j, 0));
            }
        }

        return tileVectors;

    }
    private List<Vector3Int> ConvertBoundsToVectors(BoundsInt bounds)
    {
        List<Vector3Int> vectors = new List<Vector3Int>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            vectors.Add( new Vector3Int(pos.x, pos.y));
        }

        return vectors;
    }

    DebugMenu logger1 = new DebugMenu();
    private void DrawEdgePositions()
    {
        List<Vector3Int> drawPositions = new List<Vector3Int>(); // this is where we draw the new tiles
        List<Vector3Int> position2Draw = new List<Vector3Int>(); // this is the tiles we will be drawing
        List<Vector3Int> tileVectors;

        BoundsInt edgeDrawBounds = new BoundsInt();

        int xStart, length, yStart, height;

        #region Mirror Right Edge, Display on Left Edge

        xStart = 0 - MaxCameraHexWidth;
        length = MaxCameraHexWidth;

        yStart = 0;
        height = mapYHexCount;

        edgeDrawBounds.x = xStart;
        edgeDrawBounds.y = yStart;

        edgeDrawBounds.size = new Vector3Int(length, height, 1);

        tileVectors = GetEdgePositions(MapBounds.RightBound);

        logger1.StartTimer();

        RiverTilePlacer.DisplayRivers(ConvertBoundsToVectors(edgeDrawBounds),
            RiverTilePlacer.GetRiverTiles(tileVectors));

        CoastTilePlacer.DisplayCoasts(ConvertBoundsToVectors(edgeDrawBounds),
                CoastTilePlacer.GetCoastTiles(tileVectors));


        PlaceTiles(ConvertBoundsToVectors(edgeDrawBounds).ToArray(), GetBaseTiles(tileVectors).ToArray());

        #endregion

      //  logger1.LogTime("Left Edge");

        #region Mirror Left Edge, Display on Right Edge
        xStart = mapXHexCount;
        length = MaxCameraHexWidth;

        yStart = 0;
        height = mapYHexCount;

        edgeDrawBounds.x = xStart;
        edgeDrawBounds.y = yStart;

        edgeDrawBounds.size = new Vector3Int(length, height, 1);

        tileVectors = GetEdgePositions(MapBounds.LeftBound);

        RiverTilePlacer.DisplayRivers(ConvertBoundsToVectors(edgeDrawBounds),
                     RiverTilePlacer.GetRiverTiles(tileVectors));

        CoastTilePlacer.DisplayCoasts(ConvertBoundsToVectors(edgeDrawBounds),
        CoastTilePlacer.GetCoastTiles(tileVectors));

        PlaceTiles(ConvertBoundsToVectors(edgeDrawBounds).ToArray(), GetBaseTiles(tileVectors).ToArray());
        #endregion

      //  logger1.LogTime("Right Edge");

        #region Mirror Top Edge, Display on bot Edge

        xStart = 0;
        length = mapXHexCount;

        yStart = 0 - MaxCameraHexHeight;
        height = MaxCameraHexHeight;

        edgeDrawBounds.x = xStart;
        edgeDrawBounds.y = yStart;

        edgeDrawBounds.size = new Vector3Int(length, height, 1);

        tileVectors = GetEdgePositions(MapBounds.TopBound);

        RiverTilePlacer.DisplayRivers(ConvertBoundsToVectors(edgeDrawBounds),
            RiverTilePlacer.GetRiverTiles(tileVectors));

        CoastTilePlacer.DisplayCoasts(ConvertBoundsToVectors(edgeDrawBounds),
        CoastTilePlacer.GetCoastTiles(tileVectors));

        PlaceTiles(ConvertBoundsToVectors(edgeDrawBounds).ToArray(), GetBaseTiles(tileVectors).ToArray());

        #endregion

     //   logger1.LogTime("Bot Edge");

        #region Mirror Bot Edge, Display on top Edge

        xStart = 0;
        length = mapXHexCount;

        yStart = mapYHexCount;
        height = MaxCameraHexHeight;

        edgeDrawBounds.x = xStart;
        edgeDrawBounds.y = yStart;

        edgeDrawBounds.size = new Vector3Int(length, height, 1);

        tileVectors = GetEdgePositions(MapBounds.BottomBound);

        RiverTilePlacer.DisplayRivers(ConvertBoundsToVectors(edgeDrawBounds),
                RiverTilePlacer.GetRiverTiles(tileVectors), true);

        CoastTilePlacer.DisplayCoasts(ConvertBoundsToVectors(edgeDrawBounds),
        CoastTilePlacer.GetCoastTiles(tileVectors), true);

        PlaceTiles(ConvertBoundsToVectors(edgeDrawBounds).ToArray(), GetBaseTiles(tileVectors).ToArray(), true);
        #endregion

       // logger1.LogTime("Top Edge");

    }
    private List<LandScapeTile> GetBaseTiles(List<Vector3Int> tileVectors)
    {
        List<LandScapeTile> tiles = new List<LandScapeTile>();

        foreach (Vector3Int vector in tileVectors)
        {
            tiles.Add(baseTileMap.GetTile(vector) as LandScapeTile);
        }

        return tiles;
    }

    private List<River> GetRivers(List<Vector3Int> tileVectors)
    {
        // since not all tiles have rivers, some parts of the list will be null'
    // YOU MUST ACCOUNT FOR THAT!!!

        List<River> rivers = new List<River>();
        RiverData tempN;
        River aRiver = null;

        foreach (Vector3Int vector in tileVectors)
        {
            // this linq will find the FIRST element in Maprivers that has thesame location
            // if there is no such element, then it will return Null

            aRiver = MapRivers.FirstOrDefault(x => x.RiverPoints.TryGetValue(vector, out tempN) == true);

            rivers.Add(aRiver);
        }

        return rivers;
    }
    private List<Vector3Int> GetEdgePositions(MapBounds edgeToCopy)
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();

        int xStart, length, yStart, height;

        if (edgeToCopy == MapBounds.LeftBound)
        {
            xStart = 0;
            length = MaxCameraHexWidth;

            yStart = 0;
            height = mapYHexCount;

            for (int i = yStart; i < height; i++)
            {
                for (int j = xStart; j < xStart + length; j++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }
        else if (edgeToCopy == MapBounds.RightBound)
        {
            xStart = mapXHexCount - MaxCameraHexWidth;
            length = MaxCameraHexWidth;

            yStart = 0;
            height = mapYHexCount;

            for (int i = yStart; i < height; i++)
            {
                for (int j = xStart; j < xStart + length; j++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }
        else if (edgeToCopy == MapBounds.TopBound)
        {
            xStart = 0;
            length = mapXHexCount;

            yStart = mapYHexCount - MaxCameraHexHeight;
            height = MaxCameraHexHeight;

            for (int i = yStart; i < yStart + height; i++)
            {
                for (int j = xStart; j < length; j++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }
        else if (edgeToCopy == MapBounds.BottomBound)
        {
            xStart = 0;
            length = mapXHexCount;

            yStart = 0;
            height = MaxCameraHexHeight;

            for (int i = yStart; i < yStart + height; i++)
            {
                for (int j = xStart; j < length; j++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }

        return tileVectors;
    }
    private Vector3 GetmousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // its a 2d map

        return mouseWorldPos;
    }

    Vector3 prevMousePos = new Vector3(0, 0, 0);
    private bool MouseMoved()
    {
        // used for scrolling
        Vector3 currInput = GetRawMouseInput();

        float difference = Math.Abs(Vector3.Distance(currInput, prevMousePos));

        // mouse movement within the hex dont count
        if (difference < hexWidth)
        {
            return false;
        }
        else
        {
            prevMousePos = currInput;
            return true;
        }
    }
    private Vector3 GetRawMouseInput()
    {
        return Input.mousePosition;
    }
    private enum MapBounds { LeftBound, RightBound, TopBound, BottomBound, None }
    /// <summary>
    /// Use this to see if the camera is at the end of the bounds
    /// So that you can know when the change its position to the other side
    /// </summary>
    /// <returns></returns>
    private MapBounds GetMapEdge()
    {
        if (GetHexPositionAtLeftCamPos <= 2)
        {
            return MapBounds.LeftBound;
        }

        if (mapXHexCount - GetHexPositionAtRightCamPos <= 2)
        {
            return MapBounds.RightBound;
        }

        if (GetHexPositionAtCamBotEdgePos <= 3)
        {
            return MapBounds.BottomBound;
        }

        if (mapYHexCount - GetHexPositionAtCamTopEdgePos <= 3)
        {
            return MapBounds.TopBound;
        }

        return MapBounds.None;
    }

    private void SetCameraMovement()
    {
        if (CameraPosition != prevCamPosition)
        {
            _cameraMoving = true;

            if (CameraPosition.x <= prevCamPosition.x)
            {
                _cameraMovementDirection = MovementDirection.Left;
            }
            else
            {
                _cameraMovementDirection = MovementDirection.Right;
            }

            if (CameraPosition.y <= prevCamPosition.y)
            {
                _cameraMovementDirection = MovementDirection.Down;
            }
            else
            {
                _cameraMovementDirection = MovementDirection.Up;
            }

            prevCamPosition = CameraPosition;
        }
        else
        {
            _cameraMoving = false;
            _cameraMovementDirection = MovementDirection.None;
        }
    }

    /// <summary>
    /// we can add diagonal later
    /// </summary>
    public enum MovementDirection { Left, Right, Up, Down, None }

    private bool _cameraMoving = false;
    private MovementDirection _cameraMovementDirection = MovementDirection.None;
    private bool CameraIsMoving
    {
        get
        {
            return _cameraMoving;
        }
    }

    private MovementDirection CameraMovemetDirection
    {
        get
        {
            return _cameraMovementDirection;
        }
    }
    private float CameraSize
    {
        get
        {
            return mainCamera.orthographicSize;
        }
    }
    private float CameraHeight
    {
        get
        {
            return 2 * mainCamera.orthographicSize;
        }
    }
    private float CameraWidth
    {
        get
        {
            return CameraHeight * mainCamera.aspect;
        }
    }
    private float MaxCameraHeight
    {
        get
        {
            return 2 * maxOrthoSize;
        }
    }
    private float MaxCameraWidth
    {
        get
        {
            // im pretty sure camera aspect stays thesame         
            return MaxCameraHeight * mainCamera.aspect;
        }
    }

    private int MaxCameraHexWidth
    {
        get
        {
            return (int)(MaxCameraWidth / hexDistanceX);
        }
    }
    private int MaxCameraHexHeight
    {
        get
        {
            // im pretty sure camera aspect stays thesame         
            return (int)(MaxCameraHeight / hexDistanceY);
        }
    }

    private Vector3 CameraPosition
    {
        get
        {
            return mainCamera.transform.position;
        }
    }
    /// <summary>
    /// Gets the world X Position of the left edge
    /// </summary>
    private float GetCamLeftEdgePosition
    {
        get
        {
            return CameraPosition.x - CameraWidth / 2;
        }
    }
    /// <summary>
    /// Gets the world Y Position of the right edge
    /// </summary>
    private float GetCamRightEdgePosition
    {
        get
        {
            return CameraPosition.x + CameraWidth / 2;
        }
    }
    /// <summary>
    /// Gets the world Y Position of the top edge
    /// </summary>
    private float GetCamTopEdgePosition
    {
        get
        {
            return mainCamera.transform.position.y + CameraHeight / 2;
        }
    }
    /// <summary>
    /// Gets the world Y Position of the bot edge
    /// </summary>
    private float GetCamBotEdgePosition
    {
        get
        {
            return mainCamera.transform.position.y - CameraHeight / 2;
        }
    }
    /// <summary>
    /// Gets the tile map X Position of the left edge
    /// </summary>
    private int GetHexPositionAtLeftCamPos
    {
        get
        {
            return (int)(GetCamLeftEdgePosition / hexDistanceX);
        }
    }
    /// <summary>
    /// Gets the Tile map X Position of the right edge
    /// </summary>
    private int GetHexPositionAtRightCamPos
    {
        get
        {
            return (int)(GetCamRightEdgePosition / hexDistanceX);
        }
    }
    /// <summary>
    /// Gets the top Y Position of the top edge
    /// </summary>
    private int GetHexPositionAtCamTopEdgePos
    {
        get
        {
            return (int)(GetCamTopEdgePosition / hexDistanceY);
        }
    }
    /// <summary>
    /// Gets the bot Y Position of the bot edge
    /// </summary>
    private int GetHexPositionAtCamBotEdgePos
    {
        get
        {
            return (int)(GetCamBotEdgePosition / hexDistanceY);
        }
    }

    private int GetCamHeightHexCount
    {
        get
        {
            return (int)(CameraHeight / hexDistanceY);
        }
    }
    private int GetCamWidthHexCount
    {
        get
        {
            return (int)(CameraWidth / hexDistanceX);
        }
    }
    private int GetFractal()
    {
        float fractal = 0;

        fractal = mapYHexCount * mapXHexCount;

        fractal = Mathf.Sqrt(fractal);

        return Mathf.CeilToInt(fractal / 100);
    }
    public void PrintMousePos()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // its a 2d map

        Debug.Log("Mouse is at " + GetmousePosition().ToString());
    }

    private static class RiverTilePlacer
    {
        private static RiverSpriteManager SpriteManager;
        public static Tilemap RiverTileMap;

        private static Dictionary<Vector3Int,Neighbor> RiverPoints = new Dictionary<Vector3Int, Neighbor>();
        private static Dictionary<Vector3Int, Neighbor> LakePoints = new Dictionary<Vector3Int, Neighbor>();

        private static Dictionary<Vector3Int, int> RiverMouths = new Dictionary<Vector3Int, int>();

        //Make sure your river tile map is in the proper sorting order in tilemap renderer,
        /// or else other tile map features will be placed ontop of the rivers
        public static void Instantiate(List<River> allRivers, RiverSpriteManager riverSpriteManager)
        {
            foreach (River river in allRivers)
            {
                // they might be situations where an ocean body(one hex) may have multiple rivers
                // I currently want to avoid that. One hex should only 1 1 river mouth MAX
                // we must do our best to avoid that as much as possible

                // a river and a river spout cannot be placed on thesame tile

                RiverMouths.TryAdd(river.RiverMouth.Item1, river.RiverMouth.Item2);

                foreach (KeyValuePair<Vector3Int, RiverData> riverPoint in river.RiverPoints)
                {
                    SetRiverPoint(riverPoint.Key, riverPoint.Value.Neighbor);

                }
            }

            SpriteManager = riverSpriteManager;
        }

        private static void SetRiverPoint(Vector3Int pos, Neighbor neighbor)
        {
            // so at the end of this method, each vector will have a list containing 6 elements
            // the elements will be either 1 or 2
            // 1 = means yes, river flows from this position
            // 2 = means no, no river flows from this position

            if (RiverPoints.Keys.Contains(pos))
            {
                // We combine the given neighbor with the neighbor we already have
                RiverPoints[pos] = RiverPoints[pos].CombineNeighbor(neighbor);
            }
            else
            {
                RiverPoints.Add(pos, neighbor);
            }
        }
        public static void DisplayRivers()
        {
            return;
            LandScapeTile tempTile;

            foreach (Vector3Int pos in RiverPoints.Keys)
            {
                tempTile = SpriteManager.GetRiverTile(RiverPoints[pos].NeighborNumbers);

                //  riverTileMap.SetTile(pos, SpriteManager.GetHo());

                if (tempTile != null)
                {
                    RiverTileMap.SetTile(pos, tempTile);
                }
                else
                {
                    RiverTileMap.SetTile(pos, SpriteManager.GetHo());

                    //Debug.Log("Position " + pos.ToString() + 
                    //    " No River Tile" + RiverPoints[pos].ToString());
                }
            }

            foreach (Vector3Int pos in RiverMouths.Keys)
            {
                tempTile = SpriteManager.GetRiverMouthTile(RiverMouths[pos]);

                if (tempTile != null)
                {
                    RiverTileMap.SetTile(pos, tempTile);
                }
                else
                {
                    RiverTileMap.SetTile(pos, SpriteManager.GetHo());

                    Debug.Log("Position " + pos.ToString() +
                        " No Mouth Tile" + RiverPoints[pos].ToString());
                }
            }

            RiverTileMap.RefreshAllTiles();
        }
        public static void DisplayRivers(List<Vector3Int> positions, List<LandScapeTile> rivers, bool refresh = false)
        {
            RiverTileMap.SetTiles(positions.ToArray(), rivers.ToArray());

            if(refresh == true)
            {
                RiverTileMap.RefreshAllTiles();
            }
        }

        /// <summary>
        /// This will return a one for one list with all the river tiles,
        /// if a position does not have a river, then the tile will be null
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static List<LandScapeTile> GetRiverTiles(List<Vector3Int> positions)
        {
            List<LandScapeTile> riverTiles = new List<LandScapeTile>();
            LandScapeTile temp = null;

            foreach (Vector3Int pos in positions)
            {
                temp = RiverTileMap.GetTile(pos) as LandScapeTile;

                riverTiles.Add(temp);
            }

            return riverTiles;
        }


        public static void ResetRivers()
        {
            RiverPoints.Clear();
            RiverMouths.Clear();
        }
    }

    private static class CoastTilePlacer
    {
        private static CoastSpriteManager SpriteManager;
        public static Tilemap CoastTileMap;

        private static Dictionary<Vector3Int, Neighbor> CoastPoints = new Dictionary<Vector3Int, Neighbor>();

        //Make sure your coast tile map is in the proper sorting order in tilemap renderer,
        /// or else other tile map features will be placed ontop or below it...
        public static void Instantiate(List<Coast> coastLocatons, CoastSpriteManager coastSpriteManager)
        {
            coastLocatons.ForEach(x => CoastPoints.Add(x.Position, x.Neighbor));

            SpriteManager = coastSpriteManager;
        }
        public static void DisplayCoasts()
        {
            LandScapeTile tempTile;

            foreach (Vector3Int pos in CoastPoints.Keys)
            {
                tempTile = SpriteManager.GetCoastTile(CoastPoints[pos].NeighborNumbers);

                if (tempTile != null)
                {
                    CoastTileMap.SetTile(pos, tempTile);
                }
                else
                {
                    CoastTileMap.SetTile(pos, SpriteManager.GetTest());

                    //Debug.Log("Position " + pos.ToString() + 
                    //    " No River Tile" + RiverPoints[pos].ToString());
                }
            }

            CoastTileMap.RefreshAllTiles();
        }

        public static void DisplayCoasts(List<Vector3Int> positions, List<LandScapeTile> rivers, bool refresh = false)
        {
            CoastTileMap.SetTiles(positions.ToArray(), rivers.ToArray());

            if(refresh == true)
            {
                CoastTileMap.RefreshAllTiles();
            }
        }

        /// <summary>
        /// This will return a one for one list with all the coast tiles,
        /// if a position does not have a coast, then the tile will be null
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static List<LandScapeTile> GetCoastTiles(List<Vector3Int> positions)
        {
            List<LandScapeTile> coastTiles = new List<LandScapeTile>();
            LandScapeTile temp = null;

            foreach (Vector3Int pos in positions)
            {
                temp = CoastTileMap.GetTile(pos) as LandScapeTile;

                coastTiles.Add(temp);
            }

            return coastTiles;
        }

        public static void ResetCoasts()
        {
            CoastPoints.Clear();
        }
    }
}

