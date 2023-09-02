using DB_Core;
using UnityEngine;
using static DB_Match3.BoardSystem;

namespace DB_Match3
{
    public class RoundManager : DBMonoBehaviour
    {
        [SerializeField] private BoardSystem board;
        public int Match3Score;
        public int Match3ScoreGoal = 30;
        public int MovesCount = 2;
        public bool playerInitiatedMove;
        public bool isResolvingBoard;
        public bool playerWon;

        private void Start()
        {
            Match3GameOver(false);
        }

        /// <summary>
        /// Decreases the remaining moves count when initiated by the player.
        /// </summary>
        public void MovesDecrease()
        {
            if (playerInitiatedMove && MovesCount > 0)
            {
                MovesCount--;
                InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
                playerInitiatedMove = false; // Reset the flag
            }
        }

        /// <summary>
        /// Handles the game-over state.
        /// </summary>
        public void Match3GameOver(bool value)
        {
            InvokeEvent(DBEventNames.Match3GameOverScreen, value);
        }

        /// <summary>
        /// Restarts the Match-3 game.
        /// </summary>
        public void RestartMatch3()
        {
            if (board.currentState == BoardState.Wait && playerWon == true)
            {
                board.RestartBoardAfterWin();
                Match3Score = 0;
                MovesCount = 10;
                Match3GameOver(false);
                InvokeEvent(DBEventNames.Match3ScoreTextIncrease, Match3Score);
                InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
                playerWon = false;
                board.currentState = BoardState.Move;
                Debug.Log("Do Something");
            }
            Debug.Log("Do nothing");
        }

        /// <summary>
        /// Checks the overall game state.
        /// </summary>
        public void CheckGameState()
        {
            if (board.currentState == BoardSystem.BoardState.Move)
            {
                if (Match3Score >= Match3ScoreGoal && !playerWon)
                {
                    DisplayWinMessage();
                }
                else
                {
                    DisplayLoseMessage();
                }
            }
        }

        /// <summary>
        /// Checks the win state based on the current score.
        /// </summary>
        public void CheckWinState()
        {
            if (Match3Score >= Match3ScoreGoal && !playerWon)
            {
                DisplayWinMessage();
            }

            else if (playerWon)
            {
                SetBoardStateToWait();
            }
        }

        /// <summary>
        /// Displays a win message and sets the board state to Wait.
        /// </summary>
        private void DisplayWinMessage()
        {
            Debug.Log("You Won!");
            playerWon = true;
            SetBoardStateToWait();
        }

        /// <summary>
        /// Displays a lose message and sets the board state to Wait.
        /// </summary>
        private void DisplayLoseMessage()
        {
            Debug.Log("You Lost!");
            SetBoardStateToWait();
        }

        /// <summary>
        /// Sets the board state to Wait.
        /// </summary>
        private void SetBoardStateToWait()
        {
            board.currentState = BoardSystem.BoardState.Wait;
        }
    }
}