using UnityEngine;

namespace DB_Game
{
    [CreateAssetMenu(fileName = "AudioAsset", menuName = "Audio/New Audio Asset", order = 1)]
    public class AudioAsset : ScriptableObject
    {
        public SoundEffectType effectType;
        public AudioClip clip;
        public float volume = 1f;
    }
}