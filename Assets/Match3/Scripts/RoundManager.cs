using DB_Core;
using UnityEngine;

public class RoundManager : DBMonoBehaviour
{
    [SerializeField] BoardSystem board;
    public int Match3Score;
    public int Match3ScoreGoal = 30;
    public int MovesCount = 10;
    public bool playerInitiatedMove;

    private void Start()
    {
        Match3GameOver(false);
    }

    public void MovesDecrease()
    {
        if (playerInitiatedMove && MovesCount > 0)
        {
            MovesCount--;
            InvokeEvent(DBEventNames.Match3MovesTextUpdate, MovesCount);
            playerInitiatedMove = false; // reset the flag
        }

        else
        {
            Match3GameOver(true);
        }
    }

    public void Match3GameOver(bool value)
    {
        InvokeEvent(DBEventNames.Match3GameOverScreen, value);
    }

    public void RestartMatch3()
    {
        board.ShuffleBoard();
    }
}
