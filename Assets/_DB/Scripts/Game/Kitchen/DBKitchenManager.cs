using DB_Core;
using UnityEngine;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBKitchenManager : DBLogicMonoBehaviour
    {
        [Header("Floor")]
        [SerializeField] GameObject floor1;
        [SerializeField] GameObject floor2;
        [SerializeField] GameObject floor3;

        [Header("Stove")]
        [SerializeField] GameObject stove1;
        [SerializeField] GameObject stove2;
        [SerializeField] GameObject stove3;

        [Header("Blender")]
        [SerializeField] GameObject blender1;
        [SerializeField] GameObject blender2;
        [SerializeField] GameObject blender3;

        [Header("Mixer")]
        [SerializeField] GameObject mixer1;
        [SerializeField] GameObject mixer2;
        [SerializeField] GameObject mixer3;

        [Header("Coffee Machine")]
        [SerializeField] GameObject coffeeMachine1;
        [SerializeField] GameObject coffeeMachine2;
        [SerializeField] GameObject coffeeMachine3;

        [Header("Cocoa Machine")]
        [SerializeField] GameObject cocoaMachine1;
        [SerializeField] GameObject cocoaMachine2;
        [SerializeField] GameObject cocoaMachine3;

        [Header("Ice Cream Stand")]
        [SerializeField] GameObject iceCreamStand1;
        [SerializeField] GameObject iceCreamStand2;
        [SerializeField] GameObject iceCreamStand3;

        [Header("First Ice Cream Taste")]
        [SerializeField] GameObject firstIceCreamTaste1;
        [SerializeField] GameObject firstIceCreamTaste2;
        [SerializeField] GameObject firstIceCreamTaste3;

        [Header("Second Ice Cream Taste")]
        [SerializeField] GameObject secondIceCreamTaste1;
        [SerializeField] GameObject secondIceCreamTaste2;
        [SerializeField] GameObject secondIceCreamTaste3;

        [Header("Third Ice Cream Taste")]
        [SerializeField] GameObject thirdIceCreamTaste1;
        [SerializeField] GameObject thirdIceCreamTaste2;
        [SerializeField] GameObject thirdIceCreamTaste3;
    }
}