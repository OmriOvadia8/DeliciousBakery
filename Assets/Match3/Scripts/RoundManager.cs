using DB_Core;
using UnityEngine;
using static DB_Match3.BoardSystem;
using DB_Game;

namespace DB_Match3
{
    public class RoundManager : DBMonoBehaviour
    {
        [SerializeField] private BoardSystem board;
        [SerializeField] DBPauseCurrencyManager rewardsManager;

        public int Match3Score;
        public int Match3ScoreGoal;
        public int MovesCount;
        private double reward;
        private double minimumReward = 1000;

        public bool PlayerInitiatedMove;
        public bool IsResolvingBoard;
        public bool PlayerWon;
        public bool IsGameOver;
        private bool gotReward;

        private void Start()
        {
            SetGameOverState(false);
            Match3VarsReset();
        }

        public void DecreaseMoves()
        {
            if (!PlayerInitiatedMove || MovesCount <= 0) return;

            MovesCount--;
            InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
            PlayerInitiatedMove = false;
        }

        public void SetGameOverState(bool isGameOver)
        {
            int canvasOrder = isGameOver ? 2 : 0;

            InvokeEvent(DBEventNames.Match3GameOverScreen, isGameOver);
            InvokeEvent(DBEventNames.Match3RestartButtonVisibility, isGameOver);
            InvokeEvent(DBEventNames.Match3CanvasOrder, canvasOrder);
        }

        public void RestartMatch3()
        {
            if (board.currentState != BoardState.Wait || !IsGameOver) return;

            ResetGameParameters();
            SetGameOverState(false);
        }

        private void ResetGameParameters()
        {
            board.RestartBoardAfterWin();
            Match3VarsReset();
            PlayerWon = false;
            IsGameOver = false;
            gotReward = false;
            board.currentState = BoardState.Move;
        }

        public void CheckGameState()
        {
            if (board.currentState != BoardState.Move) return;

            if (Match3Score >= Match3ScoreGoal && !PlayerWon)
            {
                EndGame(true);
                InvokeEvent(DBEventNames.PlaySound, SoundEffectType.UpgradeButtonClick);
                InvokeEvent(DBEventNames.Match3WinCoinsParticles, null);
            }
            else
            {
                EndGame(false);
            }
        }

        private void EndGame(bool isWon)
        {
            PlayerWon = isWon;
            IsGameOver = true;
            board.currentState = BoardState.Wait;
            InvokeEvent(DBEventNames.Match3GameEndText, isWon);
            SetGameOverState(true);
        }

        public void Match3RewardDecider()
        {
            reward = rewardsManager.Match3Reward() + minimumReward;
            InvokeEvent(DBEventNames.RewardTextMatch3, reward);
        }

        public void ScoreCheck(Gem gemToCheck)
        {
            Match3Score += gemToCheck.ScoreValue;
            InvokeEvent(DBEventNames.Match3ScoreTextIncrease, Match3Score);

            if (Match3Score >= Match3ScoreGoal && !gotReward)
            {
                InvokeEvent(DBEventNames.AddCurrencyUpdate, reward);
                InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.BuyButtonsCheck, null);
                
                gotReward = true;
            }
        }

        private void Match3VarsReset()
        {
            var matchScore = board.AllGems[0,0].ScoreValue * 5;
            var minMoves = 10;
            var maxMoves = 15;
            var minimumScore = minMoves * matchScore;
            var maximumScore = maxMoves * matchScore;
            Match3ScoreGoal = Random.Range(minimumScore, maximumScore).RoundUpToNearestTen();
            MovesCount = Random.Range(minMoves, maxMoves);
            Match3Score = 0;
            InvokeEvent(DBEventNames.Match3ScoreGoalText, Match3ScoreGoal);
            InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
            InvokeEvent(DBEventNames.Match3ScoreTextIncrease, Match3Score);
        }
    }
}
