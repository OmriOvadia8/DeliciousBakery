using UnityEngine.UI;
using UnityEngine;

namespace DB_Game
{
    public class Device : MonoBehaviour
    {
        [SerializeField] Skin skin; 
        [SerializeField] Image deviceImage;
        [SerializeField] int deviceIndex;
        [SerializeField] DBDeviceSkinDataManager skinStatusManager;

        public void ChangeSkin(int spriteIndex)
        {
            if (skin != null && deviceImage != null && spriteIndex >= 0 && spriteIndex < skin.sprites.Length)
            {
                deviceImage.sprite = skin.sprites[spriteIndex];
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Equipped = spriteIndex;
                skinStatusManager.SaveSkinsUnlockData();
            }
        }
    }
}