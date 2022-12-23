using System;
using System.Collections.Generic;
using UnityEngine;
using static LandScapeTile;

namespace Assets.Scripts
{
    internal class CoastSpriteManager : MonoBehaviour
    {
        [SerializeField] private LandScapeTile coastAsset;
        [SerializeField] private ConnectedSprites connectedCoastSprites;

        [SerializeField] HexagonalRuleTile coastSpriteRules;

        public void Instantiate(float hexScale)
        {
            connectedCoastSprites.Instantiate(hexScale, coastAsset, coastSpriteRules);
        }

        public LandScapeTile GetCoastTile(List<int> neighbors)
        {
            return connectedCoastSprites.GetRandomTile(neighbors);
        }

        public LandScapeTile GetTest()
        {
            return connectedCoastSprites.GetTest();
        }
    }
}
