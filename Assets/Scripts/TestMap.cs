using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace MadKart
{
    public class TestMap : MonoBehaviour
    {
        public void Start()
        {
            string mapJson = File.ReadAllText(Application.persistentDataPath + "/test.txt");
            Map map = JsonConvert.DeserializeObject<Map>(mapJson);
            map.Instantiate();
        }
    }
}