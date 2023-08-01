using System;
using UnityEngine;

namespace MadKart
{
    [Serializable]
    public struct TileMetadata
    {
        public GameObject Prefab;
        public TileType TileType;

        //public ConnectionType NeutralConnection;
        //public ConnectionType NinetyDegreesConnection;
        //public ConnectionType OneHundredEightyDegreesConnection;
        //public ConnectionType MinusNinetyDegreesConnection;

        //public ConnectionType NeutralZPlus1Connection;
        //public ConnectionType NinetyDegreesZPlus1Connection;
        //public ConnectionType OneHundredEightyDegreesZPlus1Connection;
        //public ConnectionType MinusNinetyDegreesZPlus1Connection;
    }
}