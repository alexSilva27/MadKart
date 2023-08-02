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
                    int positionX = (int) Math.Round(transform.position.x);
                    int positionY = (int) Math.Round(transform.position.y * 2f);
                    int positionZ = (int) Math.Round(transform.position.z);
                    (int X, int Y, int Z) position = (positionX, positionY, positionZ);

                    TileRotation rotation;
                    float angle = Vector3.SignedAngle(Vector3.right, transform.right, Vector3.up);

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

                transform.position = new Vector3(_tile.Position.X, _tile.Position.Y / 2f, _tile.Position.Z);
                transform.rotation = Quaternion.identity;

                float angle = _tile.Rotation switch
                {
                    TileRotation.MinusNinetyDegrees => -90f,
                    TileRotation.NinetyDegrees => 90f,
                    TileRotation.OneHundredEightyDegrees => 180f,
                    _ => 0f,
                };
                Vector3 rotationPivot = transform.position + new Vector3(0.5f, 0f, 0.5f);
                transform.RotateAround(rotationPivot, Vector3.up, angle);
            }
        }
    }
}