using System.Collections;
using Gisha.Effects.Audio;
using Gisha.scorejam18.Gameplay;
using Gisha.scorejam18.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gisha.scorejam18.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; set; }

        private LeaderboardController _leaderboardController;
        private GameTimer _gameTimer;
        private Collector[] _collectors;

        private void Awake()
        {
            Instance = this;
            _leaderboardController = FindObjectOfType<LeaderboardController>();
            _collectors = FindObjectsOfType<Collector>();
            _gameTimer = GetComponent<GameTimer>();
        }

        private void Start()
        {
            int randomMusic = Random.Range(1, 3);
            AudioManager.Instance.PlayMusic("GameMusic" + randomMusic);
            
            _gameTimer.StartTimer();
        }

        private void OnEnable()
        {
            Collector.CollectableAcquired += OnCollectableAcquired;
            _gameTimer.TimeOut += Lose;
        }

        private void OnDisable()
        {
            Collector.CollectableAcquired -= OnCollectableAcquired;
            _gameTimer.TimeOut -= Lose;
        }

        public static void Win()
        {
            Instance._gameTimer.StopTimer();
            UIManager.Instance.ShowWinPopup();
            
            AudioManager.Instance.PlaySFX("Victory");
        }

        public static void Lose()
        {
            Instance.StartCoroutine(Instance.LoseRoutine());
            
            AudioManager.Instance.PlaySFX("Lose");
        }

        private void OnCollectableAcquired()
        {
            foreach (var collector in _collectors)
            {
                if (!collector.IsReady)
                    return;
            }
            
            Win();
        }

        public void LoadRandomLevel()
        {
            int level = Random.Range(1, 4);
            SceneManager.LoadScene("Level" + level);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }


        private IEnumerator LoseRoutine()
        {
            Instance._gameTimer.StopTimer();
            UIManager.Instance.ShowLosePopup();

            yield return _leaderboardController.SubmitScoreRoutine(PlayerManager.CurrentScore);
            PlayerManager.CurrentScore = 0;
        }
    }
}