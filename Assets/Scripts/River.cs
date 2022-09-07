using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GridManager;

namespace Assets.Scripts
{
    #region What is a River Class
    /* The River class is simply a list of vectors that
     * contain the position of a river from the river source to the rivers end
     * That is all it is, a position of vectors
     * Each vector or position in the river list will also have a ruling
     * This ruling dictates the hex sides the rivers touches
     * 
     */
    #endregion
    public class River
    {
        public static Planet mainPlanet;
        private Tilemap planetMap;

        private List<Vector3Int> riverPositions = new List<Vector3Int>();

        public Dictionary<Vector3Int, List<int>> RiverPoints = new Dictionary<Vector3Int, List<int>>();

        public (Vector3Int, int) RiverMouth;

        public River(Vector3Int riverSource)
        {
            planetMap = mainPlanet.PlanetMap;

            CreateRiver(riverSource);
        }

        // the last position of every rver will be a lake, and the lake
        // the lake will only 1 neighbor - this neighbor will be the side for the river mouth
        // remember the River simply returns then list with the exact NEIGHBORS
        // so if the river flows from n2 - n5, the list will contain the numbers 2 and 6
        private void CreateRiver(Vector3Int riverSource)
        {
            int nextNeighbor = -1;
            int prevNeighbor = -1;

            Vector3Int currentPosition = riverSource;
            Vector3Int nextPosition;
            LandScapeTile tempTile;

            List<int> neighbors = new List<int>();

            float currentElevation = mainPlanet
                .NoiseManager.GetMountainLevelNoiseValue(currentPosition.x, currentPosition.y);

            while (true)
            {
                neighbors = new List<int>();

                nextPosition = GetLowestNeighbor(currentPosition, ref currentElevation, ref nextNeighbor );

                if(prevNeighbor != -1)
                {
                    // neighbor 2 on the old hex is neighbor 5 in the current tile
                    neighbors.Add(LandScapeTile.GetOppositeNeighbor(prevNeighbor));
                }

                neighbors.Add(nextNeighbor);

                if (currentPosition == nextPosition)
                {
                    // This means we have reached an uphill

                    break;
                }

                tempTile = planetMap.GetTile(currentPosition) as LandScapeTile;

                if(tempTile.LandScapeIsSeaorOcean())
                {
                    //  This means we have reached the sea or ocean
                    if(!riverPositions.Contains(nextPosition))
                    {

                    }

                    break; 
                }
                else
                {
                    // river is still going downhill
                    if (!riverPositions.Contains(currentPosition))
                    {
                        riverPositions.Add(currentPosition);
                        RiverPoints.Add(currentPosition, neighbors.ToList());
                    }
                    else
                    {
                        // this shouldnt hit....normally
                        // if it does, some thing went wrong 
                        // simply here as a fail safe
                        break;
                    }
                }

                currentPosition = nextPosition;
                prevNeighbor = nextNeighbor;
            }

            // the last position of the river will always be a lake ocean or sea
            // if the loop breaks out and gets to this point,
            // that means the river cannot flow past the current position

            RiverMouth.Item1 = currentPosition;
            RiverMouth.Item2 = LandScapeTile.GetOppositeNeighbor(prevNeighbor);
        }

        private Vector3Int GetLowestNeighbor(Vector3Int currentPosition, ref float currentElevation, ref int neighbor)
        {
            float lowestElevation = currentElevation;

            Vector3Int lowestPosition = currentPosition;

            LandScapeTile currentTile = mainPlanet.PlanetMap.GetTile(currentPosition) as LandScapeTile;

            Vector3Int[] neighbors = currentTile.GetNeighborPositions(currentPosition, mainPlanet.MapSize);

            int i = 1;

            foreach (Vector3Int pos in neighbors)
            {
                float temp = mainPlanet.NoiseManager.GetMountainLevelNoiseValue(pos.x, pos.y);

                if(temp < lowestElevation)
                {
                    lowestElevation = temp;
                    lowestPosition = pos;
                    neighbor = i;
                }
                i++;
            }

            currentElevation = lowestElevation;
            return lowestPosition;
        }

        public static List<Vector3Int>
            GetSourceLocations(List<(Vector3Int, float)> possibleSources, float planetFractal)
        {
            // the next highestPoint must be some distance away
            // this is because Rivers are usually spread out

            planetFractal *= 2; // edit this number to adjust river distance

            possibleSources = possibleSources.OrderByDescending(x => x.Item2).ToList();

            List<Vector3Int> sources = new List<Vector3Int>();
            Vector3Int tempVector = Vector3Int.zero;
            Vector3Int prevPos = Vector3Int.zero;
            float distance;

            for (int i = 0; i < possibleSources.Count / 2; i++)
            {
                tempVector = possibleSources[i].Item1; ;

                distance = Vector3Int.Distance(possibleSources[i].Item1, prevPos);

                if (IsOutsideRange(tempVector, sources, planetFractal))
                {
                    if(!sources.Contains(possibleSources[i].Item1))
                    {
                        sources.Add((possibleSources[i].Item1));
                    }                
                }     
                
                prevPos = tempVector;
            }

            return sources;
        }


        public Vector3Int GetLakePosition
        {
            get
            {
                return RiverMouth.Item1;
            }
        }
        private static bool IsOutsideRange(Vector3Int tempVector, List<Vector3Int> currentPositions, float planetFractal)
        {
            float distance = 0;

            for (int i = 0; i < currentPositions.Count; i++)
            {
                distance = Vector3Int.Distance(currentPositions[i], tempVector);

                if (! (distance > planetFractal))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
