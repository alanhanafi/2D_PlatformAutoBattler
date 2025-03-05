using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class PlatformerManager : MonoBehaviour
    {
        [SerializeField] private float gameTimer = 30;
        
        [SerializeField] private TextMeshProUGUI timerText;
        private float timer;
        
        private bool isGameRunning = false;
        
        #region Singleton

        internal static PlatformerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        #endregion

        private void Start()
        {
            isGameRunning = true;
            UpdateTimer(gameTimer);
        }

        private void Update()
        {
            if (!isGameRunning)
                return;
            UpdateTimer(timer- Time.deltaTime);
            if (timer <=0)
                EndSpeedrun();
        }

        private void UpdateTimer(float newTime)
        {
            timer = newTime;
            timerText.text = timer.ToString("0.00");
        }

        private void EndSpeedrun()
        {
            isGameRunning = false;
            SceneManager.LoadScene("AutoBattle");
        }
    }
}