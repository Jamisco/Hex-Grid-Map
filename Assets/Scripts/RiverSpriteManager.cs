using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.ConnectedSprites;

namespace Assets.Scripts
{
    internal class RiverSpriteManager : MonoBehaviour
    {
        [SerializeField] private LandScapeTile riverAsset;
        [SerializeField] private ConnectedSprites connectedRiverSprites;
        [SerializeField] private ConnectedSprites connectedRiverMouthSprite;

        [SerializeField] HexagonalRuleTile riverSpriteRules;
        [SerializeField] HexagonalRuleTile riverMouthRules;

        public void Instantiate(float hexScale)
        {
            connectedRiverSprites.Instantiate(hexScale, riverAsset, riverSpriteRules);
            connectedRiverMouthSprite.Instantiate(hexScale, riverAsset, riverMouthRules);
        }

        public LandScapeTile GetRiverTile(List<int> neighbors)
        {
            return connectedRiverSprites.GetRandomTile(neighbors);
        }

        public LandScapeTile GetRiverMouthTile(int neighbor)
        {
            return connectedRiverMouthSprite.GetRandomTile(neighbor);
        }

        public LandScapeTile GetHo()
        {
            return connectedRiverSprites.GetTest();
        }
    }
}
