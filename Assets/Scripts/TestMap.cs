using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace MadKart
{
    public class TestMap : MonoBehaviour
    {
        public void Start()
        {
            Application.targetFrameRate = 60;

            //string mapJson = File.ReadAllText(Application.persistentDataPath + "/test.json");
            //Map map = JsonConvert.DeserializeObject<Map>(mapJson);
            //map.Instantiate();
        }
    }
}