using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    [SerializeField] BoardSystem board;

    public List<Gem> currentMatches = new List<Gem>();

    public void FindAllMatches()
    {
        currentMatches.Clear();

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Gem currentGem = board.AllGems[x, y];
                if(currentGem != null)
                {
                    if(x > 0 && x < board.Width - 1)
                    {
                        Gem leftGem = board.AllGems[x - 1, y];
                        Gem rightGem = board.AllGems[x + 1, y];

                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type && currentGem.type != Gem.GemType.Stone)
                            {
                                currentGem.IsMatched = true;
                                leftGem.IsMatched = true;
                                rightGem.IsMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);
                            }
                        }
                    }

                    if (y > 0 && y < board.Height - 1)
                    {
                        Gem aboveGem = board.AllGems[x, y + 1];
                        Gem belowGem = board.AllGems[x, y - 1];

                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.type == currentGem.type && belowGem.type == currentGem.type && currentGem.type != Gem.GemType.Stone)
                            {
                                currentGem.IsMatched = true;
                                aboveGem.IsMatched = true;
                                belowGem.IsMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(aboveGem);
                                currentMatches.Add(belowGem);
                            }
                        }
                    }
                }


            }
        }

        if(currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }

        CheckForBombs();
        board.roundMan.CheckWinState();
    }

    public void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];

            int x = gem.PosIndex.x;
            int y = gem.PosIndex.y;

            if(gem.PosIndex.x > 0)
            {
                if (board.AllGems[x-1,y] != null)
                {
                    if (board.AllGems[x-1,y].type == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int (x-1, y), board.AllGems[x-1,y]);
                    }
                }
            }


            if (gem.PosIndex.x < board.Width - 1)
            {
                if (board.AllGems[x + 1, y] != null)
                {
                    if (board.AllGems[x + 1, y].type == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.AllGems[x + 1, y]);
                    }
                }
            }

            if (gem.PosIndex.y > 0)
            {
                if (board.AllGems[x, y - 1] != null)
                {
                    if (board.AllGems[x, y - 1].type == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), board.AllGems[x, y - 1]);
                    }
                }
            }


            if (gem.PosIndex.y < board.Height - 1)
            {
                if (board.AllGems[x, y + 1] != null)
                {
                    if (board.AllGems[x, y + 1].type == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.AllGems[x, y + 1]);
                    }
                }
            }

        }
    }

    public void MarkBombArea(Vector2Int bombPos, Gem bomb)
    {
        for (int x = bombPos.x - bomb.BlastSize; x <= bombPos.x + bomb.BlastSize; x++)
        {
            for (int y = bombPos.y - bomb.BlastSize; y <= bombPos.y + bomb.BlastSize; y++)
            {
                if(x >= 0 && x < board.Width && y >= 0 && y < board.Height)
                {
                    if (board.AllGems[x,y] != null)
                    {
                        board.AllGems[x, y].IsMatched = true;
                        currentMatches.Add(board.AllGems[x, y]);
                    }
                }
            }   
        }

        currentMatches = currentMatches.Distinct().ToList();
    }
}
