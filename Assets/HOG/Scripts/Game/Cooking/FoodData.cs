using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Food Data")]
public class FoodData : ScriptableObject
{
    public string foodName;
    public int level;
    public int profit;
    public float cookingTime;
}
