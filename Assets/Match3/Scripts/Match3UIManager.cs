using UnityEngine;
using TMPro;
using DB_Core;
using System.Collections;
using DG.Tweening;

namespace DB_Match3
{
    public class Match3UIManager : DBMonoBehaviour
    {
        [SerializeField] RoundManager roundManager;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text movesLeftText;
        [SerializeField] TMP_Text scoreGoalText;
        [SerializeField] GameObject gameOverScreen;
        [SerializeField] TMP_Text scoreToastText;

        private void OnEnable()
        {
            ScoreTextIncrease(roundManager.Match3Score);
            MovesTextUpdate(roundManager.MovesCount);
            scoreGoalText.text = $"{roundManager.Match3ScoreGoal}";
            AddListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            AddListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            AddListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            AddListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            RemoveListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            RemoveListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            RemoveListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
        }

        private void ScoreTextIncrease(object score)
        {
            int currentScore = (int)score;
            scoreText.text = $"{currentScore}";
        }

        private void MovesTextUpdate(object movesCount)
        {
            int currentMovesCount = (int)movesCount;
            movesLeftText.text = $"{currentMovesCount}";
        }

        private void GameOverScreenShow(object boolValue)
        {
            bool value = (bool)boolValue;
            gameOverScreen.SetActive(value);
        }

        private void ScoreToastTextIncrease(object score)
        {
            int scoreGain = (int)score;
            scoreToastText.text = $"+{scoreGain}";

            scoreToastText.gameObject.SetActive(true);
            StartCoroutine(ToastDestruction());
        }

        private IEnumerator ToastDestruction()
        {
            yield return new WaitForSeconds(1);
            scoreToastText.gameObject.SetActive(false);
        }

    }
}