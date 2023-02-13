using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Game
{
    public class UIManager : MonoBehaviour
    {
        private HOGScoreManager Score => HOGGameLogic.Instance.ScoreManager;
        private HOGMoneyHolder Money => HOGGameLogic.Instance.PlayerMoney;

        [SerializeField] TMP_Text moneyText;

        private void Update()
        {        
            if (Score.TryGetScoreByTag(ScoreTags.GameCurrency, ref Money.startingCurrency))
            {
                moneyText.text = Money.startingCurrency.ToString();
            }
        }
    }
}