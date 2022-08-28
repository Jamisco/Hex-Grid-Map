using System;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    internal class CoastSpriteManager : MonoBehaviour
    {
        [SerializeField] private TileContainer _coast;
        [SerializeField] private LandScapeTile coastAsset;

        public void Instantiate(float hexScale)
        {
            // make sure all sprites have a PPU for 256
            // this method loops through all the sprites in the lists and changes their ppu so the sprites could match the cell sizes given the current scale of the grid
            // the reason we cannot do this in the OnafterDeserialize() method is because that method does not allow for us to get and set any texture, including sprite textures
            // and no you cannot call this method in deserialize either

            _coast.Instantiate(hexScale, coastAsset);

        }

        public TileContainer Coasts
        {
            get
            {
                return _coast;
            }
        }
    }
}
