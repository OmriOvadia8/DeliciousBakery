using UnityEngine;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBMuteManager : DBLogicMonoBehaviour
    {
        [SerializeField] Sprite muteSprite;      
        [SerializeField] Sprite unmuteSprite;     
        [SerializeField] Image buttonImage;

        public void ToggleMute()
        {
            AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            buttonImage.sprite = unmuteSprite;

            if (AudioListener.volume == 0)
            {
                buttonImage.sprite = muteSprite;
            }
        }
    }
}