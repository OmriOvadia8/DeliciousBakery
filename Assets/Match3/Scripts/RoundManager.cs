using DB_Core;
using UnityEngine;

public class RoundManager : DBMonoBehaviour
{
    [SerializeField] BoardSystem board;
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

    public void MovesDecrease()
    {
        Debug.Log("Inside MovesDecrease method");  // Debugging line
        if (playerInitiatedMove && MovesCount > 0)
        {
            Debug.Log("Decreasing moves");  // Debugging line
            MovesCount--;
            InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
            playerInitiatedMove = false; // reset the flag
        }
        else
        {
            Debug.Log("Not decreasing moves because some condition is not met"); // Debugging line
        }
    }


    public void Match3GameOver(bool value)
    {
        // Simplify the logic for invoking the game over screen
        InvokeEvent(DBEventNames.Match3GameOverScreen, value);
    }

    public void RestartMatch3()
    {
        board.ShuffleBoard();
        Match3Score = 0;
        MovesCount = 20;
        Match3GameOver(false);
    }

    public void CheckGameState()
    {
        // Only check for game state when there are 0 moves and the board is in 'Move' state
        if (board.currentState == BoardSystem.BoardState.Move)
        {
            // If player reaches the score goal, they win
            if (Match3Score >= Match3ScoreGoal && playerWon == false)
            {
                // Do whatever you want when the game is won, like transitioning scenes or displaying a message
                Debug.Log("You Won!");
            }
            else // If moves are zero and the player hasn't reached the score goal, they lose
            {
                // Do whatever you want when the game is lost, like transitioning scenes or displaying a message
                Debug.Log("You Lost!");
            }
        }
    }

    public void CheckWinState()
    {
        if(Match3Score >= Match3ScoreGoal && playerWon == false)
        {
            Debug.Log("You Won!");
            playerWon = true;   
            board.currentState = BoardSystem.BoardState.Wait;
        }
    }
}
