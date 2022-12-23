using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts
{
    [Serializable]

    #region Purpose of this class
    /* This class will be more sprites that display a sprite according to the hex side in question
     * for example, a river may flow from side 1 to side 3, 
     * therefore we need a means to show which sides a particular set of sprites connect to
     * Or a hex can only have a coast on side 5 and 6 and we need to relate those sides to its respective sprite
     */
    #endregion
    internal class ConnectedSprites
    {
        private List<TileContainer> tiles = new List<TileContainer>();

        Dictionary<Neighbor, TileContainer> tilesDictionary = new Dictionary<Neighbor, TileContainer>();

        HexagonalRuleTile connectedSprites;

        //[SerializeField] private ConnectedSide[] connectedSides = new ConnectedSide[6];

        public void Instantiate(float hexScale, LandScapeTile tileAsset, HexagonalRuleTile hexRuleSprites)
        {
            // we do this because the default neighbor vectors are wrong
            // plus the game code(unity) tells us to preset these values to our own values
            // I am guessing their default values work with some other shape, but not hexes

            // hexRuleSprites.m_TilingRules.ForEach(x => x.m_NeighborPositions = LandScapeTile.NeighborPositions);

            connectedSprites = hexRuleSprites;

            //ValidateTilingNeighbors();

            TileContainer temp;
            Neighbor tempNeighbor;

            List<int> neighbors = Enumerable.Repeat(2, 6).ToList();

            /// DO NOT EDIT THE M_NEIGHBOR RULES VARIABLE OF TILING RULES
            /// It changes the values in editor and unchecks some of your values
            /// so if check a side as "this" it might uncheck it
            /// 

            foreach (TilingRule rules in hexRuleSprites.m_TilingRules)
            {
                if (rules.m_Sprites[0] != null)
                {
                    neighbors = Enumerable.Repeat(2, 6).ToList();

                    // this will only return values marked as "this" or "not this"

                    Dictionary<Vector3Int, int> neighborValues = rules.GetNeighbors();

                    foreach (Vector3Int pos in neighborValues.Keys)
                    {
                        int index = NeighborPositions.IndexOf(pos);
                        int num = neighborValues[pos];

                        if (num == 1)
                        {
                            neighbors[index] = 1;
                        }
                    }

                    // we do this so if 2 sprites have thesame neighbors,
                    // we can just combine into one tile
                    // this allow for more diversity in placing tiles since
                    // if all the sprites are under one tile, we can use the getrandomtile method

                    tempNeighbor = new Neighbor(neighbors);


                    // we cant compare using the dictionary.contains key because 
                    // the equality comparer for dictionary will compare our list by reference
                    // not value
                    // Another way around this is to simply define a struct for the list<int>
                    // then we can override the equals and hashcode methods

                    // Additionally, be advised that changing the neighbors
                    // will also change and STORE the neighbors after game ends
                    // so be really carefull

                    if (tilesDictionary.ContainsKey(tempNeighbor))
                    {
                        tilesDictionary[tempNeighbor].AddSprites(rules.m_Sprites.ToList());
                    }
                    else
                    {
                        temp = new TileContainer(hexScale, tileAsset, rules.m_Sprites.ToList());

                        tilesDictionary.Add(tempNeighbor, temp);
                    }
                }
            }
        }

        // these are references to the tile rulings in editor
        // The positions of those little boxes surronding the sprite
        // THEY WILL NOT WORK TO GET THE NEIGHBOR TILES OF A HEX!!!
        // i had no manually insert these values,
        // i have no bearing whatsoever as to why they are in this order

        private static List<Vector3Int> NeighborPositions = new List<Vector3Int>()
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),

            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
        };

        public LandScapeTile GetRandomTile(List<int> neighbors)
        {
            TileContainer tiles;

            Neighbor temp = new Neighbor(neighbors);

            if (tilesDictionary.TryGetValue(temp, out tiles))
            {
                return tiles.GetRandomTile();
            };

            // This means we could not find a sprite that has those tile rules
            Debug.Log("Sprite Not Found for Sides" +
                ": " + Neighbor.GetString(neighbors));
            return null;
        }

        public LandScapeTile GetRandomTile(int neighborSide)
        {
            TileContainer tiles;

            Neighbor temp = new Neighbor(neighborSide);

            if (tilesDictionary.TryGetValue(temp, out tiles))
            {
                return tiles.GetRandomTile();
            };

            Debug.Log("Sprite Not Found for Side: " + neighborSide);

            // This means we could not find a sprite that has those tile rules
            return null;
        }


        public LandScapeTile GetTest()
        {
            foreach (TileContainer tile in tilesDictionary.Values)
            {
                return tile.GetRandomTile();
            }

            return null;
        }
    }

    public struct Neighbor : IEquatable<Neighbor>
    {
        private bool _isConnected;

        public bool IsConnected { get { return _isConnected;  } }
        private List<bool> NeighborValues { get; }
        public List<int> NeighborNumbers { get; }

        public Neighbor(List<int> neighbors)
        {
            _isConnected = false;

            NeighborNumbers = neighbors;
            NeighborValues = new List<bool>();

            NeighborValues = Convert2Bool(neighbors);
        }

        public Neighbor(int neighborSide)
        {
            _isConnected = false;

            NeighborNumbers = new List<int>();

            NeighborValues = new List<bool>();

            NeighborNumbers = CreateNeighborList(neighborSide);

            NeighborValues = Convert2Bool(NeighborNumbers);
        }

        private List<bool> Convert2Bool(List<int> aList)
        {
            List<bool> result = new List<bool>();

            foreach (int num in aList)
            {
                if(num == 1)
                {
                    result.Add(true);
                    _isConnected = true;
                }
                else
                {
                    result.Add(false);
                }
            }

            return result;
        }

        /// <summary>
        /// If you have a list of sides already, then u can pass in that list
        /// for example, if you have neighbors at position 2, 5
        /// This will create a neigbor struct with a value of 1 at position 2 and 5
        /// </summary>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        public static Neighbor ConvertSidesToNeighbor(List<int> neighbors)
        {
            List<int> result = Enumerable.Repeat(2, 6).ToList();

            foreach (int aSide in neighbors)
            {
                // since a hex only has 6 sides
                if(aSide > 0 && aSide <= 6)
                {
                    result[aSide - 1] = 1;
                }           
            }

            return new Neighbor(result);
        }
        private List<int> CreateNeighborList(int neighborSide)
        {
            List<int> result = Enumerable.Repeat(2, 6).ToList();

            try
            {
                result[neighborSide - 1] = 1;
            }
            catch (Exception)
            {

                throw;
            }
          
            _isConnected = true;

            return result;
        }

        public void InsertNeighbor(int index, bool hasNeighbor)
        {
            NeighborValues[index] = hasNeighbor;

            if (hasNeighbor == true)
            {
                NeighborNumbers[index] = 1;
                _isConnected = true;
            }
            else
            {
                NeighborNumbers[index] = 2;
            }         
        }
        /// <summary>
        /// Returns a new Neighbor that is combined with the given neighbor
        /// </summary>
        /// <param name="newNeighbor"></param>
        /// <returns></returns>
        public Neighbor CombineNeighbor(Neighbor newNeighbor)
        {
            List<int> newNum = newNeighbor.NeighborNumbers;
            List<int> combined = new List<int>();

            for (int i = 0; i < NeighborNumbers.Count; i++)
            {
                if (NeighborNumbers[i] == 1)
                {
                    combined.Add(1);
                }
                else
                {
                    if (newNum[i] == 1)
                    {
                        combined.Add(1);
                    }
                    else
                    {
                        combined.Add(2);
                    }
                }
            }

            return new Neighbor(combined);
        }
        public override int GetHashCode()
        {
            // a proper hashcode is needed since we will be using dictionary 
            // for lookup and equality comparison

            int hashcode = 887; // random prime number
            int randomPrime = 1459;

            for (int i = 0; i < NeighborNumbers.Count; i++)
            {
                hashcode = hashcode * randomPrime + NeighborNumbers[i].GetHashCode();
            }

            return hashcode;
        }
        public override bool Equals(object obj)
        {
            if(obj is Neighbor)
            {
                return Equals(obj);
            }
            else
            {
                return false;
            }      
        }
        public bool Equals(Neighbor other)
        {
            if (NeighborValues.SequenceEqual(other.NeighborValues))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(Neighbor a, Neighbor b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Neighbor a, Neighbor b)
        {
            return a.Equals(b);
        }

        public override string ToString()
        {
            string str = "";

            foreach (int  num in NeighborNumbers)
            {
                str += num + " ";
            }

            return str;
        }

        public static string GetString(List<int> neighbors)
        {
            string str = "";

            foreach (int num in neighbors)
            {
                str += num + " ";
            }

            return str;
        }

        public static int Normalize(int neighbor)
        {
            if (neighbor > 6)
            {
                return 1;
            }
            if (neighbor <= 0)
            {
                return 1;
            }

            // the neighbor is accurate
            return neighbor;

        }

    }
}