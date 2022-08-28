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
    public class TileContainer
    {
        [SerializeField] public List<Sprite> tileSprites = new List<Sprite>();

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

            tileAsset.TileTemperature = temp;

            for (int i = 0; i < tileSprites.Count; i++)
            {
                tileSprites[i] = ChangePPU(tileSprites[i], hexScale);
            }

            SetTiles(tileAsset);
        }
        private void SetTiles(LandScapeTile tileAsset)
        {
            tiles = new List<LandScapeTile>();

            foreach (Sprite aSprite in tileSprites)
            {
                tileAsset.sprite = aSprite;
                tiles.Add(UnityEngine.Object.Instantiate(tileAsset));
            }
        }

        private Vector2 newPivot = new(0, 0); // this is = pivot.bottom in inspector
        private Sprite ChangePPU(Sprite aSprite, float hexScale)
        {
            // matches the sprite ppu to the hex grid cells, because when we change the scale of the cells, the ppu for the sprite must also change
            return Sprite.Create(aSprite.texture, aSprite.rect, newPivot, aSprite.pixelsPerUnit / hexScale, 1, SpriteMeshType.Tight, aSprite.border);

        }
    }
}
