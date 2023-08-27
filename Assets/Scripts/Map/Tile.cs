//MIT License

//Copyright (c) 2023 - Alexandre Silva

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

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