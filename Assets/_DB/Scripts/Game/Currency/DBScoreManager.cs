using System.Collections.Generic;
using DB_Core;

namespace DB_Game
{
    public class DBScoreManager
    {
        public DBPlayerScoreData PlayerScoreData = new();

        public DBScoreManager()
        {
            DBManager.Instance.SaveManager.Load<DBPlayerScoreData>(delegate (DBPlayerScoreData data)
            {
                PlayerScoreData = data ?? new DBPlayerScoreData();
            });
        }

        public bool TryGetScoreByTag(ScoreTags tag, ref int scoreOut)
        {
            if (PlayerScoreData.ScoreByTag.TryGetValue(tag, out var score))
            {
                scoreOut = score;
                return true;
            }

            return false;
        }

        public void SetScoreByTag(ScoreTags tag, int amount = 0)
        {
            DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnCurrencySet, (tag, amount));
            PlayerScoreData.ScoreByTag[tag] = amount;

            DBManager.Instance.SaveManager.Save(PlayerScoreData);
        }

        public void ChangeScoreByTagByAmount(ScoreTags tag, int amount = 0)
        {
            if (PlayerScoreData.ScoreByTag.ContainsKey(tag))
            {
                SetScoreByTag(tag, PlayerScoreData.ScoreByTag[tag] + amount);
            }
            else
            {
                SetScoreByTag(tag, amount);
            }
        }

        public bool TryUseScore(ScoreTags scoreTag, int amountToReduce)
        {
            var score = 0;
            var hasType = TryGetScoreByTag(scoreTag, ref score);
            var hasEnough = false;

            if (hasType)
            {
                hasEnough = amountToReduce <= score;
            }

            if (hasEnough)
            {
                ChangeScoreByTagByAmount(scoreTag, -amountToReduce);
            }

            else
            {
                DBManager.Instance.CrashManager.LogBreadcrumb($"User Doesn't have enough coins of type {scoreTag.ToString()}");
            }

            return hasEnough;
        }
    }

    public class DBPlayerScoreData : IDBSaveData
    {
        public Dictionary<ScoreTags, int> ScoreByTag = new();     
    }

    public enum ScoreTags
    {
        GameCurrency,
        PremiumCurrency
    }
}