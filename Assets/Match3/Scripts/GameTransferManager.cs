using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTransferManager : MonoBehaviour
{
    [SerializeField] GameObject MiniGameScene;
    [SerializeField] GameObject MainGameScene;

    public void GamesTransfer(bool value)
    {
        MainGameScene.SetActive(value);
        MiniGameScene.SetActive(!value);
    }

}
