
using Assets.Scripts;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static LandScapes;

[CreateAssetMenu]
public class LandScapeTile : Tile
{
    [SerializeField] LandScape _landscape;
    [SerializeField] Temperature _temperature;
    [SerializeField] Color _highlightColor;



    private float _elevation = -1;

    private float _noise = 0;

    public void HighLight(bool mark)
    {
        if (mark)
        {
            // _highlightColor = Color.green;
        }
        else
        {
            //_highlightColor = Color.clear;
        }

    }
    public Color HighlightColor
    {
        get
        {
            return _highlightColor;
        }
    }

    public float Noise
    {
        get
        {
            return _noise;
        }
        set
        {
            _noise = value;
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // please dont call base.refresh method in this method,
        // your just creating a stack overflow
    }

    public static int GetOppositeNeighbor(int neighbor)
    {
        if (neighbor <= 3)
        {
            return neighbor + 3;
        }
        else
        {
            return neighbor - 3;
        }
    }

    private Vector3Int GetNeighborHex(Vector3Int curPos, int neighborSide, Vector2Int mapSize)
    {
        /// PLEASE BE ADVICED, THE Y POSITION OF THE HEX MATTERS
        /// WE HAVE TO ACCOUNT FOR CHANGES IF THE Y POSITION IS ODD OR EVEN
        Vector3Int tempPos = curPos;

        switch (neighborSide)
        {
            case 1:
                // increase both x and y by 1

                if (tempPos.y % 2 == 1)
                {
                    tempPos.x += 1;
                }

                tempPos.y += 1;

                break;
            case 2:

                tempPos.x += 1;

                break;
            case 3:

                if (tempPos.y % 2 == 1)
                {
                    tempPos.x += 1;
                }

                tempPos.y -= 1;

                break;
            case 4:

                if (tempPos.y % 2 == 0)
                {
                    tempPos.x -= 1;
                }

                tempPos.y -= 1;

                break;
            case 5:

                tempPos.x -= 1;
                break;

            default:
                // case 6

                if (tempPos.y % 2 == 0)
                {
                    tempPos.x -= 1;
                }

                tempPos.y += 1;
                break;
        }


        if (tempPos.x >= mapSize.x)
        {
            tempPos.x -= mapSize.x;
        }

        if (tempPos.x < 0)
        {
            tempPos.x += mapSize.x;
        }

        if (tempPos.y >= mapSize.y)
        {
            tempPos.y -= mapSize.y;
        }

        if (tempPos.y < 0)
        {
            tempPos.y += mapSize.y;
        }

        return tempPos;
    }

    /// <summary>
    /// Gets the neighbor hexes in order, from position 0 - 5
    /// </summary>
    /// <returns></returns>

    public Vector3Int[] GetNeighborPositions(Vector3Int curPos, Vector2Int mapSize)
    {
        Vector3Int[] neighbors = new Vector3Int[6];

        // we start at index 1 because out getNeighborHex is also index from the number 1
        for (int i = 1; i <= 6; i++)
        {
            neighbors[i - 1] = GetNeighborHex(curPos, i, mapSize);
        }

        return neighbors;
    }

    public LandScapeTile[] GetNeighborTiles(Vector3Int curPos, Vector2Int mapSize, Tilemap tileMap)
    {
        LandScapeTile[] neighbors = new LandScapeTile[6];

        // we start at index 1 because out getNeighborHex is also index from the number 1
        for (int i = 1; i <= 6; i++)
        {
            neighbors[i - 1] = tileMap.GetTile(GetNeighborHex(curPos, i, mapSize)) as LandScapeTile;
        }

        return neighbors;
    }

    public Temperature TileTemperature
    {
        get
        {
            return _temperature;
        }
        set
        {
            _temperature = value;
        }
    }

    public bool[] GetCoasts(Tilemap tileMap, Vector3Int curPos, Vector2Int mapSize)
    {
        bool[] coastSides = new bool[6];
        LandScapeTile curTile, nextTile;

        for (int i = 0; i < coastSides.Length; i++)
        {
            coastSides[i] = false;
        }

        curTile = tileMap.GetTile(curPos) as LandScapeTile;

        if (LandScapeIsSeaorOcean(curTile._landscape))
        {
            int i = 0;

            foreach (Vector3Int pos2 in curTile.GetNeighborPositions(curPos, mapSize))
            {
                nextTile = tileMap.GetTile(pos2) as LandScapeTile;
                // check if the neighbor tiles are land.
                // if they are then we have a coast
                if (LandScapeIsLand(nextTile._landscape))
                {
                    coastSides[i] = true;
                }

                i++;

            }

            return coastSides;

        }
        else
        {
            return coastSides;
        }
    }

    public bool LandScapeIsSeaorOcean()
    {
        switch (_landscape)
        {
            case LandScape.Ocean:
            case LandScape.Sea:
                return true;
            default:
                return false;
        }
    }

    public static bool LandScapeIsSeaorOcean(LandScape landscape)
    {
        switch (landscape)
        {
            case LandScape.Ocean:
            case LandScape.Sea:
                return true;
            default:
                return false;
        }
    }

    private bool LandScapeIsLand(LandScape landscape)
    {
        switch (landscape)
        {
            case LandScape.Ocean:
            case LandScape.Sea:
            case LandScape.Lake:
                return false;
            default:
                return true;
        }
    }

    public float Elevation
    {
        get
        {
            return _elevation;
        }
        set
        {
            _elevation = value;
        }
    }

    internal LandScape TileLandScape
    {
        get
        {
            return _landscape;
        }
    }


    [MenuItem("Assets/Create/2D/Custom Tiles/LandScapeTile")]
    public static void CreateAsset()
    {
        string path = "Assets/Tiles/LandTile.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LandScapeTile>(), path);
    }

}
