using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    //  if your tiles arent anchoring properly, make sure the tilemap pivot is
    // x = -.5, y = -2/3
    public class SpriteManager : MonoBehaviour
    {
        // make sure your sprites and their respective ENUMS MATCH IN COUNT AND ORDER!!!!!!!!!!!!!!!!!

        [SerializeField]
        private Sprite[] spriteValues;

        [SerializeField]
        private LandScapeType[] landScapeKeys;

        private Dictionary<LandScapeType, Sprite> spriteMap = new Dictionary<LandScapeType, Sprite>();

        public Sprite GetSprite(LandScapeType key)
        {

            try
            {
                return spriteMap[key];
            }
            catch (Exception)
            {
                throw;
            }

        }

        //void ISerializationCallbackReceiver.OnBeforeSerialize()
        //{
        //}
        //void ISerializationCallbackReceiver.OnAfterDeserialize()
        //{
        //    Debug.Log("After");

        //    for (int i = 0; i < spriteValues.Length; i++)
        //    {
        //        spriteMap.TryAdd(landScapeKeys[i], spriteValues[i]);
        //    }

        //}

        public void SetSpritePPU(float hexScale)
        {
            // make sure all sprites have a PPU for 256
            // this method loops through all the sprites in the dictionary and changes their ppu so the sprites could match the cell sizes given the current scale of the grid
            // the reason we cannot do this in the OnafterDeserialize() method is because that method does not allow for us to get and set any texture, including sprite textures
            // and no you cannot call this method in deserialize either

            for (int i = 0; i < spriteValues.Length; i++)
            {
                Sprite aSprite = spriteValues[i];

                Vector2 newPivot = new(0, 0); // this is = pivot.bottom in inspector

                // matches the sprite ppu to the hex grid cells, because when we change the scale of the cells, the ppu for the sprite must also change
                Sprite newSprite = Sprite.Create(aSprite.texture, aSprite.rect, newPivot, aSprite.pixelsPerUnit / hexScale, 1, SpriteMeshType.Tight, aSprite.border);

                spriteMap.TryAdd(landScapeKeys[i], newSprite);
            }
        }
    }
}
