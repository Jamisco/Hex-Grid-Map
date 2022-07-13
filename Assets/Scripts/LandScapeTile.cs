
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Assets.Scripts.SpriteManager;

[CreateAssetMenu]
public class LandScapeTile : Tile
{
    [SerializeField] LandScapeType _landScape;
    [SerializeField] string _landScapeName;
    [SerializeField] Color _highlightColor;
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

    //public Sprite TileSprite
    //{
    //    get
    //    {
    //        return sprite;
    //    }
    //    set 
    //    {
    //        sprite = value;
    //    }
    //}

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // please dont call base.refresh method in this method,
        // your just creating a stack overflow
    }


    public LandScapeType LandScape
    {
        get
        {
            return _landScape;
        }
    }


    [MenuItem("Assets/Create/2D/Custom Tiles/LandScapeTile")]
    public static void CreateAsset()
    {
        string path = "Assets/Tiles/LandTile.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LandScapeTile>(), path);
    }

    /// <summary>
    /// BEWARE CHANGING THE ORDER OF THESE ENUMS WILL ALSO CHANGE THE TILE OBJECTS IN UNITY EDITOR!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    public enum LandScapeType
    {
        Water, Plains, Desert, Forest, Moutains, Jungle, Snow, Unknown
    }


}
