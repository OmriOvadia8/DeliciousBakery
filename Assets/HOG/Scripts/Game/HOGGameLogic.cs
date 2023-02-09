using System;
using Core;

namespace Game
{
    public class HOGGameLogic : IHOGBaseManager
    {
        public static HOGGameLogic Instance;

        public HOGScoreManager ScoreManager;
        public HOGUpgradeManager UpgradeManager;

        public HOGGameLogic()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            ScoreManager = new HOGScoreManager();
            UpgradeManager = new HOGUpgradeManager();

            onComplete.Invoke();
        }
    }
}