using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;
using System.Timers;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using static LandScapes;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{

    // Start is called before the first frame update
    // Make sure you set the sprites in the set sprite method

    private Ocean OceanTiles;
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

    [SerializeField] private Camera mainCamera;
    [SerializeField] public float hexScale;

    [SerializeField] private float baseCellSize;

    [SerializeField] private int mapXHexCount, mapYHexCount;

    [SerializeField] private int maxMountainCount;
    [SerializeField] private int maxMountainLength;
    [SerializeField] private int maxMountainWidth;

    public readonly struct Planet
    {
        public readonly int HexCountX { get; }
        public readonly int HexCountY { get; }

        public readonly float Width { get; }
        public readonly float Height { get; }

        public readonly int MountainHexWidth { get; }
        public readonly int MountainHexLength { get; }

        public readonly int NumOfMountains { get; }

        public readonly float Fractal;

        public readonly List<List<Vector2>> planetMountains;

        private readonly List<Vector2> mountains;

        public Planet(int mapXCount, int mapYCount, float mapWidth, float mapHeight,
            float fractal, int mtnLength, int mtnWidth, int mtnCount)
        {
            HexCountX = mapXCount;
            HexCountY = mapYCount;

            Height = mapHeight;
            Width = mapWidth;

            Fractal = fractal;

            MountainHexLength = mtnLength;
            MountainHexWidth = mtnWidth;

            mountains = new List<Vector2>();
            planetMountains = new List<List<Vector2>>();

            NumOfMountains = mtnCount;

            SetMountainHexes();
        }


        private void SetMountainHexes()
        {
            //int length;
            //int width;

            //int xPos;
            //int yPos;
            //int direction = 0;

            //List<Vector2> aMountain;
            //Vector2 newPosition, startVec;

            //for (int i = 0; i < NumOfMountains; i++)
            //{
            //    aMountain = new List<Vector2>();

            //    xPos = Random.Range(0, HexCountX);
            //    yPos = Random.Range(0, HexCountY);

            //    newPosition = new Vector2(xPos, yPos);
            //    mountains.Add(newPosition);

            //    length = Random.Range(1, MountainHexLength);

            //    for (int x = 0; x < length; x++)
            //    {
            //        // if use a while loop because the direction we roll, 
            //        // might already have a mountain and so we want to start over

            //        while (true)
            //        {
            //            direction = Random.Range(1, 7);

            //            newPosition = GetNeighborHex(direction, newPosition);

            //            if (!mountains.Contains(newPosition))
            //            {
            //                mountains.Add(newPosition);
            //                break;
            //            }

            //            if (MtnHexIsEnclosed(newPosition))
            //            {
            //                break;
            //            }
            //        }


            //        //width = yPos + Random.Range(1, MountainHexWidth);

            //        //for (int y = yPos; y < width; y++)
            //        //{
            //        //    mountains.Add(new Vector2(x, ));

            //        //}
            //    }
            //}
        }

        private bool MtnHexIsEnclosed(Vector2 hexPos)
        {
            Vector2 temp = new Vector2();

            for (int i = 1; i <= 6; i++)
            {
                temp = GetNeighborHex(i, hexPos);

                if (mountains.Contains(temp))
                {
                    return false;
                }

            }

            return true;
        }

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

    }

    GridNoiseManager noiseManager;
    SpriteManager gridSpriteManager;

    private float hexWidth, hexHeight;

    private readonly Vector3Int botLeftPos = new(0, 0, 0);
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
            if(map.name.Equals("BaseTileMap"))
            {
                baseTileMap = map as Tilemap;
            }

            if (map.name.Equals("CoastTileMap"))
            {
                coastTileMap = map as Tilemap ;
            }
        }

        gridSpriteManager = gameObject.GetComponent<SpriteManager>();


        //hexScale = Math.Clamp(hexScale, 1, 10);

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

    }

    Vector3Int prevTilePos;
    Color prevTileColor;

    Vector3Int curTilePos;
    Vector3 curMousePos;
    public void Update()
    {
        curTilePos = GetTilePosAtMousePos();
        curMousePos = GetmousePosition();

        bool hasTile = baseTileMap.HasTile(curTilePos);

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
        gridSpriteManager.Instantiate(hexScale);

        // each tile must have its own unique sprite, this is because each tile is its own
        // object. The tilemap has one instance of each tile(object)
        // even if 100 tiles of that instance are being displayed, t
        // they still refer to thesame instance. Thus, changing the sprite/data of one instance
        // also changes the data/sprites in all the other 100 instances

        OceanTiles = gridSpriteManager.Oceans;
        PlainTiles = gridSpriteManager.Plains;
        HillTiles = gridSpriteManager.Hills;
        HighlandTiles = gridSpriteManager.Highlands;
        MountainTile = gridSpriteManager.Mountains;
        DesertTile = gridSpriteManager.Deserts;

        DryLandsTile = gridSpriteManager.DryLands;
        WetLandsTile = gridSpriteManager.WetLands;
        ForestTile = gridSpriteManager.Forest;
    }

    // every time we change the map height, width etc, we wanna change respective values such as center etc
    private void SetMapValues()
    {
        centerTile = new Vector3Int(mapXHexCount / 2, mapYHexCount / 2);

        mapCenterPos = baseTileMap.CellToWorld(centerTile);
        mapCenterPos.z = -1; /// yes it needs to be -1 ---> for the camera

        mapWidth = mapCenterPos.x * 2;
        mapHeight = mapCenterPos.y * 2;

        MainPlanet = new Planet(mapXHexCount, mapYHexCount, mapWidth, mapHeight,
            GetFractal(), maxMountainLength, maxMountainWidth, maxMountainCount);

    }
    private void GridNoiseManager_changeScale(float scale)
    {
        SetMapValues();
        GenerateGrid();
    }

    DebugMenu logger = new DebugMenu();
    private void GenerateGrid()
    {
        //logger.StartTimer();

        noiseManager.ComputeNoise(MainPlanet);

        List<Vector3Int> tilePositions = new List<Vector3Int>();
        List<Tile> tiles = new List<Tile>();

        logger.StartTimer();

        for (int x = 0; x < mapXHexCount; x++)
        {
            tilePositions.Clear();
            tiles.Clear();

            for (int y = 0; y < mapYHexCount; y++)
            {
                tilePositions.Add(new Vector3Int(x, y));

                tiles.Add(GetBiomeToPlace(x, y));

            }

            PlaceTiles(tilePositions.ToArray(), tiles.ToArray());
        }

        baseTileMap.RefreshAllTiles();

        //logger.LogTime("Place Tiles: ");
    }

    // Temperature, Precipitaion
    //LandScapeType[,] biomeTable = new LandScapeType[3, 3]
    //{
    //    {LandScapeType.WoodLands, LandScapeType.Plains, LandScapeType.WoodLands,  },
    //    {LandScapeType.Jungle, LandScapeType.WoodLands, LandScapeType.Ocean,  },
    //};
    public Tile GetBiomeToPlace(int x, int y)
    {
        // based of Koppen climate classification, look up on wikipedia
        float tempNoise, preNoise, surlvlNoise, mtnLvlNoise;

        tempNoise = noiseManager.GetTempNoiseValue(x, y);
        preNoise = noiseManager.GetPrecipNoiseValue(x, y);
        surlvlNoise = noiseManager.GetSurfaceLevelNoiseValue(x, y);
        mtnLvlNoise = noiseManager.GetMountainLevelNoiseValue(x, y);

        Temperature temp = GetTemperature(tempNoise);
        Precipitation rain = GetPrecipitation(preNoise);
        GroundLevel grndLvl = GetGroundLevel(surlvlNoise);
        HeightLevel heightLvl = GetHeightLevel(mtnLvlNoise);

        Biome aBiome = GetBiome(temp, rain);

        // LandScapeType biomeType = biomeTable[(int)temp, (int)precip];
        // first determine if land or water
        // if water -- check for mountauns...maybe
        // if land - check height map
        // if hills --> mountains == display with proper temp
        // if plains
        // - check precipitation
        // display woodlands with proper temp

        switch (grndLvl)
        {
            case GroundLevel.BelowGround:

                return OceanTiles.GetRandomTile(temp);

            case GroundLevel.AboveGround:

                switch (aBiome)
                {
                    case Biome.Desert:
                        return DesertTile.GetRandomTile(temp, heightLvl);
                    case Biome.Plains:
                        return PlainTiles.GetRandomTile(temp, heightLvl);
                    case Biome.Forest:
                        return ForestTile.GetRandomTile(temp, heightLvl);
                    case Biome.DryLands:
                        return DryLandsTile.GetRandomTile(temp, heightLvl);
                    case Biome.WetLands:
                        return WetLandsTile.GetRandomTile(temp, heightLvl);
                    default:
                        return PlainTiles.GetRandomTile(temp, heightLvl);
                }
        }

        // now we must match temperature with their respective landscapes
        // so each landscape type has its own weather tiles.
        // for example, plain will have its own snow and desert tiles
        // thus snow and desert are not landscapes rather they are temperatures

        //switch (biomeType)
        //{
        //    case LandScapeType.Mountains:
        //        return mountainTile;
        //    case LandScapeType.Water:
        //        return waterTile;
        //    case LandScapeType.Plains:
        //        return plainTile;
        //    case LandScapeType.Desert:
        //        return desertTile;
        //    case LandScapeType.Forest:
        //        return forestTile;
        //    case LandScapeType.Jungle:
        //        return jungleTile;
        //}

        return null;
    }

    //public enum BiomeType
    //{
    //    Desert,
    //    Savanna,
    //    TropicalRainforest,
    //    Grassland,
    //    Woodland,
    //    SeasonalForest,
    //    TemperateRainforest,
    //    BorealForest,
    //    Tundra,
    //    Ice
    //};

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

    private GroundLevel GetGroundLevel(float levelNoise)
    {
        GroundLevel level;

        //Debug.Log("Level: " + levelNoise);

        switch (levelNoise)
        {
            case < .5f:
                level = GroundLevel.BelowGround;
                break;
            default:
                level = GroundLevel.AboveGround;
                break;
        }

        return level;
    }

    private HeightLevel GetHeightLevel(float levelNoise)
    {
        HeightLevel level;

        //Debug.Log("Level: " + levelNoise);

        switch (levelNoise)
        {
            case < .75f:
                level = HeightLevel.Flat;
                break;
            case < .9f:
                level = HeightLevel.Hill;
                break;
            case < .95f:
                level = HeightLevel.Highland;
                break;
            default:
                level = HeightLevel.Mountain;
                break;
        }

        return level;
    }


    public void PlaceTile(Vector3Int pos, Tile aTile)
    {
        baseTileMap.SetTile(pos, aTile);
        //tileMap.RefreshTile(pos);

    }
    public void PlaceTiles(Vector3Int[] pos, Tile[] tiles, bool refreshTiles = false)
    {
        baseTileMap.SetTiles(pos, tiles);

        if (refreshTiles == true)
        {
            baseTileMap.RefreshAllTiles();
        }
    }

    public void PlaceTilesBlock(BoundsInt bounds, Tile[] tiles)
    {
        baseTileMap.SetTilesBlock(bounds, tiles);
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
        maxTileViewed = 20;

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
    private Vector2 GetScrollMultiplier()
    {
        // we divide camera size because when zoomed in, the speed can be very fast/sensitive
        float scrollSpeed = 10f *
            MathFunctions.Normalize(CameraSize, minOrthoSize, maxOrthoSize, .05f, 1);

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

        // display opposite side of the map when user has reached the either edge of the map
        switch (GetMapEdgeX(position.x))
        {
            case MapEdgeX.LeftEdge:

                if (MouseMoved())
                {
                    SetLongitudeEdges(MapEdgeX.LeftEdge);
                }

                if (posX <= 0 - camWidth / 2 + hexDistanceX * 1.3f)
                {
                    posX = mapWidth - camWidth / 2;
                }

                break;
            case MapEdgeX.RightEdge:

                if (MouseMoved())
                {
                    SetLongitudeEdges(MapEdgeX.RightEdge);
                }

                if (posX >= mapWidth + camWidth / 2 - hexDistanceX * 1.3f)
                {
                    posX = 0 + camWidth / 2 - hexDistanceX;
                }

                break;
            default:
                break;
        }

        switch (GetMapEdgeY(position.y))
        {
            case MapEdgeY.BottomEdge:

                if (MouseMoved())
                {
                    SetLatitudeEdges(MapEdgeY.BottomEdge);
                }

                if (posY <= 0 - camHeight / 2 + hexDistanceY)
                {
                    posY = mapHeight - camHeight / 2;
                }

                break;
            case MapEdgeY.TopEdge:

                if (MouseMoved())
                {
                    SetLatitudeEdges(MapEdgeY.TopEdge);
                }

                if (posY >= mapHeight + camHeight / 2)
                {
                    posY = 0 + camHeight / 2 - hexDistanceY;
                }

                break;
            default:
                break;
        }

        mainCamera.transform.position = new Vector3(posX, posY, -1);
    }
    private void SetLongitudeEdges(MapEdgeX mapEdge)
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();
        List<Tile> tiles = GetOppositeLongitudeTiles(mapEdge);

        int length = GetCamWidthHexCount;

        if (mapEdge == MapEdgeX.RightEdge)
        {
            for (int j = mapXHexCount; j <= mapXHexCount + length; j++)
            {
                // we are drawing above the top of the map
                for (int i = GetHexPositionAtCamBotEdgePos; i <= GetHexPositionAtCamTopEdgePos; i++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }
        else if (mapEdge == MapEdgeX.LeftEdge)
        {
            // loop from left to right, first, then from top to bottom
            // row to row, column to column

            for (int j = -1; j >= 0 - length; j--)
            {   // we are drawing below the bottom of the map
                for (int i = GetHexPositionAtCamBotEdgePos; i <= GetHexPositionAtCamTopEdgePos; i++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }

        PlaceTiles(tileVectors.ToArray(), tiles.ToArray(), true);
    }
    private void SetLatitudeEdges(MapEdgeY mapEdge)
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();
        List<Tile> tiles = GetOppositeLatitudeTiles(mapEdge);

        int height = GetCamHeightHexCount;

        if (mapEdge == MapEdgeY.TopEdge)
        {
            // get position to draw outside tiles

            for (int j = mapYHexCount; j <= mapYHexCount + height; j++)
            {
                // we are drawing above the top of the map
                for (int i = GetHexPositionAtLeftCamPos; i <= GetHexPositionAtRightCamPos; i++)
                {
                    tileVectors.Add(new Vector3Int(i, j, 0));
                }
            }
        }
        else if (mapEdge == MapEdgeY.BottomEdge)
        {
            //position to draw tiles

            // loop from left to right, first, then from top to bottom
            // row to row, column to column
            for (int j = -1; j >= 0 - height; j--)
            {   // we are drawing below the bottom of the map
                for (int i = GetHexPositionAtLeftCamPos; i <= GetHexPositionAtRightCamPos; i++)
                {
                    tileVectors.Add(new Vector3Int(i, j, 0));
                }
            }
        }

        PlaceTiles(tileVectors.ToArray(), tiles.ToArray(), true);
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

    private List<Tile> GetTilesAtVector(List<Vector3Int> tileVectors)
    {
        List<Tile> tiles = new List<Tile>();

        foreach (Vector3Int vector in tileVectors)
        {
            tiles.Add(baseTileMap.GetTile(vector) as LandScapeTile);
        }

        return tiles;
    }
    private List<Tile> GetOppositeLatitudeTiles(MapEdgeY currentMapEdge)
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();

        int leftSide = GetHexPositionAtLeftCamPos;
        int rightSide = GetHexPositionAtRightCamPos;

        int height = GetCamHeightHexCount;

        if (currentMapEdge == MapEdgeY.TopEdge)
        {
            // get the tiles at bottom edges, with the lowest edge being first == so y = 0 is first in list

            for (int j = 1; j <= height; j++)
            {
                for (int i = leftSide; i <= rightSide; i++)
                {
                    tileVectors.Add(new Vector3Int(i, j, 0));
                }
            }
        }
        else if (currentMapEdge == MapEdgeY.BottomEdge)
        {
            // get the tiles at top edges, with the highests edge being first, so y = highestY is first in the list
            for (int j = mapYHexCount - 1; j >= mapYHexCount - height; j--)
            {
                for (int i = leftSide; i <= rightSide; i++)
                {
                    tileVectors.Add(new Vector3Int(i, j, 0));
                }
            }
        }

        return GetTilesAtVector(tileVectors);
    }
    private List<Tile> GetOppositeLongitudeTiles(MapEdgeX currentMapEdge)
    {
        List<Vector3Int> tileVectors = new List<Vector3Int>();

        int topSide = GetHexPositionAtCamTopEdgePos;
        int botSide = GetHexPositionAtCamBotEdgePos;

        int length = GetCamWidthHexCount;

        if (currentMapEdge == MapEdgeX.LeftEdge)
        {
            // get the tiles at right edges, 
            // tiles we be added to list on a row by row order
            // so all tiles in first row, then second row, etc..

            // get the tiles at right edges, from bottom to top
            for (int j = mapXHexCount - 1; j >= mapXHexCount - length; j--)
            {
                for (int i = botSide; i <= topSide; i++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }

        }
        else if (currentMapEdge == MapEdgeX.RightEdge)
        {
            // get the tiles at left edges, from bottom to top

            for (int j = 1; j <= length; j++)
            {
                for (int i = botSide; i <= topSide; i++)
                {
                    tileVectors.Add(new Vector3Int(j, i, 0));
                }
            }
        }

        return GetTilesAtVector(tileVectors);
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
    private enum MapEdgeX { LeftEdge, RightEdge, None }
    private MapEdgeX GetMapEdgeX(float camXPosition)
    {
        if (GetHexPositionAtLeftCamPos <= 2)
        {
            return MapEdgeX.LeftEdge;
        }

        if (mapXHexCount - GetHexPositionAtRightCamPos <= 2)
        {
            return MapEdgeX.RightEdge;
        }

        return MapEdgeX.None;
    }
    private enum MapEdgeY { TopEdge, BottomEdge, None }
    private MapEdgeY GetMapEdgeY(float camYPosition)
    {
        if (GetHexPositionAtCamBotEdgePos <= 2)
        {
            return MapEdgeY.BottomEdge;
        }

        if (mapYHexCount - GetHexPositionAtCamTopEdgePos <= 2)
        {
            return MapEdgeY.TopEdge;
        }

        return MapEdgeY.None;

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
    private float GetCamLeftEdgePosition
    {
        get
        {
            return mainCamera.transform.position.x - CameraWidth / 2;
        }
    }
    private float GetCamRightEdgePosition
    {
        get
        {
            return mainCamera.transform.position.x + CameraWidth / 2;
        }
    }
    private int GetHexPositionAtLeftCamPos
    {
        get
        {
            return (int)(GetCamLeftEdgePosition / hexDistanceX);
        }
    }
    private int GetHexPositionAtRightCamPos
    {
        get
        {
            return (int)(GetCamRightEdgePosition / hexDistanceX);
        }
    }
    private float GetCamTopEdgePosition
    {
        get
        {
            return mainCamera.transform.position.y + CameraHeight / 2;
        }
    }
    private float GetCamBotEdgePosition
    {
        get
        {
            return mainCamera.transform.position.y - CameraHeight / 2;
        }
    }
    private int GetHexPositionAtCamTopEdgePos
    {
        get
        {
            return (int)(GetCamTopEdgePosition / hexDistanceY);
        }
    }
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
}
