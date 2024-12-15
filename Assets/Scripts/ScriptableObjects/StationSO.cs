using UnityEngine;
using KoftaAndKonafa.Enums;
using System.Collections.Generic;

namespace KoftaAndKonafa.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewStation", menuName = "KoftaAndKonafa/Station", order = 1)]
    public class StationSO : ScriptableObject
    {
        // The type of station
        public GameEnums.StationType stationType;

        public GameObject stationPrefab;

        
        // Serializable key-value pair for receivable items
        [System.Serializable]
        public struct ReceivableItem
        {
            public IngredientSO ingredient;
            public GameEnums.IngredientState state;
        }

        // List of receivable items
        public List<ReceivableItem> receivableItems = new List<ReceivableItem>();
    }
}