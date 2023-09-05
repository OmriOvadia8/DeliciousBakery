using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DB_Core;

namespace DB_Match3
{
    public class MatchFinder : DBMonoBehaviour
    {
        [SerializeField] private BoardSystem board;
        public List<Gem> currentMatches = new List<Gem>();

        public void FindAllMatches()
        {
            currentMatches.Clear();
            
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Gem currentGem = board.AllGems[x, y];
                    if (currentGem == null) continue;

                    CheckHorizontalMatches(x, y, currentGem);
                    CheckVerticalMatches(x, y, currentGem);
                }
            }

            RemoveDuplicateMatches();
            CheckForBombs();
        }

        private void CheckHorizontalMatches(int x, int y, Gem currentGem)
        {
            if (x <= 0 || x >= board.Width - 1) return;

            Gem leftGem = board.AllGems[x - 1, y];
            Gem rightGem = board.AllGems[x + 1, y];

            if (leftGem != null && rightGem != null &&
                leftGem.type == currentGem.type && rightGem.type == currentGem.type &&
                currentGem.type != Gem.GemType.Stone)
            {
                MarkAsMatched(currentGem, leftGem, rightGem);
            }
        }

        private void CheckVerticalMatches(int x, int y, Gem currentGem)
        {
            if (y <= 0 || y >= board.Height - 1) return;

            Gem aboveGem = board.AllGems[x, y + 1];
            Gem belowGem = board.AllGems[x, y - 1];

            if (aboveGem != null && belowGem != null &&
                aboveGem.type == currentGem.type && belowGem.type == currentGem.type &&
                currentGem.type != Gem.GemType.Stone)
            {
                MarkAsMatched(currentGem, aboveGem, belowGem);
            }
        }

        private void MarkAsMatched(params Gem[] gems)
        {
            foreach (Gem gem in gems)
            {
                gem.IsMatched = true;
                currentMatches.Add(gem);
            }
        }

        private void RemoveDuplicateMatches()
        {
            if (currentMatches.Count > 0)
            {
                currentMatches = currentMatches.Distinct().ToList();
            }
        }

        public void CheckForBombs()
        {
            for (int i = 0; i < currentMatches.Count; i++)
            {
                Gem gem = currentMatches[i];
                int x = gem.PosIndex.x;
                int y = gem.PosIndex.y;

                CheckForAdjacentBomb(x - 1, y);
                CheckForAdjacentBomb(x + 1, y);
                CheckForAdjacentBomb(x, y - 1);
                CheckForAdjacentBomb(x, y + 1);
            }
            RemoveDuplicateMatches();
        }

        private void CheckForAdjacentBomb(int x, int y)
        {
            if (x >= 0 && x < board.Width && y >= 0 && y < board.Height)
            {
                Gem adjacentGem = board.AllGems[x, y];
                if (adjacentGem != null && adjacentGem.type == Gem.GemType.Bomb)
                {
                    MarkBombArea(new Vector2Int(x, y), adjacentGem);
                }
            }
        }

        public void MarkBombArea(Vector2Int bombPos, Gem bomb)
        {
            for (int x = bombPos.x - bomb.BlastSize; x <= bombPos.x + bomb.BlastSize; x++)
            {
                for (int y = bombPos.y - bomb.BlastSize; y <= bombPos.y + bomb.BlastSize; y++)
                {
                    if (x < 0 || x >= board.Width || y < 0 || y >= board.Height) continue;

                    Gem targetGem = board.AllGems[x, y];
                    if (targetGem != null)
                    {
                        targetGem.IsMatched = true;
                        currentMatches.Add(targetGem);
                    }
                }
            }

            RemoveDuplicateMatches();
        }
    }
}