using DB_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DB_Game;

namespace DB_Match3
{
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
                    if(gem.type == Gem.GemType.Bomb)
                    {
                        InvokeEvent(DBEventNames.PlaySound, SoundEffectType.Match3Bomb);
                    }
                    else
                    {
                        InvokeEvent(DBEventNames.PlaySound, SoundEffectType.Match3GemBreak);
                    }
                    Instantiate(gem.DestroyEffect, new Vector2(position.x, position.y), Quaternion.identity);
                    Destroy(gem.gameObject);
                    gem = null;
                }
            }
        }

        public void DestroyMatches()
        {
            int totalScore = 0;
            List<Vector2Int> matchedPositions = new List<Vector2Int>();
            bool isBomb = ProcessMatchesAndGatherInfo(ref totalScore, matchedPositions);

            if (ShouldSpawnBomb(totalScore, isBomb))
            {
                SpawnRandomBomb(matchedPositions);
            }

            InvokeEvent(DBEventNames.Match3ScoreToast, totalScore);
            StartCoroutine(DecreaseRowCo());
        }

        private bool ProcessMatchesAndGatherInfo(ref int totalScore, List<Vector2Int> matchedPositions)
        {
            bool isBomb = false;
            int gemScoreValue = MatchFinder.currentMatches[0].ScoreValue; // Assuming list is not empty

            foreach (var match in MatchFinder.currentMatches)
            {
                if (match == null) continue;

                ScoreCheck(match);
                matchedPositions.Add(match.PosIndex);
                DestroyMatchedGemsAt(match.PosIndex);

                totalScore += gemScoreValue;
                if (match.type == Gem.GemType.Bomb)
                {
                    isBomb = true;
                }
            }
            return isBomb;
        }

        private bool ShouldSpawnBomb(int totalScore, bool isBomb)
        {
            int gemScoreValue = MatchFinder.currentMatches[0].ScoreValue; // Assuming list is not empty
            return (totalScore >= (gemScoreValue * 4)) && !isBomb;
        }

        private void SpawnRandomBomb(List<Vector2Int> matchedPositions)
        {
            int randomIndex = UnityEngine.Random.Range(0, matchedPositions.Count);
            Vector2Int bombPosition = matchedPositions[randomIndex];
            SpawnGemAtPosition(bombPosition, Bomb);
        }


        private IEnumerator DecreaseRowCo()
        {
            yield return new WaitForSeconds(.2f);

            for (int x = 0; x < Width; x++)
            {
                ShiftColumnDown(x);
            }

            StartCoroutine(FillBoardCo());
        }

        private void ShiftColumnDown(int columnIndex)
        {
            int nullCounter = 0;

            for (int y = 0; y < Height; y++)
            {
                if (AllGems[columnIndex, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    MoveGemDown(columnIndex, y, nullCounter);
                }
            }
        }

        private void MoveGemDown(int x, int y, int nullCounter)
        {
            AllGems[x, y].PosIndex.y -= nullCounter;
            AllGems[x, y - nullCounter] = AllGems[x, y];
            AllGems[x, y] = null;
        }


        private IEnumerator FillBoardCo()
        {
            yield return new WaitForSeconds(.5f);
            RefillBoard();
            yield return new WaitForSeconds(.5f);

            MatchFinder.FindAllMatches();

            if (HasMatches())
            {
                yield return HandleMatches();
            }
            else
            {
                yield return HandleNoMatches();
            }
        }

        private bool HasMatches()
        {
            return MatchFinder.currentMatches.Count > 0;
        }

        private IEnumerator HandleMatches()
        {
            yield return new WaitForSeconds(1);
            DestroyMatches();
        }

        private IEnumerator HandleNoMatches()
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.Move;
            InvokeEvent(DBEventNames.Match3ReturnButton, true);

            if (IsEndOfRound())
            {
                roundMan.CheckGameState();
            }

            if (IsScoreGoalReached())
            {
                roundMan.CheckGameState();
            }
        }

        private bool IsEndOfRound()
        {
            return MatchFinder.currentMatches.Count == 0 && roundMan.MovesCount == 0;
        }

        private bool IsScoreGoalReached()
        {
            return roundMan.Match3Score >= roundMan.Match3ScoreGoal;
        }


        private void RefillBoard() => FillBoard();

        private void CheckMisplacedGems()
        {
            List<Gem> foundGems = GetAllSceneGems();
            RemoveBoardGemsFromList(foundGems);
            DestroyGemsInList(foundGems);
        }

        private List<Gem> GetAllSceneGems()
        {
            return new List<Gem>(FindObjectsOfType<Gem>());
        }

        private void RemoveBoardGemsFromList(List<Gem> gemList)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    gemList.Remove(AllGems[x, y]);
                }
            }
        }

        private void DestroyGemsInList(List<Gem> gemList)
        {
            foreach (var gem in gemList)
            {
                Destroy(gem.gameObject);
            }
        }


        private void ScoreCheck(Gem gemToCheck)
        {
            roundMan.Match3Score += gemToCheck.ScoreValue;
            InvokeEvent(DBEventNames.Match3ScoreTextIncrease, roundMan.Match3Score);

            double reward = 12345678910;
            if (roundMan.Match3Score >= roundMan.Match3ScoreGoal)
            {
                InvokeEvent(DBEventNames.AddCurrencyUpdate, reward);
            }
        }

        public void RestartBoardAfterWin()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Gem gem = GetGemAtPosition(new Vector2Int(x, y));
                    if (gem != null)
                    {
                        Destroy(gem.gameObject);
                        AllGems[x, y] = null;
                    }
                }
            }

            InitializeBoard();

            StartCoroutine(FillBoardCo());
            currentState = BoardState.Move;
        }
    }
}