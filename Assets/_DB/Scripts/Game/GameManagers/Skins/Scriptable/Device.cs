using UnityEngine.UI;
using UnityEngine;

namespace DB_Game
{
    public class Device : MonoBehaviour
    {
        [SerializeField] Skin skin; // the skin for this device
        [SerializeField] Image deviceImage; // the sprite renderer for this device

        private void Start()
        {
            
        }

        public void ChangeSkin(int spriteIndex)
        {
            if (skin != null && deviceImage != null && spriteIndex >= 0 && spriteIndex < skin.sprites.Length)
            {
                deviceImage.sprite = skin.sprites[spriteIndex];
            }
        }
    }
}