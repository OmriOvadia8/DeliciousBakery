using UnityEngine;
using TMPro;
using DB_Core;

namespace DB_Match3
{
    public class Match3UIManager : DBMonoBehaviour
    {
        [SerializeField] RoundManager roundManager;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text movesLeftText;
        [SerializeField] TMP_Text scoreGoalText;
        [SerializeField] GameObject gameOverScreen;

        private void OnEnable()
        {
            ScoreTextIncrease(roundManager.Match3Score);
            MovesTextUpdate(roundManager.MovesCount);
            scoreGoalText.text = $"Reach to {roundManager.Match3ScoreGoal} Points";
            AddListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            AddListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            AddListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            RemoveListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            RemoveListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
        }

        private void ScoreTextIncrease(object score)
        {
            int currentScore = (int)score;
            scoreText.text = $"Score: {currentScore}";
        }

        private void MovesTextUpdate(object movesCount)
        {
            int currentMovesCount = (int)movesCount;
            movesLeftText.text = $"Moves: {currentMovesCount}";
        }

        private void GameOverScreenShow(object boolValue)
        {
            bool value = (bool)boolValue;
            gameOverScreen.SetActive(value);
        }

    }
}