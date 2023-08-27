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
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace MadKart
{
    public class BuildMapWindow : EditorWindow
    {
        [MenuItem("Window/BuildMapWindow")]
        public static void ShowExample()
        {
            BuildMapWindow wnd = GetWindow<BuildMapWindow>();
            wnd.titleContent = new GUIContent("BuildMapWindow");
        }

        private TextField _fileNameTextField;

        private void GenerateMap()
        {
            List<Tile> tiles = new List<Tile>();

            var scene = EditorSceneManager.GetActiveScene();
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                TileController[] tilesController = obj.GetComponentsInChildren<TileController>(false);

                foreach (TileController tileController in tilesController)
                {
                    if (tileController.gameObject.activeInHierarchy)
                    {
                        tiles.Add(tileController.Tile);
                    }
                }
            }

            Map map = new Map(tiles);
            string json = JsonConvert.SerializeObject(map, Formatting.Indented);
            string fileName = Application.persistentDataPath + "/" + _fileNameTextField.text;
            File.WriteAllText(fileName, json);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy
            Label label = new Label("Name of the file the map will be saved to:");
            root.Add(label);

            _fileNameTextField = new TextField();
            root.Add(_fileNameTextField);

            // Create button
            Button button = new Button();
            button.text = "Save";
            button.clicked += GenerateMap;
            root.Add(button);
        }
    }
}