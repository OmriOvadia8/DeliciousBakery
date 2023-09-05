using UnityEngine;
using TMPro;
using DB_Core;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

namespace DB_Match3
{
    public class Match3UIManager : DBMonoBehaviour
    {
        [SerializeField] Canvas match3Canvas;
        [SerializeField] RoundManager roundManager;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text movesLeftText;
        [SerializeField] TMP_Text scoreGoalText;
        [SerializeField] GameObject gameOverScreen;
        [SerializeField] TMP_Text scoreToastText;
        [SerializeField] GameObject restartButton;
        [SerializeField] TMP_Text gameEndText;
        [SerializeField] Button returnButton;

        private void OnEnable()
        {
            ScoreTextIncrease(roundManager.Match3Score);
            MovesTextUpdate(roundManager.MovesCount);
            scoreGoalText.text = $"{roundManager.Match3ScoreGoal}";
            AddListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            AddListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            AddListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            AddListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
            AddListener(DBEventNames.Match3RestartButtonVisibility, Match3RestartButtonVisibility);
            AddListener(DBEventNames.Match3CanvasOrder, Match3CanvasOrder);
            AddListener(DBEventNames.Match3GameEndText, GameOverText);
            AddListener(DBEventNames.Match3ReturnButton, Match3ReturnButtonInteractable);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            RemoveListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            RemoveListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            RemoveListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
            RemoveListener(DBEventNames.Match3RestartButtonVisibility, Match3RestartButtonVisibility);
            RemoveListener(DBEventNames.Match3CanvasOrder, Match3CanvasOrder);
            RemoveListener(DBEventNames.Match3GameEndText, GameOverText);
            RemoveListener(DBEventNames.Match3ReturnButton, Match3ReturnButtonInteractable);
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

        private void GameOverText(object winLoseText)
        {
            bool winOrLose = (bool)winLoseText;

            if(winOrLose)
            {
                gameEndText.text = "You Win!";
            }
            else
            {
                gameEndText.text = "Game Over!";
            }
        }

        private void Match3ReturnButtonInteractable(object boolValue)
        {
            bool value = (bool)boolValue;
            returnButton.interactable = value;
        }

        private void Match3CanvasOrder(object orderNum)
        {
            int sortingOrderNumber = (int)orderNum;
            match3Canvas.sortingOrder = sortingOrderNumber;
        }

        private void ScoreToastTextIncrease(object score)
        {
            int scoreGain = (int)score;
            scoreToastText.text = $"+{scoreGain}";

            // Set to start state before animating
            scoreToastText.transform.localScale = Vector3.one;
            scoreToastText.color = new Color(scoreToastText.color.r, scoreToastText.color.g, scoreToastText.color.b, 1);
            scoreToastText.transform.localPosition = new Vector3(0, 60, 0);

            scoreToastText.gameObject.SetActive(true);

            // DOTween animations
            // Scale up
            scoreToastText.transform.DOScale(1.2f, 0.3f);

            // Move up by 50 units from the current position and fade out over 1 second
            scoreToastText.transform.DOLocalMoveY(60f + 30f, 1f);

            // Fade out
            DOTween.To(() => scoreToastText.color, x => scoreToastText.color = x, new Color(scoreToastText.color.r, scoreToastText.color.g, scoreToastText.color.b, 0), 1f).OnComplete(() => StartCoroutine(ToastDestruction()));
        }

        private IEnumerator ToastDestruction()
        {
            // Reset to starting states, just in case you'll use the object again later
            scoreToastText.transform.localScale = Vector3.one;
            scoreToastText.color = new Color(scoreToastText.color.r, scoreToastText.color.g, scoreToastText.color.b, 1);
            scoreToastText.transform.localPosition = new Vector3(0, 60, 0);

            scoreToastText.gameObject.SetActive(false);

            yield return null;
        }

        private void Match3RestartButtonVisibility(object boolValue)
        {
            bool value = (bool)boolValue;
            restartButton.SetActive(value);
        }

    }
}