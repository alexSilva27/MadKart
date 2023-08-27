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