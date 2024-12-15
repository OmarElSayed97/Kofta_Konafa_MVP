using UnityEngine;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    [System.Serializable]
    public class Ingredient
    {
        [Header("Ingredient Details")]
        public GameEnums.Ingredient ingredientName;
        public int ingredientID;
        public GameObject ingredientPrefab;

        [Header("State Info")]
        public GameEnums.IngredientState currentState;

        [Header("Processing Times")]
        public float prepTime;

        /// <summary>
        /// Initializes a new Ingredient based on an IngredientSO.
        /// </summary>
        /// <param name="ingredientSO">The source IngredientSO.</param>
        public Ingredient(IngredientSO ingredientSO)
        {
            ingredientName = ingredientSO.ingredientName;
            ingredientID = ingredientSO.ingredientID;
            ingredientPrefab = ingredientSO.ingredientPrefab;
            currentState = ingredientSO.currentState;
            prepTime = ingredientSO.prepTime;
            
        }
        
        
    }
}