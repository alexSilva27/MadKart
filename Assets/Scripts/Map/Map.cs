using System.Collections.Generic;

namespace MadKart
{
    public class Map
    {
        public Map(List<Tile> tiles)
        {
            Tiles = tiles;
        }

        public List<Tile> Tiles { get; }

        public void Instantiate()
        {
            Tiles.ForEach(x => x.Instantiate());
        }
    }
}