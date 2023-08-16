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