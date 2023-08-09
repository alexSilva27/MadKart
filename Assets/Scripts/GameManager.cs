using UnityEngine;

namespace MadKart
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}