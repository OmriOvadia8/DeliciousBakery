using System.Collections.Generic;
using Core;

namespace Game
{
    public class HOGScoreManager
    {
        public HOGPlayerScoreData PlayerScoreData = new();

        public HOGScoreManager()
        {
            HOGManager.Instance.SaveManager.Load<HOGPlayerScoreData>(delegate (HOGPlayerScoreData data)
            {
                PlayerScoreData = data ?? new HOGPlayerScoreData();
                //if (data == null)
                //{
                //    PlayerScoreData = new HOGPlayerScoreData();
                //}
                //else
                //{
                //    PlayerScoreData = data;
                //}
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
            HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OnCurrencySet, (tag, amount));
            PlayerScoreData.ScoreByTag[tag] = amount;

            HOGManager.Instance.SaveManager.Save(PlayerScoreData);
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

            return hasEnough;
        }
    }

    public class HOGPlayerScoreData : IHOGSaveData
    {
        public Dictionary<ScoreTags, int> ScoreByTag = new();     
    }

    public enum ScoreTags
    {
        GameCurrency,
        PremiumCurrency
    }
}