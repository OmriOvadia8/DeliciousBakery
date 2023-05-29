using System;
using DB_Core;

namespace DB_Game
{
    public class DBGameLogicLoader : DBGameLoaderBase
    {
        public override void StartLoad(Action onComplete)
        {
            var dbGameLogic = new DBGameLogic();
            dbGameLogic.LoadManager(() => base.StartLoad(onComplete));
        }
    }
}