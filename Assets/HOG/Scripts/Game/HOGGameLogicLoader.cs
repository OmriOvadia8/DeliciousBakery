using System;
using Core;
using UnityEngine;

namespace Game
{
    public class HOGGameLogicLoader : HOGGameLoaderBase
    {
        public override void StartLoad(Action onComplete)
        {
            var hogGameLogic = new HOGGameLogic();
            hogGameLogic.LoadManager(() =>
            {
                base.StartLoad(onComplete);
            });

        }
    }
}