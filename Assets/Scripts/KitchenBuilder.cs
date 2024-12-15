using System;
using System.Collections;
using System.Collections.Generic;
using KoftaAndKonafa;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;


public class KitchenBuilder : MonoBehaviour
{
    [SerializeField] private List<SpawningPoint> spawningPoints;
    [SerializeField] private List<StationSO> stationPool;
    // Dictionaries to track provided ingredients and stations
    private Dictionary<GameEnums.StationType, int> stationAvailability;
    private Dictionary<GameEnums.Ingredient, int> ingredientAvailability;
    private KitchenManager _kitchenManager;
   

    private void Awake()
    {
        foreach (var spawnPoint in spawningPoints)
        {
            foreach (Transform child in spawnPoint.spawningPointPlaceholder.transform) 
                Destroy(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _kitchenManager = KitchenManager.Instance;
        InitializeStationDictionary();
        InitializeIngredientDictionary();
        InitStations();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [System.Serializable]
    public struct SpawningPoint
    {
        public GameEnums.PlacementPoint placement;
        public GameObject spawningPointPlaceholder;
        public bool isEmpty;
    }

    void DummyStationAssigning()
    {
        for (int i = 0; i < spawningPoints.Count; i++)
        {
            GameObject temp = Instantiate(stationPool[Random.Range(0, 5)].stationPrefab, spawningPoints[i].spawningPointPlaceholder.transform);
            temp.transform.position = spawningPoints[i].spawningPointPlaceholder.transform.position;
        }
    }
    
    /// <summary>
    /// Initializes the ingredient availability dictionary.
    /// </summary>
    private void InitializeIngredientDictionary()
    {
        ingredientAvailability = new Dictionary<GameEnums.Ingredient, int>();
        foreach (GameEnums.Ingredient ingredient in Enum.GetValues(typeof(GameEnums.Ingredient)))
        {
            ingredientAvailability.Add(ingredient, 0);
        }
    }
    
    /// <summary>
    /// Initializes the ingredient availability dictionary.
    /// </summary>
    private void InitializeStationDictionary()
    {
        stationAvailability = new Dictionary<GameEnums.StationType, int>();
        foreach (GameEnums.StationType station in Enum.GetValues(typeof(GameEnums.StationType)))
        {
            stationAvailability.Add(station, 0);
        }
    }

    private void InitStations()
    {
        foreach (MealSO meal in _kitchenManager.playerOneMeals)
        {
            foreach (MealSO.RequiredIngredient requiredIngredient in meal.requiredIngredients)
            {
                InitResourceStation(requiredIngredient.ingredient);
                InitPrepStation(requiredIngredient.finalState);

                if (meal.needsCooking)
                {
                    InitPrepStation(GameEnums.IngredientState.Cooked);
                }
            }
        }
        InitAssemblingStations();
        
    }
    
    /// <summary>
    /// Checks if an ingredient is available.
    /// </summary>
    /// <param name="ingredient">Ingredient to check.</param>
    /// <returns>True if the ingredient is available, otherwise false.</returns>
    public bool CheckIngredientAvailability(GameEnums.Ingredient ingredient)
    {
        return ingredientAvailability.ContainsKey(ingredient) && ingredientAvailability[ingredient] > 0;
    }

    
    /// <summary>
    /// Checks if a station is available based on the required ingredient state.
    /// </summary>
    /// <param name="state">Ingredient state to check.</param>
    /// <returns>True if the corresponding station is available, otherwise false.</returns>
    public bool CheckStationAvailability(GameEnums.IngredientState state)
    {
        GameEnums.StationType stationType = MapStateToStation(state);
        return stationAvailability.ContainsKey(stationType) && stationAvailability[stationType] > 0;
    }


    void InitResourceStation(IngredientSO ingredient)
    {
        bool isIngredientAlreadyAvailable = CheckIngredientAvailability(ingredient.ingredientName);
        if (!isIngredientAlreadyAvailable)
        {
            int spawnIndex = GetRandomSpawnPoint();
            int stationIndex = GetStationIndexInStationPool(GameEnums.IngredientState.Default);
            GameObject tempStation = Instantiate(stationPool[0].stationPrefab, spawningPoints[spawnIndex].spawningPointPlaceholder.transform); // 0 refers to ResourceStation
            tempStation.transform.position = spawningPoints[spawnIndex].spawningPointPlaceholder.transform.position;
            StationManager station = tempStation.GetComponent<StationManager>();
            Instantiate(ingredient.ingredientPrefab, station.stationItemPlaceholder.transform);
            ingredient.currentState = GameEnums.IngredientState.Default;
            station.currentIngredient = new Ingredient(ingredient);
            station.stationID = spawnIndex;

            //Update Availability Dictionaries
            ingredientAvailability[ingredient.ingredientName]++;
            stationAvailability[GameEnums.StationType.ResourceStation]++;
            var spawningPoint = spawningPoints[spawnIndex];
            spawningPoint.isEmpty = false;
            spawningPoints[spawnIndex] = spawningPoint;
        }
        
        
    }

    void InitPrepStation(GameEnums.IngredientState ingredientState)
    {
        bool isStationAlreadyAvailable = CheckStationAvailability(ingredientState);
        if (!isStationAlreadyAvailable)
        {
            int spawnIndex = GetRandomSpawnPoint();
            int stationIndex = GetStationIndexInStationPool(ingredientState);
            GameObject tempStation = Instantiate(stationPool[stationIndex].stationPrefab, spawningPoints[spawnIndex].spawningPointPlaceholder.transform); // 0 refers to ResourceStation
            tempStation.transform.position = spawningPoints[spawnIndex].spawningPointPlaceholder.transform.position;
            StationManager station = tempStation.GetComponent<StationManager>();
            station.stationID = spawnIndex;
            station.currentIngredient = null;
            Debug.Log("CURRENT STATION: " + station.stationSO.stationType + " --- " + stationPool[stationIndex].stationPrefab.name);
            //Update Availability Dictionaries
            stationAvailability[MapStateToStation(ingredientState)]++;
            var spawningPoint = spawningPoints[spawnIndex];
            spawningPoint.isEmpty = false;
            spawningPoints[spawnIndex] = spawningPoint;
        }
    }


    void InitAssemblingStations()
    {
        for (int i = 0; i < 3; i++)
        {
            int spawnIndex = GetRandomSpawnPoint();
            GameObject tempStation = Instantiate(stationPool[3].stationPrefab, spawningPoints[spawnIndex].spawningPointPlaceholder.transform); // 0 refers to ResourceStation
            tempStation.transform.position = spawningPoints[spawnIndex].spawningPointPlaceholder.transform.position;
            StationManager station = tempStation.GetComponent<StationManager>();
            station.stationID = spawnIndex;
            station.currentIngredient = null;
            //Update Availability Dictionaries
            var spawningPoint = spawningPoints[spawnIndex];
            spawningPoint.isEmpty = false;
            spawningPoints[spawnIndex] = spawningPoint;
        }
    }
    /// <summary>
    /// Maps the ingredient state to the corresponding station type.
    /// </summary>
    /// <param name="state">The ingredient state to map.</param>
    /// <returns>The corresponding station type.</returns>
    private GameEnums.StationType MapStateToStation(GameEnums.IngredientState state)
    {
        switch (state)
        {
            case GameEnums.IngredientState.Chopped:
                return GameEnums.StationType.ChoppingStation;
            case GameEnums.IngredientState.Heated:
                return GameEnums.StationType.HeatStation;
            case GameEnums.IngredientState.Cooked:
                return GameEnums.StationType.CookStation;
            case GameEnums.IngredientState.Default:
                return GameEnums.StationType.ResourceStation;
            default:
                Debug.LogError($"Unhandled IngredientState: {state}");
                return GameEnums.StationType.ResourceStation;
        }
    }

    private int GetRandomSpawnPoint()
    {
        int spawnPointIndex = 0;
        bool pointFound = false;
        
        while (!pointFound)
        {
            spawnPointIndex = Random.Range(0, spawningPoints.Count);
            bool isEmptySpawnPoint = spawningPoints[spawnPointIndex].isEmpty;
            if (isEmptySpawnPoint)
                pointFound = true;
        }
        return spawnPointIndex;
    }


    private int GetStationIndexInStationPool(GameEnums.IngredientState state)
    {
        GameEnums.StationType stationType = MapStateToStation(state);
        switch (stationType)
        {
            case GameEnums.StationType.ResourceStation:
                return 0;
            case GameEnums.StationType.ChoppingStation:
                return 1;
            case GameEnums.StationType.HeatStation:
                return 2;
            case GameEnums.StationType.CookStation:
                return 4;
            default:
                return 0;
        }
    }
    
    
    

    
}
