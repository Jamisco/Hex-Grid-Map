
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

    List<Sprite> _sprites;

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

    private Vector3Int GetNeighborHex(Vector3Int curPos, int neighborSide, Vector2Int mapSize)
    {
        Vector3Int neighborPosition = curPos;

        switch (neighborSide)
        {
            case 1:
                // increase both x and y by 1

                if (neighborPosition.y % 2 == 1)
                {
                    neighborPosition.x += 1;
                }

                neighborPosition.y += 1;

                break;
            case 2:

                neighborPosition.x += 1;

                break;
            case 3:

                if (neighborPosition.y % 2 == 1)
                {
                    neighborPosition.x += 1;
                }

                neighborPosition.y -= 1;

                break;
            case 4:

                if (neighborPosition.y % 2 == 0)
                {
                    neighborPosition.x -= 1;
                }

                neighborPosition.y -= 1;

                break;
            case 5:

                neighborPosition.x -= 1;
                break;

            default:
                // case 6

                if (neighborPosition.y % 2 == 0)
                {
                    neighborPosition.x -= 1;
                }

                neighborPosition.y += 1;
                break;
        }

        if (neighborPosition.x >= mapSize.x)
        {
            neighborPosition.x -= mapSize.x;
        }

        if (neighborPosition.x < 0)
        {
            neighborPosition.x += mapSize.x;
        }

        if (neighborPosition.y >= mapSize.y)
        {
            neighborPosition.y -= mapSize.y;
        }

        if (neighborPosition.y < 0)
        {
            neighborPosition.y += mapSize.y;
        }

        return neighborPosition;
    }

    /// <summary>
    /// Gets the neighbor hexes in order, from position 0 - 5
    /// </summary>
    /// <returns></returns>

    public Vector3Int[] GetNeighborHexes(Vector3Int curPos, Vector2Int mapSize)
    {
        Vector3Int[] neighbors = new Vector3Int[6];

        // we start at index 1 because out getNeighborHex is also index from the number 1
        for (int i = 1; i <= 6; i++)
        {
            neighbors[i - 1] = GetNeighborHex(curPos, i, mapSize);
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

            foreach (Vector3Int pos2 in curTile.GetNeighborHexes(curPos, mapSize))
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

    private bool LandScapeIsSeaorOcean(LandScape landscape)
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
