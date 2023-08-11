using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadKart
{
    public class TestMap : MonoBehaviour
    {
        [SerializeField] private GameObject _mapRoot;

        public void Start()
        {
            //string mapJson = File.ReadAllText(Application.persistentDataPath + "/myFirstMap.json");
            string mapJson = Resources.Load<TextAsset>("Maps/myFirstMap").text;
            Map map = JsonConvert.DeserializeObject<Map>(mapJson);
            map.Instantiate();

            foreach(var obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (obj.GetComponent<TileController>() != null)
                {
                    obj.transform.SetParent(_mapRoot.transform, true);
                }
            }

            _mapRoot.transform.localScale *= 16f;
        }
    }
}