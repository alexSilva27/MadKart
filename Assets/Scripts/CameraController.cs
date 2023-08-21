using System;
using UnityEngine;

namespace MadKart
{
    public class CameraController : MonoBehaviour
    {
        [Serializable]
        private struct SerializedData
        {
            public Camera Camera;
            public KartController Kart;

            public float X;
            public float Y;
            public float Offset;
        }

        [SerializeField] private SerializedData _serializedData;

        private void LateUpdate()
        {
            Vector3 kartPosition = _serializedData.Kart.VisualTopTransform.position;
            Transform camera = _serializedData.Camera.transform;
            Vector3 kartToCameraXZNormalized = Vector3.ProjectOnPlane(camera.position - kartPosition, Vector3.up).normalized;

            camera.position = kartPosition + kartToCameraXZNormalized * _serializedData.X + Vector3.up * _serializedData.Y;
            camera.LookAt(kartPosition + Vector3.up * _serializedData.Offset, Vector3.up);
        }
    }
}