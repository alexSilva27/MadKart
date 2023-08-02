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

        [JsonIgnore] public GameObject Instance { get; private set; }

        public void Instantiate()
        {
            TileMetadata tileMetadata = MapMetadata.Metadata.GetTileMetadata(TileType);
            Vector3 position = new Vector3(Position.X, Position.Y / 2f, Position.Z);
            Instance = GameObject.Instantiate(tileMetadata.Prefab, position, Quaternion.identity);

            float angle = Rotation switch
            {
                TileRotation.MinusNinetyDegrees => -90f,
                TileRotation.NinetyDegrees => 90f,
                TileRotation.OneHundredEightyDegrees => 180f,
                _ => 0f,
            };
            Vector3 rotationPivot = position + new Vector3(0.5f, 0f, 0.5f);
            Instance.transform.RotateAround(rotationPivot, Vector3.up, angle);
        }
    }
}