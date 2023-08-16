using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace MadKart
{
    public class Tile
    {
        public Tile(TileType tileType, (int X, int Y, int Z) position, TileRotation rotation)
        {
            TileType = tileType;
            Position = position;
            Rotation = rotation;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TileType TileType { get; }

        public (int X, int Y, int Z) Position { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TileRotation Rotation { get; } // left hand rule with the thumb representing the UP axis.

        [JsonIgnore] public TileController Controller { get; private set; }

        public void Instantiate(Transform rootTransform)
        {
            TileMetadata tileMetadata = MapMetadata.Metadata.GetTileMetadata(TileType);
            
            Controller = GameObject.Instantiate(tileMetadata.Prefab, rootTransform, false).GetComponent<TileController>();
            Controller.Tile = this;
        }
    }
}