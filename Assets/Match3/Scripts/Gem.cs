using DB_Core;
using System.Collections;
using UnityEngine;

public class Gem : DBMonoBehaviour
{
    [HideInInspector]
    public Vector2Int PosIndex;
    [HideInInspector]
    public BoardSystem Board;

    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;

    public bool IsMatched;
    private float swipeAngle;
    public int ScoreValue = 10;

    public SpriteRenderer gemSprite;

    private Gem otherGem;

    public int BlastSize = 2;

    private Vector2Int previousPos;

    public enum GemType
    {
        Blue,
        Green,
        Red,
        Yellow,
        Purple,
        Bomb,
        Stone
    }

    public GemType type;

    public void SetUpGem(Vector2Int pos, BoardSystem theBoard)
    {
        PosIndex = pos;
        Board = theBoard;
    }

    private void OnMouseDown()
    {
        if(Board.currentState == BoardSystem.BoardState.Move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, PosIndex) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, PosIndex, Board.GemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3 (PosIndex.x, PosIndex.y, 0);
            Board.AllGems[PosIndex.x, PosIndex.y] = this;
        }
    }

    private void OnMouseUp()
    {
        if(Board.currentState == BoardSystem.BoardState.Move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y , finalTouchPos.x - firstTouchPos.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPos, finalTouchPos) > 0.5f)
        {
            MovePieces();
        }
    }

    private void MovePieces()
    {
        previousPos = PosIndex;
        Board.roundMan.isResolvingBoard = true;

        if (Board.roundMan.MovesCount > 0)
        {
            Board.roundMan.playerInitiatedMove = true;

            if (swipeAngle < 45 && swipeAngle > -45 && PosIndex.x < Board.Width - 1) // right
            {
                otherGem = Board.AllGems[PosIndex.x + 1, PosIndex.y];
                otherGem.PosIndex.x--;
                PosIndex.x++;

                Board.AllGems[PosIndex.x, PosIndex.y] = this;
                Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;

            }

            else if ((swipeAngle > 135 || swipeAngle < -135) && PosIndex.x > 0) // left
            {
                otherGem = Board.AllGems[PosIndex.x - 1, PosIndex.y];
                otherGem.PosIndex.x++;
                PosIndex.x--;

                Board.AllGems[PosIndex.x, PosIndex.y] = this;
                Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;

            }

            else if (swipeAngle > 45 && swipeAngle <= 135 && PosIndex.y < Board.Height - 1) // swipe up
            {
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y + 1];
                otherGem.PosIndex.y--;
                PosIndex.y++;

                Board.AllGems[PosIndex.x, PosIndex.y] = this;
                Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;

            }

            else if (swipeAngle < 45 && swipeAngle >= -135 && PosIndex.y > 0) // swipe down
            {
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y - 1];
                otherGem.PosIndex.y++;
                PosIndex.y--;

                Board.AllGems[PosIndex.x, PosIndex.y] = this;
                Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;
            }
        }

        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        Board.currentState = BoardSystem.BoardState.Wait;

        yield return new WaitForSeconds(0.5f);

        Board.MatchFinder.FindAllMatches();

        if (otherGem != null)
        {
            if (!IsMatched && !otherGem.IsMatched)
            {
                // Reverting the move, so we shouldn't decrement the moves count.
                Board.roundMan.playerInitiatedMove = false;
                otherGem.PosIndex = PosIndex;
                PosIndex = previousPos;

                Board.AllGems[PosIndex.x, PosIndex.y] = this;
                Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;

                yield return new WaitForSeconds(0.5f);

                Board.currentState = BoardSystem.BoardState.Move;
            }
            else
            {
                if (Board.roundMan.playerInitiatedMove)
                {
                    Board.roundMan.MovesDecrease();
                }

                Board.DestroyMatches();
            }
        }
    }



}