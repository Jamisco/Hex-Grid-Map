using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using static LandScapes;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    // This class creates tiles numbering the amount of sprites given to it
    // so if you give it, 4 sprites, it creates 4 unique tiles out of that sprite
    //
    public class TileContainer
    {
        [SerializeField] public List<Sprite> tileSprites = new List<Sprite>();
        private float Hexscale;
        private LandScapeTile tileAsset;
        public TileContainer(float hexScale, LandScapeTile tileAsset, List<Sprite> tileSprites, 
            Temperature temp = Temperature.Warm)
        {
            this.tileSprites = tileSprites;
            Instantiate(hexScale, tileAsset, temp);
        }

        private List<LandScapeTile> tiles;

        public LandScapeTile GetRandomTile()
        {
            return tiles[Random.Range(0, tiles.Count)];
        }

        public LandScapeTile GetTile(int index)
        {
            return tiles[index];
        }
        public void Instantiate(float hexScale, LandScapeTile tileAsset, Temperature temp = Temperature.Warm)
        {
            // make sure all sprites have a PPU for 256 INITIALLY!!!
            // this method loops through all the sprites in the lists and changes their ppu so the sprites could match the cell sizes given the current scale of the grid
            // the reason we cannot do this in the OnafterDeserialize() method is because that method does not allow for us to get and set any texture, including sprite textures
            // and no you cannot call this method in deserialize either

            Hexscale = hexScale;
            this.tileAsset = tileAsset;

            tileAsset.TileTemperature = temp;

            SetTiles();
        }
        private void SetTiles()
        {
            tiles = new List<LandScapeTile>();

            foreach (Sprite aSprite in tileSprites)
            {
                tileAsset.sprite = aSprite;

                // creates a new tile by cloning the tile asset with the current sprite
                tiles.Add(UnityEngine.Object.Instantiate(tileAsset));
            }
        }

        public void AddSprite(Sprite sprite)
        {
            tileSprites.Add(sprite);

            tileAsset.sprite = sprite;
            tiles.Add(UnityEngine.Object.Instantiate(tileAsset));
        }

        public void AddSprites(List<Sprite> sprites)
        {
            foreach (Sprite sprite in sprites)
            {
                AddSprite(sprite);
            }
        }

        private static Vector2 newPivot = new(0, 0); // this is = pivot.bottom in inspector
        public static Sprite ChangePPU(Sprite aSprite, float hexScale)
        {
            // matches the sprite ppu to the hex grid cells, because when we change the scale of the cells, the ppu for the sprite must also change
            return Sprite.Create(aSprite.texture, aSprite.rect, newPivot, aSprite.pixelsPerUnit / hexScale, 1, SpriteMeshType.Tight, aSprite.border);

        }
    }
}
