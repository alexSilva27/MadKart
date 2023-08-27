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

using System;
using UnityEngine;

namespace MadKart
{
    public class TileController : MonoBehaviour
    {
        [Serializable]
        private struct SerializedData
        {
            public TileType TileType;
            public MeshCollider GroundNonConvexMeshCollider;
        }

        [SerializeField]
        private SerializedData _serializedData;

        private Tile _tile;
        public Tile Tile
        {
            get
            {
                if (_tile == null)
                {
                    int positionX = (int) Math.Round(transform.localPosition.x);
                    int positionY = (int) Math.Round(transform.localPosition.y * 2f);
                    int positionZ = (int) Math.Round(transform.localPosition.z);
                    (int X, int Y, int Z) position = (positionX, positionY, positionZ);

                    TileRotation rotation;
                    float angle = Vector3.SignedAngle(transform.parent.right, transform.right, Vector3.up);

                    if (Mathf.Approximately(angle, -90f))
                    {
                        rotation = TileRotation.MinusNinetyDegrees;
                        position.X--;
                    }
                    else if (Mathf.Approximately(angle, 90f))
                    {
                        rotation = TileRotation.NinetyDegrees;
                        position.Z--;
                    }
                    else if (Mathf.Approximately(angle, 0f))
                    {
                        rotation = TileRotation.Neutral;
                    }
                    else
                    {
                        rotation = TileRotation.OneHundredEightyDegrees;
                        position.X--;
                        position.Z--;
                    }

                    _tile = new Tile(_serializedData.TileType, position, rotation);
                }

                return _tile;
            }
            set
            {
                _tile = value;

                transform.localPosition = new Vector3(_tile.Position.X, _tile.Position.Y / 2f, _tile.Position.Z);
                transform.localRotation = Quaternion.identity;

                float angle = _tile.Rotation switch
                {
                    TileRotation.MinusNinetyDegrees => -90f,
                    TileRotation.NinetyDegrees => 90f,
                    TileRotation.OneHundredEightyDegrees => 180f,
                    _ => 0f,
                };
                Vector3 rotationPivot = transform.TransformPoint(new Vector3(0.5f, 0f, 0.5f));
                transform.RotateAround(rotationPivot, transform.up, angle);

                if (_serializedData.GroundNonConvexMeshCollider != null)
                {
                    GroundNonConvexMeshCollider = new MeshColliderData(_serializedData.GroundNonConvexMeshCollider);
                }
            }
        }

        public MeshColliderData GroundNonConvexMeshCollider { get; private set; }
    }
}