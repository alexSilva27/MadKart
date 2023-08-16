using System.Collections.Generic;
using UnityEngine;

namespace MadKart
{
    public class Map
    {
        public Map(List<Tile> tiles)
        {
            Tiles = tiles;
        }

        public List<Tile> Tiles { get; }

        public void Instantiate(Transform rootTransform)
        {
            Tiles.ForEach(x => x.Instantiate(rootTransform));
        }
    }
}