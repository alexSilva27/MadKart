using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace MadKart
{
    public class TestMap : MonoBehaviour
    {
        [SerializeField] private Transform _rootTransform;

        public void Start()
        {
            //string mapJson = File.ReadAllText(Application.persistentDataPath + "/testMap");
            string mapJson = Resources.Load<TextAsset>("Maps/testMap").text;
            Map map = JsonConvert.DeserializeObject<Map>(mapJson);
            map.Instantiate(_rootTransform);
        }
    }
}