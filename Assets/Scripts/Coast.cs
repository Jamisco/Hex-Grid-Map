using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static GridManager;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts
{
    internal class Coast
    {
        private static Planet mainPlanet;
        private static Tilemap planetMap;

        private Neighbor _neighbor;
        private Vector3Int _position;


        public Coast(Vector3Int position, Neighbor aNeighbor)
        {
            _position = position;
            _neighbor = aNeighbor;
        }
        public Coast(Vector3Int position, List<int> neighbors)
        {
            _position = position;
            _neighbor = new Neighbor(neighbors);
        }
        public static List<Coast> CreateCoasts(List<Vector3Int> waterPositions)
        {
            List<Coast> coasts = new List<Coast>();
            LandScapeTile tempTile;
            Neighbor tempNeighbor;

            foreach (Vector3Int pos in waterPositions)
            {
                tempTile = planetMap.GetTile(pos) as LandScapeTile;
                tempNeighbor = tempTile.GetPossibleCoasts(ref planetMap, pos, mainPlanet.MapSize);

                // we make sure the current water tile has a coast
                if(tempNeighbor.IsConnected)
                {
                    // the Temperate Enum is order from freezing - very hot

                    if ((int)tempTile.TileTemperature > 2)
                    {
                        coasts.Add(new Coast(pos, tempNeighbor));
                    }
                    
                }
            }

            return coasts;
        }

        public Neighbor Neighbor
        {
            get
            {
                return _neighbor;
            }
        }

        public Vector3Int Position
        {
            get
            {
                return _position;
            }
        }

        public static void SetPlanet(Planet planet)
        {
            mainPlanet = planet;

            planetMap = planet.PlanetMap;
        }
    }
}
