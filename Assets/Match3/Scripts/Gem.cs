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
            HandleMouseDown();
        }

        private void HandleMouseDown()
        {
            if (IsBoardInMoveState())
            {
                SetFirstTouchPosition();
            }
        }

        private bool IsBoardInMoveState()
        {
            return Board.currentState == BoardSystem.BoardState.Move;
        }

        private void SetFirstTouchPosition() => firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        private void Update() => HandleMovement();

        private void HandleMovement()
        {
            if (IsGemMoving())
            {
                SmoothMoveGem();
            }
            else
            {
                SnapGemToGrid();
            }
        }

        private bool IsGemMoving()
        {
            return Vector2.Distance(transform.position, PosIndex) > 0.01f;
        }

        private void SmoothMoveGem() => transform.position = Vector2.Lerp(transform.position, PosIndex, Board.GemSpeed * Time.deltaTime);

        private void SnapGemToGrid()
        {
            transform.position = new Vector3(PosIndex.x, PosIndex.y, 0);
            Board.AllGems[PosIndex.x, PosIndex.y] = this;
        }

        private void OnMouseUp() => HandleMouseUp();

        private void HandleMouseUp()
        {
            if (IsBoardInMoveState())
            {
                SetFinalTouchPosition();
                CalculateSwipeAngleAndMovePieces();
            }
        }

        private void SetFinalTouchPosition() => finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        private void CalculateSwipeAngleAndMovePieces()
        {
            CalculateSwipeAngle();
            if (IsSwipeLongEnough())
            {
                MovePieces();
            }
        }

        private void CalculateSwipeAngle() => swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;

        private bool IsSwipeLongEnough()
        {
            return Vector3.Distance(firstTouchPos, finalTouchPos) > 0.5f;
        }
        private void MovePieces()
        {
            SetPreviousPosition();
            PrepareBoardForMove();
            StartCoroutine(CheckMoveCo());
        }

        private void SetPreviousPosition() => previousPos = PosIndex;

        private void PrepareBoardForMove()
        {
            Board.roundMan.IsResolvingBoard = true;

            if (Board.roundMan.MovesCount > 0)
            {
                Board.roundMan.PlayerInitiatedMove = true;
                SwapGems();
            }
        }

        private void SwapGems()
        {
            if (SwipeRight())
            {
                otherGem = Board.AllGems[PosIndex.x + 1, PosIndex.y];
                SwapPositions(1, 0);
            }
            else if (SwipeLeft())
            {
                otherGem = Board.AllGems[PosIndex.x - 1, PosIndex.y];
                SwapPositions(-1, 0);
            }
            else if (SwipeUp())
            {
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y + 1];
                SwapPositions(0, 1);
            }
            else if (SwipeDown())
            {
                otherGem = Board.AllGems[PosIndex.x, PosIndex.y - 1];
                SwapPositions(0, -1);
            }
        }

        private bool SwipeRight() => swipeAngle > -45 && swipeAngle <= 45 && PosIndex.x < Board.Width - 1;
        private bool SwipeLeft() => (swipeAngle > 135 || swipeAngle <= -135) && PosIndex.x > 0;
        private bool SwipeUp() => swipeAngle > 45 && swipeAngle <= 135 && PosIndex.y < Board.Height - 1;
        private bool SwipeDown() => swipeAngle > -135 && swipeAngle <= -45 && PosIndex.y > 0;

        private void SwapPositions(int x, int y)
        {
            otherGem.PosIndex = PosIndex;
            UpdatePosition(x, y);
        }

        private void UpdatePosition(int x, int y)
        {
            PosIndex.x += x;
            PosIndex.y += y;
            Board.AllGems[PosIndex.x, PosIndex.y] = this;
            Board.AllGems[otherGem.PosIndex.x, otherGem.PosIndex.y] = otherGem;
        }

        public IEnumerator CheckMoveCo()
        {
            Board.currentState = BoardSystem.BoardState.Wait;
            InvokeEvent(DBEventNames.Match3ReturnButton, false);
            yield return new WaitForSeconds(0.5f);

            Board.MatchFinder.FindAllMatches();

            if (!IsMatched && !otherGem.IsMatched)
            {
                RevertMove();
                yield return new WaitForSeconds(0.5f);
                InvokeEvent(DBEventNames.Match3ReturnButton, true);
                Board.currentState = BoardSystem.BoardState.Move;
            }
            else
            {
                CompleteMove();
            }
        }

        private void RevertMove()
        {
            Board.roundMan.PlayerInitiatedMove = false;
            otherGem.PosIndex = PosIndex;
            PosIndex = previousPos;

            UpdatePosition(0, 0);
        }

        private void CompleteMove()
        {
            if (Board.roundMan.PlayerInitiatedMove)
            {
                Board.roundMan.DecreaseMoves();
            }
            Board.DestroyMatches();
        }
    }
}