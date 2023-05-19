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

        public bool TryGetScoreByTag(ScoreTags tag, ref double scoreOut)
        {
            if (PlayerScoreData.ScoreByTag.TryGetValue(tag, out var score))
            {
                scoreOut = score;
                return true;
            }

            return false;
        }

        public void SetScoreByTag(ScoreTags tag, double amount = 0)
        {
            DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnCurrencySet, (tag, amount));
            PlayerScoreData.ScoreByTag[tag] = amount;

            DBManager.Instance.SaveManager.Save(PlayerScoreData);
        }

        public void ChangeScoreByTagByAmount(ScoreTags tag, double amount = 0)
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

        public bool TryUseScore(ScoreTags scoreTag, double amountToReduce)
        {
            var score = 0D;
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
        public Dictionary<ScoreTags, double> ScoreByTag = new();
    }

    public enum ScoreTags
    {
        GameCurrency,
        PremiumCurrency
    }
}
