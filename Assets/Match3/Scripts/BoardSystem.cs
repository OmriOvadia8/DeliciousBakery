using DB_Core;
using DB_Game;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class BoardSystem : DBMonoBehaviour
{
    public int Width;
    public int Height;

    public GameObject bgTilePrefab;
    public Gem[] Gems;
    public Gem[,] AllGems;

    public float GemSpeed;
    public MatchFinder MatchFinder;

    public Gem Bomb;
    public float bombChance = 2f;

    [SerializeField] BoardLayout boardLayout;
    public RoundManager roundMan;
    private Gem[,] layoutStore;

    public enum BoardState
    {
        Wait,
        Move
    }

    public BoardState currentState = BoardState.Move;

    private void Start()
    {
        AllGems = new Gem[Width, Height];
        layoutStore = new Gem[Width, Height];
        //Manager.PoolManager.InitPool("YourGemResourceName", Height * Width, this.transform, 100);
        //Manager.PoolManager.InitPool("YourBGTileResourceName", Height * Width, this.transform, 100);

        //Make all instantiations and destroys to 

        SetUp();

    }

    private void SetUp()
    {
        if(boardLayout != null)
        {
            layoutStore = boardLayout.GetLayout();
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                SpawnTilesBG(x, y);

                if (layoutStore[x,y] != null)
                {
                    SpawnGems(new Vector2Int(x, y), layoutStore[x, y]);
                }

                else
                {
                    int gemToUse = Random.Range(0, Gems.Length);

                    int iterations = 0;
                    while (MatchesAt(new Vector2Int(x, y), Gems[gemToUse]) && iterations < 100)
                    {
                        gemToUse = Random.Range(0, Gems.Length);
                        iterations++;
                    }

                    SpawnGems(new Vector2Int(x, y), Gems[gemToUse]);
                }
            }
        }
    }

    private void SpawnTilesBG(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);
        GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
        bgTile.transform.parent = transform;
        bgTile.name = $"Tile ({x},{y})";
    }

    private void SpawnGems(Vector2Int pos , Gem gemToSpawn)
    {
        if(Random.Range(0, 100f) < bombChance)
        {
            gemToSpawn = Bomb;
        }

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x,pos.y + Height,0), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = $"Gem ({pos.x},{pos.y})";
        AllGems[pos.x, pos.y] = gem;

        gem.SetUpGem(pos, this);
    }

    private bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if(posToCheck.x > 1)
        {
            if (AllGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && AllGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (AllGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && AllGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }

        return false;
    }

    private void DestroyMatchedGemsAt(Vector2Int position)
    {
        Gem gem = AllGems[position.x, position.y];
        if (gem != null)
        {
            if (gem.IsMatched)
            {
                Destroy(gem.gameObject);
                gem = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < MatchFinder.currentMatches.Count; i++)
        {
            if (MatchFinder.currentMatches[i] != null)
            {
                ScoreCheck(MatchFinder.currentMatches[i]);
                DestroyMatchedGemsAt(MatchFinder.currentMatches[i].PosIndex);
            }
        }
        roundMan.CheckWinState();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (AllGems[x,y] == null)
                {
                    nullCounter++;
                }
                else if(nullCounter > 0)
                {
                    AllGems[x, y].PosIndex.y -= nullCounter;
                    AllGems[x, y - nullCounter] = AllGems[x, y];
                    AllGems[x, y] = null;
                }

            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(0.5f);
        MatchFinder.FindAllMatches();

        if(MatchFinder.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(1);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            currentState = BoardState.Move;

            // Assuming that DestroyMatches sets the Board to 'Move' state,
            // perform your game state check here
            if (MatchFinder.currentMatches.Count == 0 && roundMan.MovesCount == 0)
            {
                roundMan.CheckGameState();
            }

            roundMan.CheckWinState();
        }

    }

    private void RefillBoard()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if(AllGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, Gems.Length);

                    SpawnGems(new Vector2Int(x, y), Gems[gemToUse]);
                }
            }
        }

        CheckMisplacedGems();
    }

    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsOfType<Gem>());

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (foundGems.Contains(AllGems[x,y]))
                {
                    foundGems.Remove(AllGems[x,y]);
                }
            }
        }

        foreach (var gems in foundGems)
        {
            Destroy(gems.gameObject);
        }
    }

    private void ScoreCheck(Gem gemToCheck)
    {
        roundMan.Match3Score += gemToCheck.ScoreValue;
        InvokeEvent(DBEventNames.Match3ScoreTextIncrease, roundMan.Match3Score);

        double reward = 12345678910;
        if(roundMan.Match3Score >= roundMan.Match3ScoreGoal)
        {
            InvokeEvent(DBEventNames.AddCurrencyUpdate, reward);
        }
    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.Wait)
        {
            currentState = BoardState.Wait;

            List<Gem> gemsFromBoard = new List<Gem>();


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    gemsFromBoard.Add(AllGems[x, y]);
                    AllGems[x, y] = null;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBoard.Count);
                    int iterations = 0;

                    while(MatchesAt(new Vector2Int(x,y), gemsFromBoard[gemToUse]) && iterations < 100 && gemsFromBoard.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBoard.Count);
                        iterations++;   
                    }

                    gemsFromBoard[gemToUse].SetUpGem(new Vector2Int(x, y), this);
                    AllGems[x, y] = gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }

            StartCoroutine(FillBoardCo());
        }
    }
}
