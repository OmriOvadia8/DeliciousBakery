using UnityEngine;

namespace DB_Game
{
    [CreateAssetMenu(fileName = "New Skin", menuName = "Skins/Skin")]
    public class Skin : ScriptableObject
    {
        public Sprite[] sprites;
    }
}