using DB_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gem;

public class BoardSystem : DBMonoBehaviour
{
    [Header("Board Settings")]
    public int Width;
    public int Height;

    [Header("Prefab Settings")]
    public GameObject bgTilePrefab;
    public Gem[] Gems;
    public Gem Bomb;

    [Header("Gameplay Settings")]
    public float GemSpeed;
    public float bombChance = 2f;

    [Header("Dependencies")]
    [SerializeField] private BoardLayout boardLayout;
    public RoundManager roundMan;
    public MatchFinder MatchFinder;

    public Gem[,] AllGems;
    private Gem[,] layoutStore;

    public enum BoardState { Wait, Move }
    public BoardState currentState = BoardState.Move;

    private void Start() => InitializeBoard();

    private void InitializeBoard()
    {
        InitializeGemArrays();
        PopulateBoard();
    }

    private void InitializeGemArrays()
    {
        AllGems = new Gem[Width, Height];
        layoutStore = boardLayout?.GetLayout();
    }

    private void PopulateBoard()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                SpawnBackgroundTile(x, y);
                SpawnInitialGems(x, y);
            }
        }
    }

    private void FillBoard(List<Gem> gemList = null)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (AllGems[x, y] == null)
                {
                    Gem gemToUse = gemList != null ? gemList[Random.Range(0, gemList.Count)] : GetRandomGem(x, y);
                    SpawnGemAtPosition(new Vector2Int(x, y), gemToUse);

                    if (gemList != null)
                    {
                        gemList.Remove(gemToUse);
                    }
                }
            }
        }

        CheckMisplacedGems();
    }

    private Gem GetGemAtPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height)
        {
            return null;
        }
        return AllGems[position.x, position.y];
    }

    private void SpawnBackgroundTile(int x, int y)
    {
        Vector2 position = new Vector2(x, y);
        GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity, transform);
        bgTile.name = $"Tile ({x},{y})";
    }

    private void SpawnInitialGems(int x, int y)
    {
        Gem gemToSpawn = layoutStore?[x, y] ?? GetRandomGem(x, y);
        SpawnGemAtPosition(new Vector2Int(x, y), gemToSpawn);
    }

    private Gem GetRandomGem(int x, int y)
    {
        int iterations = 0;
        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, Gems.Length);
            iterations++;
        } while (HasMatchAt(new Vector2Int(x, y), Gems[randomIndex]) && iterations < 100);

        return Gems[randomIndex];
    }

    private void SpawnGemAtPosition(Vector2Int position, Gem gemToSpawn)
    {
        if (Random.Range(0, 100f) < bombChance) gemToSpawn = Bomb;

        Vector3 spawnPosition = new Vector3(position.x, position.y + Height, 0);
        Gem newGem = Instantiate(gemToSpawn, spawnPosition, Quaternion.identity, transform);
        newGem.name = $"Gem ({position.x},{position.y})";
        newGem.SetUpGem(position, this);

        AllGems[position.x, position.y] = newGem;
    }

    private bool HasMatchAt(Vector2Int position, Gem gem)
    {
        Gem left1 = GetGemAtPosition(new Vector2Int(position.x - 1, position.y));
        Gem left2 = GetGemAtPosition(new Vector2Int(position.x - 2, position.y));

        Gem down1 = GetGemAtPosition(new Vector2Int(position.x, position.y - 1));
        Gem down2 = GetGemAtPosition(new Vector2Int(position.x, position.y - 2));

        if ((left1?.type == gem.type && left2?.type == gem.type) ||
            (down1?.type == gem.type && down2?.type == gem.type))
        {
            return true;
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

    private void RefillBoard() => FillBoard();

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
                    gemsFromBoard.Add(GetGemAtPosition(new Vector2Int(x, y)));
                    AllGems[x, y] = null;
                }
            }

            FillBoard(gemsFromBoard);

            StartCoroutine(FillBoardCo());
        }
    }
}
