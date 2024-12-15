using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    public class TestStaton : StationManager
    {
        private List<MealAssemblyData> stationMealData;
        [HideInInspector]public  MealAssemblyData currentMeal; // Tracks the active meal being assembled
        [HideInInspector]public  MealAssemblyData bestMeal; // Tracks the active meal being assembled
        //[HideInInspector]public bool IsCurrentMealReady => currentMeal != null && IsMealComplete(currentMeal);
        
        
        private void Awake()
        {
            InitializeMealData();
            currentMeal = null;
        }
        
        public MealAssemblyData GetBestMatchingMeal(Ingredient ingredient)
        {
            if (currentMeal != null && IsMealLocked(currentMeal))
            {
                return currentMeal; // Lock to current meal if active
            }

            return stationMealData
                .Where(m => m.mealSO.requiredIngredients.Any(i => i.ingredient.ingredientName == ingredient.ingredientName && i.finalState == ingredient.currentState))
                .OrderByDescending(m => CountMatchingIngredients(m))
                .FirstOrDefault();
        }

        private void InitializeMealData()
        {
            stationMealData = KitchenManager.Instance.playerOneMealData
                .Select(meal => new MealAssemblyData
                {
                    mealSO = meal.mealSO,
                    mealPrefabInstance = Instantiate(meal.mealPrefabInstance, stationItemPlaceholder.transform),
                    activatedIngredients = new HashSet<GameEnums.Ingredient>()
                })
                .ToList();
        }
        
        
        private void ResetMeal(MealAssemblyData mealData)
        {
            mealData.mealPrefabInstance.SetActive(false);
            ResetMealChildren(mealData);
            mealData.activatedIngredients.Clear();
            DeactivatePlate(mealData);
            currentMeal = null;
        }
        
        
        
        
        private void ResetMealChildren(MealAssemblyData mealData)
        {
            var mealIngredientMapping = mealData.mealPrefabInstance.GetComponent<MealIngredientMapping>();
            foreach (var ingredient in mealIngredientMapping.ingredientMappings)
            {
                ingredient.ingredientObject.SetActive(false);
            }
        }
        
        private void DeactivatePlate(MealAssemblyData mealData)
        {
            Transform mealHolder = mealData.mealPrefabInstance.transform.Find("Meal_Holder");
            if (mealHolder is not null && mealHolder.childCount > 0)
            {
                mealHolder.GetChild(0).gameObject.SetActive(false);
            }
        }
        
        private int CountMatchingIngredients(MealAssemblyData mealData)
        {
            return mealData.mealSO.requiredIngredients
                .Count(i => mealData.activatedIngredients.Contains(i.ingredient.ingredientName));
        }
        
        private bool IsMealLocked(MealAssemblyData meal)
        {
            if (meal == null || meal.activatedIngredients == null || !meal.activatedIngredients.Any()) 
                return false;

            var uniqueMeals = stationMealData
                .Where(m => meal.activatedIngredients.All(
                    activated => m.mealSO.requiredIngredients.Any(
                        req => req.ingredient.ingredientName == activated)))
                .ToList();

            return uniqueMeals.Count == 1;
        }
        
        [System.Serializable]
        public class MealAssemblyData
        {
            public MealSO mealSO;
            public GameObject mealPrefabInstance;
            public HashSet<GameEnums.Ingredient> activatedIngredients = new HashSet<GameEnums.Ingredient>();
        }
    }
    
    
    
}
