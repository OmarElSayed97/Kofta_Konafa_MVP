using System;
using System.Collections;
using System.Collections.Generic;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;
using UnityEngine;

public class IngredientLoader : MonoBehaviour
{
    #region Singleton

    private static IngredientLoader _instance;

    public static IngredientLoader Instance { get { return _instance; } }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion

    [SerializeField] private GameObject[] prefabList;
    [SerializeField] private IngredientSO[] ingredientSOsList;
    public Dictionary<int, GameObject> ingredientsPrefabs = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> preparedIngredientsPrefabs = new Dictionary<int, GameObject>();
    public Dictionary<GameEnums.Ingredient, IngredientSO> IngredientSos = new Dictionary<GameEnums.Ingredient, IngredientSO>();
    


    private void Start()
    {
        for (int i = 0; i < 13; i++)
        {
            ingredientsPrefabs.Add(i,prefabList[i]);
        }
        preparedIngredientsPrefabs.Add(0,prefabList[13]);
        preparedIngredientsPrefabs.Add(1,prefabList[14]);
        preparedIngredientsPrefabs.Add(2,prefabList[15]);
        preparedIngredientsPrefabs.Add(3,prefabList[16]);
        preparedIngredientsPrefabs.Add(5,prefabList[17]);
        preparedIngredientsPrefabs.Add(7,prefabList[18]);
        preparedIngredientsPrefabs.Add(8,prefabList[19]);
        preparedIngredientsPrefabs.Add(10,prefabList[21]);
        preparedIngredientsPrefabs.Add(11,prefabList[20]);
        
        IngredientSos.Add(GameEnums.Ingredient.Potato,ingredientSOsList[0]);
        IngredientSos.Add(GameEnums.Ingredient.Tomato,ingredientSOsList[1]);
        IngredientSos.Add(GameEnums.Ingredient.Onion,ingredientSOsList[2]);
        IngredientSos.Add(GameEnums.Ingredient.Cucumber,ingredientSOsList[3]);
        IngredientSos.Add(GameEnums.Ingredient.Cheese,ingredientSOsList[4]);
        IngredientSos.Add(GameEnums.Ingredient.Egg,ingredientSOsList[5]);
        IngredientSos.Add(GameEnums.Ingredient.Bun,ingredientSOsList[6]);
        IngredientSos.Add(GameEnums.Ingredient.BurgerPatty,ingredientSOsList[7]);
        IngredientSos.Add(GameEnums.Ingredient.ChickenPatty,ingredientSOsList[8]);
        IngredientSos.Add(GameEnums.Ingredient.Chicken,ingredientSOsList[9]);
        IngredientSos.Add(GameEnums.Ingredient.Meat,ingredientSOsList[10]);
        IngredientSos.Add(GameEnums.Ingredient.Pancake,ingredientSOsList[11]);
        IngredientSos.Add(GameEnums.Ingredient.Syrup,ingredientSOsList[12]);
        
        
    }
}
