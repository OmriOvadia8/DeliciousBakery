using DB_Core;
using UnityEngine;
using static DB_Match3.BoardSystem;

namespace DB_Match3
{
    public class RoundManager : DBMonoBehaviour
    {
        [SerializeField] private BoardSystem board;

        public int Match3Score;
        public int Match3ScoreGoal = 200;
        public int MovesCount = 2;

        public bool PlayerInitiatedMove;
        public bool IsResolvingBoard;
        public bool PlayerWon;
        public bool IsGameOver;

        private void Start()
        {
            SetGameOverState(false);
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
            Match3Score = 0;
            MovesCount = 10;
            PlayerWon = false;
            IsGameOver = false;
            board.currentState = BoardState.Move;

            InvokeEvent(DBEventNames.Match3ScoreTextIncrease, Match3Score);
            InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
        }

        public void CheckGameState()
        {
            if (board.currentState != BoardState.Move) return;

            if (Match3Score >= Match3ScoreGoal && !PlayerWon)
            {
                EndGame(true);
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
    }
}
