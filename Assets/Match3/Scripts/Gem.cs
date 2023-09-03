using DB_Core;
using System.Collections;
using UnityEngine;

namespace DB_Match3
{
    public class Gem : DBMonoBehaviour
    {
        [HideInInspector]
        public Vector2Int PosIndex;
        [HideInInspector]
        public BoardSystem Board;

        private Vector2 firstTouchPos;
        private Vector2 finalTouchPos;
        private float swipeAngle;
        private Gem otherGem;
        private Vector2Int previousPos;

        public bool IsMatched;
        public int ScoreValue = 10;
        public SpriteRenderer gemSprite;
        public int BlastSize = 2;
        public GameObject DestroyEffect;

        public enum GemType
        {
            Blue, Green, Red, Yellow, Purple, Bomb, Stone
        }

        public GemType type;

        public void SetUpGem(Vector2Int pos, BoardSystem theBoard)
        {
            PosIndex = pos;
            Board = theBoard;
        }

        private void OnMouseDown()
        {
            if (Board.currentState == BoardSystem.BoardState.Move)
            {
                firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (Vector2.Distance(transform.position, PosIndex) > 0.01f)
            {
                transform.position = Vector2.Lerp(transform.position, PosIndex, Board.GemSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(PosIndex.x, PosIndex.y, 0);
                Board.AllGems[PosIndex.x, PosIndex.y] = this;
            }
        }

        private void OnMouseUp()
        {
            if (Board.currentState == BoardSystem.BoardState.Move)
            {
                finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }

        private void CalculateAngle()
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;

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
                SwapGems();
            }

            StartCoroutine(CheckMoveCo());
        }

        private void SwapGems()
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && PosIndex.x < Board.Width - 1)
            {
                // Right
                otherGem = Board.AllGems[PosIndex.x + 1, PosIndex.y];
                SwapPositions(1, 0);
            }
            else if (swipeAngle > 135 || swipeAngle <= -135 && PosIndex.x > 0)
            {
                // Left
                otherGem = Board.AllGems[PosIndex.x - 1, PosIndex.y];
                SwapPositions(-1, 0);
            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && PosIndex.y < Board.Height - 1)
            {
                // Up
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y + 1];
                SwapPositions(0, 1);
            }
            else if (swipeAngle > -135 && swipeAngle <= -45 && PosIndex.y > 0)
            {
                // Down
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y - 1];
                SwapPositions(0, -1);
            }
        }

        private void SwapPositions(int x, int y)
        {
            otherGem.PosIndex = PosIndex;
            PosIndex.x += x;
            PosIndex.y += y;

            Board.AllGems[PosIndex.x, PosIndex.y] = this;
            Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;
        }

        public IEnumerator CheckMoveCo()
        {
            Board.currentState = BoardSystem.BoardState.Wait;
            yield return new WaitForSeconds(0.5f);

            Board.MatchFinder.FindAllMatches();

            if (!IsMatched && !otherGem.IsMatched)
            {
                RevertMove();
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

        private void RevertMove()
        {
            Board.roundMan.playerInitiatedMove = false;
            otherGem.PosIndex = PosIndex;
            PosIndex = previousPos;

            Board.AllGems[PosIndex.x, PosIndex.y] = this;
            Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;
        }
    }
}