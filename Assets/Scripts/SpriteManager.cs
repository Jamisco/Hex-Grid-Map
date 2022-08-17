using System;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    //  if your tiles arent anchoring properly, make sure the tilemap pivot is
    // x = -.5, y = -2/3
    internal class SpriteManager : MonoBehaviour
    {
        [SerializeField] private Ocean _oceans;
        [SerializeField] private Plains _plains;
        [SerializeField] private Hills _hills;
        [SerializeField] private Highlands _highlands;
        [SerializeField] private Mountains _mountains;
        [SerializeField] private Desert _deserts;
        [SerializeField] private DryLands _dryLands;
        [SerializeField] private WetLands _wetLands;
        [SerializeField] private Forest _forest;

        public void Instantiate(float hexScale)
        {
            // make sure all sprites have a PPU for 256
            // this method loops through all the sprites in the lists and changes their ppu so the sprites could match the cell sizes given the current scale of the grid
            // the reason we cannot do this in the OnafterDeserialize() method is because that method does not allow for us to get and set any texture, including sprite textures
            // and no you cannot call this method in deserialize either

            _oceans.Instantiate(hexScale);
            _plains.Instantiate(hexScale);
            _hills.Instantiate(hexScale);
            _highlands.Instantiate(hexScale);
            _mountains.Instantiate(hexScale);
            _deserts.Instantiate(hexScale);

            _dryLands.Instantiate(hexScale);
            _wetLands.Instantiate(hexScale);
            _forest.Instantiate(hexScale);

        }

        public Ocean Oceans
        {
            get
            {
                return _oceans;
            }
        }
        public Plains Plains
        {
            get
            {
                return _plains;
            }
        }
        public Hills Hills
        {
            get
            {
                return _hills;
            }
        }
        public Highlands Highlands
        {
            get
            {
                return _highlands;
            }
        }
        public Mountains Mountains
        {
            get
            {
                return _mountains;
            }
        }
        public Desert Deserts
        {
            get
            {
                return _deserts;
            }
        }

        public DryLands DryLands
        {
            get
            {
                return _dryLands;
            }
        }
        public WetLands WetLands
        {
            get
            {
                return _wetLands;
            }
        }
        public Forest Forest
        {
            get
            {
                return _forest;
            }
        }

    }
}
