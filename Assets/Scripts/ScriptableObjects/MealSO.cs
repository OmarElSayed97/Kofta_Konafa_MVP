using UnityEngine;
using System.Collections.Generic;
using KoftaAndKonafa.Enums;

namespace KoftaAndKonafa.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewMeal", menuName = "KoftaAndKonafa/Meal", order = 2)]
    public class MealSO : ScriptableObject
    {
        // Indicates if the meal needs cooking
        public bool needsCooking;

        // Indicates if the meal is ready to be served
        public bool isReady;

        // Serializable key-value pair for required ingredients
        [System.Serializable]
        public struct RequiredIngredient
        {
            public IngredientSO ingredient;
            public GameEnums.IngredientState finalState;
            public bool isDelivered;
            
        }
        
        

        // List of required ingredients for the meal
        public List<RequiredIngredient> requiredIngredients = new List<RequiredIngredient>();


      
        /// <summary>
        /// Resets the 'isDelivered' property of all required ingredients to false.
        /// </summary>
        public void ResetDeliveredStatus()
        {
            for (int i = 0; i < requiredIngredients.Count; i++)
            {
                RequiredIngredient ingredient = requiredIngredients[i];
                ingredient.isDelivered = false;
                requiredIngredients[i] = ingredient;
            }
        }
    }
}