using TMPro;
using UnityEngine;

namespace MadKart
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private float _refreshDeltaTime;

        private int _frameCountFromLastRefresh;
        private float _lastRefreshTime;

        // Update is called once per frame
        private void Update()
        {
            _frameCountFromLastRefresh++;

            if (Time.time > _lastRefreshTime + _refreshDeltaTime)
            {
                int fps = (int)(_frameCountFromLastRefresh / _refreshDeltaTime);

                _textMeshProUGUI.text = "FPS: " + fps.ToString();

                _lastRefreshTime = Time.time;
                _frameCountFromLastRefresh = 0;
            }
        }
    }
}