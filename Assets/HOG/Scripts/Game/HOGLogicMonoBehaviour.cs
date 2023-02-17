using Core;

namespace Game
{
    public class HOGLogicMonoBehaviour : HOGMonoBehaviour

    {
        public HOGGameLogic GameLogic => HOGGameLogic.Instance;
    }
}