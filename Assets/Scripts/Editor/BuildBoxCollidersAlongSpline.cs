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

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace MadKart
{
    public class BuildBoxCollidersAlongSpline : EditorWindow
    {
        [MenuItem("Window/BuildBoxCollidersAlongSpline")]
        public static void ShowExample()
        {
            BuildBoxCollidersAlongSpline wnd = GetWindow<BuildBoxCollidersAlongSpline>();
            wnd.titleContent = new GUIContent("BuildBoxCollidersAlongSpline");
        }

        private ObjectField _splineField;
        private ObjectField _physicsMaterialField;
        private IntegerField _boxCollidersCountField;
        private Vector2Field _boxCollidersSizeField;

        private void GenerateColliders()
        {
            int boxCollidersCount = _boxCollidersCountField.value;
            Spline spline = (_splineField.value as SplineContainer).Spline;
            PhysicMaterial physicMaterial = _physicsMaterialField.value as PhysicMaterial;
            Vector2 colliderYZLocalScale = _boxCollidersSizeField.value;

            GameObject topGO = new GameObject();
            topGO.transform.position = Vector3.zero;
            topGO.transform.rotation = Quaternion.identity;

            for (int i = 0; i < boxCollidersCount; i++)
            {
                float currentPointT = i / (float) boxCollidersCount;
                Vector3 currentPoint = SplineUtility.EvaluatePosition(spline, currentPointT);

                float nextPointT = (i+1) / (float) boxCollidersCount;
                Vector3 nextPoint = SplineUtility.EvaluatePosition(spline, nextPointT);

                Vector3 direction = nextPoint - currentPoint;

                GameObject colliderGO = new GameObject();
                colliderGO.transform.SetParent(topGO.transform);
                colliderGO.transform.position = currentPoint;
                colliderGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, direction.normalized).normalized);
                colliderGO.transform.localScale = new Vector3(direction.magnitude, colliderYZLocalScale.x, colliderYZLocalScale.y);

                BoxCollider collider = colliderGO.AddComponent<BoxCollider>();
                collider.material = physicMaterial;
                collider.center = new Vector3(0.5f, -0.5f, 0f);
                collider.size = new Vector3(1f, 1f, 1f);
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object.
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            Label label = new Label("Drag here the spline that will be used to generate the box colliders:");
            root.Add(label);

            // Create spline field.
            _splineField = new ObjectField();
            _splineField.objectType = typeof(SplineContainer);
            root.Add(_splineField);

            label = new Label("Drag here the physics material that will be assigned to the generated box colliders:");
            root.Add(label);

            // Create physics material field.
            _physicsMaterialField = new ObjectField();
            _physicsMaterialField.objectType = typeof(PhysicMaterial);
            root.Add(_physicsMaterialField);

            label = new Label("The amount of box colliders to be generated:");
            root.Add(label);

            // Create box colliders count field.
            _boxCollidersCountField = new IntegerField();
            root.Add(_boxCollidersCountField);

            label = new Label("The size in local YZ of the colliders to be generated:");
            root.Add(label);

            // Create box colliders size field.
            _boxCollidersSizeField = new Vector2Field();
            root.Add(_boxCollidersSizeField);

            // Create button.
            Button button = new Button();
            button.text = "Generate colliders";
            button.clicked += GenerateColliders;
            root.Add(button);
        }
    }
}