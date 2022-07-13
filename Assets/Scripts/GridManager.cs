using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;
using System.Timers;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using static LandScapeTile;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update

    // Make sure you set the sprites in the set sprite method
    [SerializeField] private LandScapeTile waterTile;
    [SerializeField] private LandScapeTile plainTile;
    [SerializeField] private LandScapeTile desertTile;
    [SerializeField] private LandScapeTile forestTile;
    [SerializeField] private LandScapeTile mountainTile;
    [SerializeField] private LandScapeTile jungleTile;
    [SerializeField] private LandScapeTile snowTile;

    public LandScapeTile aTile;

    // Start is called before the first frame update

    // Make sure you set the sprites in the set sprite method

    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Grid baseGrid;

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

        public readonly float Fractal;

        public Planet(int mapXCount, int mapYCount, float mapWidth, float mapHeight, float fractal)
        {
            HexCountX = mapXCount;
            HexCountY = mapYCount;

            Height = mapHeight;
            Width = mapWidth;

            Fractal = fractal;
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
        tileMap = baseGrid.GetComponentInChildren<Tilemap>();
        gridSpriteManager = gameObject.GetComponent<SpriteManager>();


        //hexScale = Math.Clamp(hexScale, 1, 10);

        baseCellSize *= (int)hexScale;

        hexSize = baseCellSize;

        hexWidth = hexSize * 2;
        hexHeight = hexWidth * .75f; // at scale of 3 this should be 4.5

        hexDistanceX = hexHeight * 2; // at scale of 3 this should be 9
        hexDistanceY = hexDistanceX * .75f; // at scale of 3 this should be 6

        baseGrid.cellSize = new Vector2(baseCellSize, baseCellSize);

        tileMap.transform.localScale = new Vector2(hexScale, hexScale);

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

        bool hasTile = tileMap.HasTile(curTilePos);

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

            CheckSideScroll();
        }
    }

    private void SetSprites()
    {
        gridSpriteManager.SetSpritePPU(hexScale);

        plainTile.sprite = gridSpriteManager.GetSprite(plainTile.LandScape);
        forestTile.sprite = gridSpriteManager.GetSprite(forestTile.LandScape);
        waterTile.sprite = gridSpriteManager.GetSprite(waterTile.LandScape);
        jungleTile.sprite = gridSpriteManager.GetSprite(jungleTile.LandScape);
        mountainTile.sprite = gridSpriteManager.GetSprite(mountainTile.LandScape);
        desertTile.sprite = gridSpriteManager.GetSprite(desertTile.LandScape);
        snowTile.sprite = gridSpriteManager.GetSprite(snowTile.LandScape);
        aTile = plainTile;
    }

    // every time we change the map height, width etc, we wanna change respective values such as center etc
    private void SetMapValues()
    {
        centerTile = new Vector3Int(mapXHexCount / 2, mapYHexCount / 2);

        mapCenterPos = tileMap.CellToWorld(centerTile);
        mapCenterPos.z = -1; /// yes it needs to be -1 ---> for the camera

        mapWidth = mapCenterPos.x * 2;
        mapHeight = mapCenterPos.y * 2;

        MainPlanet = new Planet(mapXHexCount, mapYHexCount, mapWidth, mapHeight, GetFractal());

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

        tileMap.RefreshAllTiles();

        //logger.LogTime("Place Tiles: ");
    }
    public Tile GetBiomeToPlace(int x, int y)
    {
        // based of Koppen climate classification, look up on wikipedia
        float tempNoise, preNoise, surlvlNoise;

        tempNoise = noiseManager.GetTempNoiseValue(x, y);
        preNoise = noiseManager.GetPrecipNoiseValue(x, y);
        surlvlNoise = noiseManager.GetSurfaceLevelNoiseValue(x, y);

        Temperature temp = GetTemperature(tempNoise);
        Precipitation precip = GetPrecipitation(preNoise);
        SurfaceLevel surLvl = GetSurfaceLevel(surlvlNoise);

        LandScapeType biomeType = biomeTable[(int)temp, (int)precip];

        switch (surLvl)
        {
            case SurfaceLevel.BelowGround:
                return waterTile;
        }

        switch (temp)
        {
            case Temperature.Hot:
                return desertTile;
            case Temperature.Warm:
                return plainTile; ;
            case Temperature.Cold:
                return snowTile; ;
            default:
                return mountainTile;
        }



        //switch (biomeType)
        //{
        //    case LandScapeType.Water:
        //        return mountainTile;
        //    case LandScapeType.Plains:
        //        return plainTile;
        //    case LandScapeType.Desert:
        //        return desertTile;
        //    case LandScapeType.Forest:
        //        return forestTile;
        //    case LandScapeType.Jungle:
        //        return jungleTile;
        //    default:
        //        return mountainTile;
        //}
    }

    private enum Temperature { Hot, Warm, Cold, None };
    private enum Precipitation { Dry, Moist, Wet, None };
    private enum SurfaceLevel { BelowGround, GroundLevel };
    public enum BiomeType
    {
        Desert,
        Savanna,
        TropicalRainforest,
        Grassland,
        Woodland,
        SeasonalForest,
        TemperateRainforest,
        BorealForest,
        Tundra,
        Ice
    };

    private Temperature GetTemperature(float tempNoise)
    {
        Temperature temp;

        switch (tempNoise)
        {
            case < .2f:
                temp = Temperature.Cold;
                break;
            case < .8f:
                temp = Temperature.Warm;
                break;
            default:
                temp = Temperature.Hot;
                break;
        }

        return temp;
    }

    private Precipitation GetPrecipitation(float precipNoise)
    {
        Precipitation precip;

        switch (precipNoise)
        {
            case < .2f:
                precip = Precipitation.Dry;
                break;
            case < .70f:
                precip = Precipitation.Moist;
                break;
            default:
                precip = Precipitation.Wet;
                break;
        }

        return precip;
    }

    private SurfaceLevel GetSurfaceLevel(float levelNoise)
    {
        SurfaceLevel level;

        //Debug.Log("Level: " + levelNoise);

        switch (levelNoise)
        {
            case < .4f:
                level = SurfaceLevel.BelowGround;
                break;
            default:
                level = SurfaceLevel.GroundLevel;
                break;
        }

        return level;
    }

    // Temperature, Precipitaion
    LandScapeType[,] biomeTable = new LandScapeType[3, 3]
    {
        {LandScapeType.Desert, LandScapeType.Desert, LandScapeType.Plains,  },
        {LandScapeType.Forest, LandScapeType.Plains, LandScapeType.Forest,  },
        {LandScapeType.Jungle, LandScapeType.Forest, LandScapeType.Water,  },
    };

    public void PlaceTile(Vector3Int pos, Tile aTile)
    {
        tileMap.SetTile(pos, aTile);
        //tileMap.RefreshTile(pos);

    }
    public void PlaceTiles(Vector3Int[] pos, Tile[] tiles)
    {
        tileMap.SetTiles(pos, tiles);
    }

    public void PlaceTilesBlock(BoundsInt bounds, Tile[] tiles)
    {
        tileMap.SetTilesBlock(bounds, tiles);
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
            scrollPos = tileMap.CellToWorld(scrollTilePos);

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
    enum SideScrollDirection { North, South, East, West, None }
    private void CheckSideScroll()
    {
        float mousePosX = GetRawMouseInput().x;
        float mousePosY = GetRawMouseInput().y;

        SideScrollDirection NorthOrSouth = SideScrollDirection.None;
        SideScrollDirection EastOrWest = SideScrollDirection.None;

        if (mousePosY > Screen.height * .95)
        {
            NorthOrSouth = SideScrollDirection.North;
        }
        else if (mousePosY < Screen.height * .05)
        {
            NorthOrSouth = SideScrollDirection.South;
        }

        if (mousePosX > Screen.width * .95)
        {
            EastOrWest = SideScrollDirection.East;

        }
        else if (mousePosX < Screen.width * .05)
        {
            EastOrWest = SideScrollDirection.West;
        }

        SideScroll(NorthOrSouth, EastOrWest);
    }
    private void SideScroll(SideScrollDirection verticalDirection, SideScrollDirection horizontallDirection)
    {
        float scrollSpeed = 2f;

        Vector3 currCamPos = mainCamera.transform.position;

        float posX, posY;

        posX = currCamPos.x;
        posY = currCamPos.y;

        if (horizontallDirection == SideScrollDirection.East)
        {
            posX += scrollSpeed;
        }
        else if (horizontallDirection == SideScrollDirection.West)
        {
            posX -= scrollSpeed;
        }

        if (verticalDirection == SideScrollDirection.North)
        {
            posY += scrollSpeed;
        }
        else if (verticalDirection == SideScrollDirection.South)
        {
            posY -= scrollSpeed;
        }


        posX = Mathf.Clamp(posX, barrierX, mapWidth - barrierX);
        posY = Mathf.Clamp(posY, barrierY, mapHeight - barrierY);

        mainCamera.transform.position = new Vector3(posX, posY, -1);

        HightLightTIles(GetTilesAtCameraPos());
    }
    private Color HighLightTile(Vector3Int curPos, Vector3Int prevPos, Color prevColor)
    {
        Color curColor = tileMap.GetColor(curPos);

        LandScapeTile curTile = tileMap.GetTile(curPos) as LandScapeTile;

        tileMap.SetColor(curPos, curTile.HighlightColor);

        try
        {
            // is prevTile is null or some random value that isnt on the map
            // this is because it is not yet initialized with a proper map value
            // color white is the transparency color for all sprites
            tileMap.SetColor(prevPos, Color.white);
        }
        catch (Exception)
        {
            return curColor;
        }

        return curColor;
    }
    private void HightLightTIles(List<Vector3Int> tiles)
    {
        LandScapeTile curTile;

        foreach (Vector3Int pos in tiles)
        {
            curTile = tileMap.GetTile(pos) as LandScapeTile;
            tileMap.SetColor(pos, curTile.HighlightColor);
        }
    }
    private Vector3Int GetTilePosAtMousePos()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // its a 2d map

        try
        {
            return tileMap.WorldToCell(mouseWorldPos);
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
            return tileMap.WorldToCell(mouseWorldPos);
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

        int yDistance = (int) hexDistanceY * 2;

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

        for(int i = leftSide; i <= rightSide; i++)
        {
            for(int j = bottomSide ; j <= bottomSide + heightHexCount; j++)
            {
                tileVectors.Add(new Vector3Int(i, j, 0));
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
