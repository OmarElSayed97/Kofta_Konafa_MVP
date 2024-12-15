using System.Collections.Generic;
using UnityEngine;
using KoftaAndKonafa.Enums;

namespace KoftaAndKonafa
{
    public class MealIngredientMapping : MonoBehaviour
    {
        [System.Serializable]
        public class IngredientMapping
        {
            public GameEnums.Ingredient ingredientName;
            public GameEnums.IngredientState ingredientState;
            public GameObject ingredientObject;
        }

        [Header("Ingredient Mappings")]
        public List<IngredientMapping> ingredientMappings;

        [Header("Meal Completion")]
        public GameObject cookedMeal;

        /// <summary>
        /// Retrieves the corresponding ingredient GameObject based on name and state.
        /// </summary>
        /// <param name="ingredientName">The name of the ingredient.</param>
        /// <param name="ingredientState">The state of the ingredient.</param>
        /// <returns>The corresponding GameObject if found; otherwise, null.</returns>
        public GameObject GetIngredientObject(GameEnums.Ingredient ingredientName, GameEnums.IngredientState ingredientState)
        {
            foreach (var mapping in ingredientMappings)
            {
                if (mapping.ingredientName == ingredientName && mapping.ingredientState == ingredientState)
                {
                    return mapping.ingredientObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Activates the cooked meal GameObject when the meal is complete.
        /// </summary>
        public void ActivateCookedMeal()
        {
            if (cookedMeal != null)
            {
                cookedMeal.SetActive(true);
            }
        }
    }
}