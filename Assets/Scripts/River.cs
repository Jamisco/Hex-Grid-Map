using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GridManager;
using static LandScapes;
using static UnityEditor.PlayerSettings;

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
        private static Tilemap planetMap;

        public static float InitialSpeed;
        public static float PlanetGravityVelocity;
        public static float RiverFlowChangeFriction;
        public static float RiverSpread;

        public Dictionary<Vector3Int, RiverData> RiverPoints = new Dictionary<Vector3Int, RiverData>();

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
            int currentNeighbor = -1;

            float prevElevation = -1;
            float currentElevation = mainPlanet
               .NoiseManager.GetMountainLevelNoiseValue(riverSource.x, riverSource.y);

            float nextElevation = -1;

            Vector3Int currentPosition = riverSource;

            Vector3Int nextPosition;
            LandScapeTile curTile;
            LandScapeTile nextTile;

            List<int> neighbors = new List<int>();

            // if the river starts at a higher elevation, the initial speed would match that 
            float streamSpeed = InitialSpeed;
            float streamVolume = mainPlanet
               .NoiseManager.GetPrecipNoiseValue(riverSource.x, riverSource.y); ;

            int opposite;
            float tempVol;

            while (true)
            {
                neighbors = new List<int>();

                nextPosition = GetDownhillNeighbor(currentPosition, currentElevation, ref nextElevation,
                    ref nextNeighbor);

                // we must verify that the river can keep flowing into the next position.
                // if not, then we know the river will stop at the current location and is therefore a lake

                nextTile = planetMap.GetTile(nextPosition) as LandScapeTile;

                if (currentPosition == nextPosition)
                {
                    Vector3Int tempPos = nextPosition;

                    if(currentNeighbor != -1)
                    {
                        nextPosition = GetOppositeNeighbor(currentPosition, currentNeighbor, ref nextNeighbor);
                    }
                    else
                    {
                        nextPosition = GetLowestNeighbor(currentPosition, currentElevation, ref nextElevation,
                                                  ref nextNeighbor);
                    }
                
                    // The River cannot no longer go foward because all paths are too high
                    if (tempPos == nextPosition)
                    {
                        //Debug.Log("No New Position " + currentPosition.ToString());
                        break;
                    }

                    nextTile = planetMap.GetTile(nextPosition) as LandScapeTile;
                }

                curTile = planetMap.GetTile(currentPosition) as LandScapeTile;

                if (curTile.LandScapeIsSeaorOcean())
                {
                    break;
                }

                bool inMontains = false;

                if (nextTile.TileLandScape >= LandScape.Hills && curTile.TileLandScape >= LandScape.Hills)
                {
                    inMontains = true;
                }

                neighbors.Add(nextNeighbor);

                if (currentNeighbor != -1)
                {
                    opposite = LandScapeTile.GetOppositeNeighbor(currentNeighbor);
                    neighbors.Add(opposite);

                    streamSpeed = CalculateSpeed(streamSpeed, prevElevation - currentElevation,
                         GetDirectionFriction(opposite, nextNeighbor), inMontains);
                }
                else
                {
                    // this will run if we are just starting the loop
                    // since the river is just starting there is no directon change friction
                    streamSpeed = CalculateSpeed(streamSpeed, 0, 0, inMontains);
                      
                }

                tempVol = mainPlanet.NoiseManager.GetPrecipNoiseValue(currentPosition.x, currentPosition.y);

                streamVolume += (1 / (1 - tempVol) * (tempVol / 2));

                if (streamSpeed <= 0)
                {
                    // the river has ran out of speed and can no longer go forward
                   // Debug.Log("Ran out of Speed" + currentPosition.ToString() + "Speed: " + streamSpeed);
                    break;
                }

                if (!RiverPoints.ContainsKey(currentPosition))
                {
                    RiverPoints.Add(currentPosition, new RiverData(RiverBody.Stream, Neighbor.ConvertSidesToNeighbor(neighbors),
                        streamSpeed, streamVolume));
                }
                else
                {
                    Debug.Log("Error");
                }

                prevElevation = currentElevation;
                currentElevation = nextElevation;
                currentNeighbor = nextNeighbor;
                currentPosition = nextPosition;
            }

            // the last position of the river will always be a lake ocean or sea
            // if the loop breaks out and gets to this point,
            // that means the river cannot flow past the current position

            RiverMouth.Item1 = currentPosition;
            RiverMouth.Item2 = LandScapeTile.GetOppositeNeighbor(currentNeighbor);
        }

        private Vector3Int GetDownhillNeighbor(Vector3Int currentPosition, float currentElevation,
            ref float nextElevation, ref int nextNeighbor)
        {
            float lowestElevation = currentElevation;

            Vector3Int lowestPosition = currentPosition;

            LandScapeTile currentTile = mainPlanet.PlanetMap.GetTile(currentPosition) as LandScapeTile;

            Vector3Int[] neighbors = currentTile.GetNeighborPositions(currentPosition, mainPlanet.MapSize);

            int i = 1;

            foreach (Vector3Int pos in neighbors)
            {
                float temp = mainPlanet.NoiseManager.GetMountainLevelNoiseValue(pos.x, pos.y);

                if (temp < lowestElevation && !RiverPoints.ContainsKey(pos))
                {
                    lowestElevation = temp;
                    lowestPosition = pos;
                    nextNeighbor = i;
                }
                i++;
            }

            nextElevation = lowestElevation;
            return lowestPosition;
        }


        private Vector3Int GetLowestNeighbor(Vector3Int currentPosition, float currentElevation,
                         ref float nextElevation, ref int nextNeighbor)
        {
            float lowestElevation = float.MaxValue;

            Vector3Int lowestPosition = Vector3Int.zero;

            LandScapeTile currentTile = mainPlanet.PlanetMap.GetTile(currentPosition) as LandScapeTile;

            Vector3Int[] neighbors = currentTile.GetNeighborPositions(currentPosition, mainPlanet.MapSize);

            int i = 1;

            foreach (Vector3Int pos in neighbors)
            {
                float temp = mainPlanet.NoiseManager.GetMountainLevelNoiseValue(pos.x, pos.y);

                if (temp < lowestElevation && !RiverPoints.ContainsKey(pos))
                {
                    lowestElevation = temp;
                    lowestPosition = pos;
                    nextNeighbor = i;
                }

                i++;
            }

            nextElevation = lowestElevation;
            return lowestPosition;
        }

        private Vector3Int GetOppositeNeighbor(Vector3Int currentPosition, int currentNeighbor, ref int nextNeighbor)
        {
           // int opposite = LandScapeTile.GetOppositeNeighbor(currentNeighbor);

            Vector3Int lowestPosition = currentPosition;

            LandScapeTile currentTile = mainPlanet.PlanetMap.GetTile(currentPosition) as LandScapeTile;

            Vector3Int[] neighbors = currentTile.GetNeighborPositions(currentPosition, mainPlanet.MapSize);

            nextNeighbor = currentNeighbor;

            lowestPosition = neighbors[currentNeighbor - 1];

            if(!RiverPoints.ContainsKey(lowestPosition))
            {
                return neighbors[currentNeighbor - 1];
            }
            else
            {
                return currentPosition;
            }        
        }


        /// <summary>
        /// Do not add 1 to the directionalDiff, this method will account for that
        /// </summary>
        /// <param name="currentSpeed"></param>
        /// <param name="elevationDiff"></param>
        /// <param name="directionalDiff"></param>
        /// <returns></returns>
        private float CalculateSpeed(float currentSpeed, float elevationDiff, float friction, bool inMountains = false)
        {
            // if the elevation difference is negative, then the speed will simply be subtracted/reduced

            if(inMountains)
            {
                return (currentSpeed + elevationDiff * PlanetGravityVelocity) - friction / 2;
            }
            else
            {
                return (currentSpeed + elevationDiff * PlanetGravityVelocity) - friction;
            }

        }

        private static bool FlowingInDirection(int currentNeighbor, int prevNeighbor)
        {
            //  int opposite = LandScapeTile.GetOppositeNeighbor(prevNeighbor);

            int left, right;

            left = prevNeighbor - 1;

            left = Neighbor.Normalize(left);

            right = prevNeighbor + 1;

            right = Neighbor.Normalize(right);

            if (currentNeighbor == prevNeighbor)
            {
                return true;
            }

            if (currentNeighbor == left)
            {
                return true;
            }

            if (currentNeighbor == right)
            {
                return true;
            }

            return false;
        }

        public float GetDirectionFriction(int neighborFrom, int neighborTo)
        {
            int opposite = LandScapeTile.GetOppositeNeighbor(neighborFrom);

            if (neighborFrom <= 0 || neighborTo <= 0)
            {
                return GetFriction(0);
            }

            if (neighborFrom == neighborTo)
            {
                Debug.Log("Error at GetDirectionDifference in River, your from and to are thesame");

                return GetFriction(0);
            }

            int distanceFromOpposite = GetShortestPath(opposite, neighborTo);
            
            return GetFriction(distanceFromOpposite);
        }

        private static int GetShortestPath(int neighborFrom, int neighborTo)
        {
            // this code gets the distance from 2 sides

            int left = 0;
            int right = 0;

            int max = 6;

            if (neighborFrom <= neighborTo)
            {
                left = neighborFrom + max - neighborTo;
                right = Math.Abs(neighborFrom - neighborTo);
            }
            else if (neighborTo < neighborFrom)
            {
                left = neighborTo + max - neighborFrom;
                right = Math.Abs(neighborFrom - neighborTo);
            }

            if (left < right)
            {
                return left;
            }
            else
            {
                return right;
            }
        }

        public static float[] Frictions;
        private float GetFriction(int flowChange)
        {
            if(flowChange == 0)
            {
                return Frictions[0] * RiverFlowChangeFriction;
            }
            if (flowChange == 1)
            {
                return Frictions[1] * RiverFlowChangeFriction;
            }
            else
            {
                return Frictions[2] * RiverFlowChangeFriction ;
            }
        }

        public static List<Vector3Int> GetSourceLocations(List<(Vector3Int, float)> possibleSources, float heightLimit)
        {
            // the next highestPoint must be some distance away
            // this is because Rivers are usually spread out

            float spread = mainPlanet.Fractal * RiverSpread; // edit this number to adjust river distance, 
            // bigger is less source, less rivers
            // smaller = more sources, more rivers

            possibleSources = possibleSources.OrderByDescending(x => x.Item2).ToList();

            List<Vector3Int> sources = new List<Vector3Int>();
            Vector3Int tempVector = Vector3Int.zero;
            Vector3Int prevPos = Vector3Int.zero;

            //item 1 = position
            //item 2 = scale
            for (int i = 0; i < possibleSources.Count; i++)
            {
                if (possibleSources[i].Item2 < heightLimit)
                {
                    continue;
                }

                tempVector = possibleSources[i].Item1; ;

                if (IsOutsideRange(tempVector, sources, spread))
                {
                    if (!sources.Contains(possibleSources[i].Item1))
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

                if (!(distance > planetFractal))
                {
                    return false;
                }
            }

            return true;
        }


    }

    public readonly struct RiverData
    {
        public RiverBody RiverBody { get; }
        public Neighbor Neighbor { get; }
        public float Speed { get; }

        public float Volume { get; }
        public float Current { get; }
        public RiverData(RiverBody body, Neighbor neighbors, float speed, float volume)
        {
            RiverBody = body;
            Neighbor = neighbors;

            Speed = speed;

            Volume = volume;

            // figure out a proper formula for current, since we are dealing with decimals we cant jsut multiple
            // for example .9 * .9 = 81
            Current = 2;


        }

    }

    public enum RiverBody { Stream, Lake }

    // the initial direction of the river must stay thesame.

    // SO IF Tthe river, flows in direction 1,2,3
    // through out its course it must keep that vector
}
