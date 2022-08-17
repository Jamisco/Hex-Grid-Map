
using Assets.Scripts;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static LandScapes;

[CreateAssetMenu]
public class LandScapeTile : Tile
{
    [SerializeField] LandScape _landScape;
    [SerializeField] Temperature _temperature;
    [SerializeField] Color _highlightColor;

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

    public LandScape TileLandScape
    {
        get
        {
            return _landScape;
        }
    }
    public enum LandScape
    {
        /// <summary>
        /// BEWARE CHANGING THE ORDER OF THESE ENUMS WILL ALSO 
        /// CHANGE THE TILE OBJECTS IN UNITY EDITOR!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// 
        Ocean, Plains, WoodLands, Mountains, Highlands, Hills, Unknown
    }



    [MenuItem("Assets/Create/2D/Custom Tiles/LandScapeTile")]
    public static void CreateAsset()
    {
        string path = "Assets/Tiles/LandTile.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LandScapeTile>(), path);
    }

}
