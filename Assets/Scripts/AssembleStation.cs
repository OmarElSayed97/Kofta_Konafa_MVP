using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    public class AssembleStation : StationManager
    {
        [SerializeField]private List<MealAssemblyData> stationMealData;
        [SerializeField]public  MealAssemblyData currentMeal; // Tracks the active meal being assembled
        [HideInInspector]public  MealAssemblyData bestMeal; // Tracks the active meal being assembled
        [HideInInspector]public bool IsCurrentMealReady;
        private bool switchingWillOccur;
        private MealAssemblyData tempMeal;

        private void Awake()
        {
            InitializeMealData();
            currentMeal = null;
        }
        
        

        /// <summary>
        /// Initializes meal data by creating independent meal instances.
        /// </summary>
        private void InitializeMealData()
        {
            stationMealData = KitchenManager.Instance.playerOneMealData
                .Select(meal => new MealAssemblyData
                {
                    mealSO = meal.mealSO,
                    mealPrefabInstance = Instantiate(meal.mealPrefabInstance, stationItemPlaceholder.transform),
                    activatedIngredients = new List<GameEnums.Ingredient>()
                })
                .ToList();
        }

        /// <summary>
        /// Handles meal pickup by the character.
        /// </summary>
        /// <returns>The prepared meal GameObject.</returns>
        public GameObject PickupMeal()
        {
            // if (currentMeal == null || !IsCurrentMealReady)
            // {
            //     Debug.Log("Nothing To Pick Up");
            //     return null;
            // }

            GameObject mealPrefab = currentMeal.mealPrefabInstance;
            ResetMeal(currentMeal);
            return mealPrefab;
        }

        /// <summary>
        /// Resets a meal after pickup.
        /// </summary>
        private void ResetMeal(MealAssemblyData mealData)
        {
            mealData.mealPrefabInstance.SetActive(false);
            ResetMealChildren(mealData);
            mealData.activatedIngredients.Clear();
            DeactivatePlate(mealData);
            currentMeal = null;
        }


        /// <summary>
        /// Checks if the ingredient matches any meal without changing state.
        /// </summary>
        /// <param name="ingredient">The ingredient to check.</param>
        /// <returns>The best matching meal or null if no match found.</returns>
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

        /// <summary>
        /// Determines whether a meal should remain locked.
        /// </summary>
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

        /// <summary>
        /// Attempts to assemble the ingredient into the correct meal.
        /// </summary>
        /// <param name="ingredient">The ingredient to assemble.</param>
        /// <returns>True if the ingredient was successfully assembled.</returns>
        public bool TryAssembleIngredient(Ingredient ingredient)
        {
            // Check if we need to switch meals
            if (currentMeal == null || !IsMealLocked(currentMeal))
            {
                bestMeal = GetBestMatchingMeal(ingredient);
                if (bestMeal == null) return false;

                if (currentMeal != null)
                {
                    ResetAllMealsExceptCurrent();  // Reset other meals
                }
                currentMeal = bestMeal;
                ActivateMealPrefab(currentMeal);
                ActivatePlate(currentMeal);
            }
            // Avoid duplicate ingredients
            if (currentMeal.activatedIngredients.Contains(ingredient.ingredientName))
            {
                Debug.LogWarning($"{ingredient.ingredientName} already added to {currentMeal.mealSO.name}.");
                return false;
            }
            // Add the new ingredient and check completion
            ActivateIngredientInMeal(currentMeal, ingredient);
            if (IsMealComplete(currentMeal))
            {
                Debug.Log($"{currentMeal.mealSO.name} is fully prepared!");
                //currentMeal = null; // Reset the current meal lock
            }
           
            return true;
        }

        /// <summary>
        /// Resets all meals except the currently active one.
        /// </summary>
        private void ResetAllMealsExceptCurrent()
        {
            MealAssemblyData mealTemp;
            foreach (MealAssemblyData meal in stationMealData)
            {
                // Debug.Log("ENTERED METHOD");
                if (meal == bestMeal) continue;
                // Deactivate old meal completely
                meal.mealPrefabInstance.SetActive(false);
                ResetMealChildren(meal);
                DeactivatePlate(meal);
                // Reactivate only previously activated ingredients that are needed in the new meal
                foreach (GameEnums.Ingredient ingredient in meal.activatedIngredients.ToList())
                {
                    if (bestMeal.mealSO.requiredIngredients.Any(req => req.ingredient.ingredientName == ingredient))
                    {
                        var mealIngredientMapping = bestMeal.mealPrefabInstance.GetComponent<MealIngredientMapping>();
                        var ingredientObject = mealIngredientMapping.GetIngredientObject(
                            ingredient, 
                            bestMeal.mealSO.requiredIngredients
                                .First(req => req.ingredient.ingredientName == ingredient)
                                .finalState);
                        if (ingredientObject is not null)
                        {
                            Debug.Log("INGREDIENT: " + ingredientObject.name);
                            ingredientObject.SetActive(true);
                            bestMeal.activatedIngredients.Add(ingredient);

                            Debug.Log($"Reactivating and adding {ingredient} to {bestMeal.mealSO.name}");
                        }
                        else
                        {
                            Debug.Log("Ingredient object turns out to be null");
                        }
                    }
                }
                
                meal.activatedIngredients.Clear();
                
            }
           
        }
       

        /// <summary>
        /// Deactivates all children under the meal prefab.
        /// </summary>
        private void ResetMealChildren(MealAssemblyData mealData)
        {
            var mealIngredientMapping = mealData.mealPrefabInstance.GetComponent<MealIngredientMapping>();
            foreach (var ingredient in mealIngredientMapping.ingredientMappings)
            {
                ingredient.ingredientObject.SetActive(false);
            }
          
            
        }

        /// <summary>
        /// Activates the meal prefab while deactivating others.
        /// </summary>
        private void ActivateMealPrefab(MealAssemblyData mealData)
        {
            mealData.mealPrefabInstance.SetActive(true);
        }

        /// <summary>
        //// <summary>
        /// Deactivates the plate inside the meal's Meal_Holder.
        /// </summary>
        private void DeactivatePlate(MealAssemblyData mealData)
        {
            Transform mealHolder = mealData.mealPrefabInstance.transform.Find("Meal_Holder");
            if (mealHolder is not null && mealHolder.childCount > 0)
            {
                mealHolder.GetChild(0).gameObject.SetActive(false);
            }
        }
        
        
        /// </summary>
        private void ActivatePlate(MealAssemblyData mealData)
        {
            Transform mealHolder = mealData.mealPrefabInstance.transform.Find("Meal_Holder");
            if (mealHolder is not null && mealHolder.childCount > 0)
            {
                mealHolder.GetChild(0).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Activates the correct ingredient within the meal.
        /// </summary>
        private void ActivateIngredientInMeal(MealAssemblyData mealData, Ingredient ingredient)
        {
            
            var mealIngredientMapping = mealData.mealPrefabInstance.GetComponent<MealIngredientMapping>();
            GameObject ingredientObject = mealIngredientMapping.GetIngredientObject(ingredient.ingredientName, ingredient.currentState);
            if (ingredientObject is not null && !mealData.activatedIngredients.Contains(ingredient.ingredientName))
            {
                mealData.activatedIngredients.Add(ingredient.ingredientName);
                ingredientObject.SetActive(true);
                Debug.Log($"{ingredient.ingredientName} added to {mealData.mealSO.name}");
                
            }
           
        }

        /// <summary>
        /// Checks if all required ingredients are assembled for the meal.
        /// </summary>
        private bool IsMealComplete(MealAssemblyData mealData)
        {
            bool isComplete = mealData.mealSO.requiredIngredients
                .All(i => mealData.activatedIngredients.Contains(i.ingredient.ingredientName));

            Debug.Log($"IsMealComplete: {isComplete}");
            Debug.Log($"Required: {string.Join(", ", mealData.mealSO.requiredIngredients.Select(i => i.ingredient.ingredientName))}");
            Debug.Log($"Activated: {string.Join(", ", mealData.activatedIngredients)}");
            if (isComplete)
                IsCurrentMealReady = true;
            return isComplete;
        }

        /// <summary>
        /// Counts matching ingredients in a meal for scoring purposes.
        /// </summary>
        private int CountMatchingIngredients(MealAssemblyData mealData)
        {
            return mealData.mealSO.requiredIngredients
                .Count(i => mealData.activatedIngredients.Contains(i.ingredient.ingredientName));
        }
    }

    [System.Serializable]
    public class MealAssemblyData
    {
        public MealSO mealSO;
        public GameObject mealPrefabInstance;
        public List<GameEnums.Ingredient> activatedIngredients = new List<GameEnums.Ingredient>();
    }
}
