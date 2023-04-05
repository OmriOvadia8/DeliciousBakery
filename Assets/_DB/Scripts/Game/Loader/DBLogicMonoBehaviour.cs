using DB_Core;

namespace DB_Game
{
    public class DBLogicMonoBehaviour : DBMonoBehaviour

    {
        public DBGameLogic GameLogic => DBGameLogic.Instance;
    }
}