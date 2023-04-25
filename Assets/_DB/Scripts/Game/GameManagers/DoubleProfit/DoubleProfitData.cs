using System;
using DB_Core;

namespace DB_Game
{
    [Serializable]
    public class DoubleProfitData : IDBSaveData
    {
        public bool IsDoubleProfitOn;
        public int DoubleProfitMultiplier;
        public int DoubleProfitDuration;
        public int CurrentDoubleProfitDuration;
        public DateTime LastSavedTime;

        public DoubleProfitData(bool isDouble, int doubleAmount, int doubleProfitDuration, int currentProfitDuration)
        {
            IsDoubleProfitOn = isDouble;
            DoubleProfitMultiplier = doubleAmount;
            DoubleProfitDuration = doubleProfitDuration;
            CurrentDoubleProfitDuration = currentProfitDuration;
            LastSavedTime = DateTime.Now;
        }
        public void TurnOnDoubleProfit()
        {
            IsDoubleProfitOn = true;
            DoubleProfitMultiplier = 2;
        }

        public void TurnOffDoubleProfit()
        {
            IsDoubleProfitOn = false;
            DoubleProfitMultiplier = 1;
            DoubleProfitDuration = 300;
        }
    }
}