using DB_Core;
using System.Collections.Generic;
using UnityEngine;

namespace DB_Game
{
    public class DBSoundManager : DBLogicMonoBehaviour
    {
        [SerializeField] private List<AudioAsset> audioAssets;
        private Dictionary<SoundEffectType, AudioAsset> audioAssetDict;
        [SerializeField] AudioSource audioSource;

        private void Awake() => InitializeAudioDictionary();

        private void OnEnable() => AddListener(DBEventNames.PlaySound, PlaySound);

        private void OnDisable() => RemoveListener(DBEventNames.PlaySound, PlaySound);
  
        private void PlaySound(object sound)
        {
            if (sound is SoundEffectType effectType)
            {
                if (audioAssetDict.TryGetValue(effectType, out AudioAsset asset))
                {
                    audioSource.PlayOneShot(asset.clip, asset.volume);
                }
                else
                {
                    DBDebug.LogException($"No audio asset found for sound effect type: {effectType}");
                }
            }
            else
            {
                DBDebug.LogException($"Sound object is not of type SoundEffectType");
            }
        }

        private void InitializeAudioDictionary()
        {
            audioAssetDict = new Dictionary<SoundEffectType, AudioAsset>();

            foreach (var audioAsset in audioAssets)
            {
                if (!audioAssetDict.ContainsKey(audioAsset.effectType))
                {
                    audioAssetDict.Add(audioAsset.effectType, audioAsset);
                }
                else
                {
                    DBDebug.LogException($"Duplicate sound effect type detected: {audioAsset.effectType}");
                }
            }
        }
    }

    public enum SoundEffectType
    {
        ButtonClick,
        UpgradeButtonClick,
        LearnButtonClick,
        PingSound,
        HireSound,
        TabSound,
        PopupSound,
        Claim,
        Match3GemBreak,
        Match3Bomb
    }
}