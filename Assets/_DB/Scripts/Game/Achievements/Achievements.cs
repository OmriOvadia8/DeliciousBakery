using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DB_Core;
using TMPro;

namespace DB_Game
{
    public class Achievements : DBLogicMonoBehaviour
    {
        public bool Completed { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public FoodData Food { get; set; }
        public int Reward { get; set; }
        public TMP_Text Text {get;set;}


        public Achievements(FoodData food, string type, int amount, int reward, TMP_Text text)
        {
            this.Completed = false;
            this.Type = type;
            this.Food = food;
            this.Amount = amount;
            this.Reward = reward;
            this.Text = text;
        }   

        
        
    }
    
}