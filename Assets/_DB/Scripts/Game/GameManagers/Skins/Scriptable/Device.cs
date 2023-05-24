using UnityEngine.UI;
using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class Device : DBLogicMonoBehaviour
    {
        [SerializeField] Skin skin; 
        [SerializeField] Image deviceImage;
        [SerializeField] int deviceIndex;
        [SerializeField] DBDeviceSkinDataManager skinStatusManager;
        [SerializeField] GameObject[] checkMark;

        public void ChangeSkin(int spriteIndex) => ChangeSkin(spriteIndex, true);

        public void ChangeSkin(int spriteIndex, bool playSound)
        {
            if (skin != null && deviceImage != null && spriteIndex >= 0 && spriteIndex < skin.sprites.Length)
            {
                deviceImage.sprite = skin.sprites[spriteIndex];
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Equipped = spriteIndex;
                if (playSound)
                {
                    InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
                }
                skinStatusManager.SaveSkinsUnlockData();

                UpdateCheckMarks(spriteIndex);
            }
        }

        private void UpdateCheckMarks(int activeIndex)
        {
            for (int i = 0; i < checkMark.Length; i++)
            {
                checkMark[i].SetActive(i == activeIndex);
            }
        }
    }
}