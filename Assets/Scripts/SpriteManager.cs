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
        // dont forget to instantiate the sprites if you declare new ones

        [SerializeField] private Ocean _oceans;
        [SerializeField] private Lake _lakes;
        [SerializeField] private Plains _plains;
        [SerializeField] private Desert _deserts;
        [SerializeField] private DryLands _dryLands;
        [SerializeField] private WetLands _wetLands;
        [SerializeField] private Forest _forest;

        [SerializeField] private Hills _hills;
        [SerializeField] private Highlands _highlands;
        [SerializeField] private Mountains _mountains;

        [SerializeField] private Hills _desertHills;
        [SerializeField] private Highlands _desertHighlands;
        [SerializeField] private Mountains _desertMountains;

        public void Instantiate(float hexScale)
        {
            // make sure all sprites have a PPU for 256
            // this method loops through all the sprites in the lists and changes their ppu
            // so the sprites could match the cell sizes given the current scale of the grid
            _hills.Instantiate(hexScale);
            _highlands.Instantiate(hexScale);
            _mountains.Instantiate(hexScale);

            _desertHills.Instantiate(hexScale);
            _desertHighlands.Instantiate(hexScale);
            _desertMountains.Instantiate(hexScale);

            _oceans.Instantiate(hexScale);
            _lakes.Instantiate(hexScale);

            _plains.Instantiate(hexScale);
            _plains.SetPlateaus(_mountains, _highlands, _hills);

            _deserts.Instantiate(hexScale);
            _deserts.SetPlateaus(_desertMountains, _desertHighlands, _desertHills);

            _forest.Instantiate(hexScale);
            _forest.SetPlateaus(_mountains, _highlands, _hills);

            _dryLands.Instantiate(hexScale);
            _dryLands.SetPlateaus(_desertMountains, _desertHighlands, _desertHills);

            _wetLands.Instantiate(hexScale);
            _wetLands.SetPlateaus(_mountains, _highlands, _hills);
        }

        public Ocean Oceans
        {
            get
            {
                return _oceans;
            }
        }

        public Lake Lakes
        {
            get
            {
                return _lakes;
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
