using System;
using UnityEngine;

namespace MadKart
{
    [CreateAssetMenu(menuName = "Scriptable Objects / MapMetadata")]
    public class MapMetadata : ScriptableObject
    {
        [Serializable]
        private struct Data
        {
            public TileMetadata[] TilesMetadata;
        }

        [SerializeField] private Data _data;

        public TileMetadata GetTileMetadata(TileType tileType) => Array.Find(_data.TilesMetadata, x => x.TileType == tileType);

        private static MapMetadata _metadata;
        public static MapMetadata Metadata => _metadata ??= Resources.Load<MapMetadata>(nameof(MapMetadata));
    }
}