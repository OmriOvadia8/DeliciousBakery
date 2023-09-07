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
            UpdateUIElements();
            AddEvents();
        }

        private void OnDisable() => RemoveEvents();

        private void UpdateUIElements()
        {
            ScoreTextIncrease(roundManager.Match3Score);
            MovesTextUpdate(roundManager.MovesCount);
            scoreGoalText.text = $"{roundManager.Match3ScoreGoal}";
        }

        private void ScoreTextIncrease(object score) => scoreText.text = $"{score}";

        private void MovesTextUpdate(object movesCount) => movesLeftText.text = $"{movesCount}";

        private void GameOverScreenShow(object show) => gameOverScreen.SetActive((bool)show);

        private void GameOverText(object won) => gameEndText.text = (bool)won ? "You Win!" : "Game Over!";

        private void ReturnButtonInteractable(object enabled) => returnButton.interactable = (bool)enabled;

        private void CanvasOrder(object order) => match3Canvas.sortingOrder = (int)order;

        private void RestartButtonVisibility(object show) => restartButton.SetActive((bool)show);

        private void SetToastDefaults()
        {
            scoreToastText.transform.localScale = Vector3.one;
            scoreToastText.color = new Color(scoreToastText.color.r, scoreToastText.color.g, scoreToastText.color.b, 1);
            scoreToastText.transform.localPosition = new Vector3(0, 60, 0);
            scoreToastText.gameObject.SetActive(true);
        }

        private void AnimateToast()
        {
            scoreToastText.transform.DOScale(1.2f, 0.3f);
            scoreToastText.transform.DOLocalMoveY(90f, 1f);
            DOTween.To(
                () => scoreToastText.color,
                x => scoreToastText.color = x,
                new Color(scoreToastText.color.r, scoreToastText.color.g, scoreToastText.color.b, 0),
                1f
            ).OnComplete(() => StartCoroutine(ToastDestruction()));
        }

        private void ScoreToastTextIncrease(object score)
        {
            int scoreGain = (int)score;
            scoreToastText.text = $"+{scoreGain}";

            SetToastDefaults();
            AnimateToast();
        }

        private IEnumerator ToastDestruction()
        {
            SetToastDefaults(); 
            scoreToastText.gameObject.SetActive(false);

            yield return null;
        }

        private void AddEvents()
        {
            AddListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            AddListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            AddListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            AddListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
            AddListener(DBEventNames.Match3RestartButtonVisibility, RestartButtonVisibility);
            AddListener(DBEventNames.Match3CanvasOrder, CanvasOrder);
            AddListener(DBEventNames.Match3GameEndText, GameOverText);
            AddListener(DBEventNames.Match3ReturnButton, ReturnButtonInteractable);
        }

        private void RemoveEvents()
        {
            RemoveListener(DBEventNames.Match3ScoreTextIncrease, ScoreTextIncrease);
            RemoveListener(DBEventNames.Match3MovesTextUpdate, MovesTextUpdate);
            RemoveListener(DBEventNames.Match3GameOverScreen, GameOverScreenShow);
            RemoveListener(DBEventNames.Match3ScoreToast, ScoreToastTextIncrease);
            RemoveListener(DBEventNames.Match3RestartButtonVisibility, RestartButtonVisibility);
            RemoveListener(DBEventNames.Match3CanvasOrder, CanvasOrder);
            RemoveListener(DBEventNames.Match3GameEndText, GameOverText);
            RemoveListener(DBEventNames.Match3ReturnButton, ReturnButtonInteractable);
        }
    }
}