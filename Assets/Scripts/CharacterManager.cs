using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;
using Unity.VisualScripting;

namespace KoftaAndKonafa
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Character State")]
        public bool isEmptyHanded = true;
        public GameEnums.Item itemInHand = GameEnums.Item.Empty;

        [Header("Meal State")]
        public bool isHoldingMeal;
        public MealSO heldMeal;
        public bool isMealCooked;

        [Header("Interaction Settings")]
        public float interactionTime = 2.0f; // Time required to interact

        [Header("Hand Transform")]
        public Transform characterHand; 
        [Header("Meal Transform")]
        public Transform characterHead;
        [Header("UI Elements")]
        public Image interactionTimerImage; // UI Image for the interaction timer

        private float _interactionTimer;
        private bool _isInsideStation;
        private GameObject _currentStation;
        private StationSO _currentStationSO;
        private int _currentStationID;
        private AssembleStation _currentAssembleStation;
        private bool _isCurrentStationBusy;
        private Ingredient _currentStationIngredient;
        private Ingredient _currentIngredient;
        private IngredientSO _currentIngredientSO;
        private GameObject _prefabInHand;
        private IngredientLoader _ingredientLoader;
        private bool situation1, situation2,situation3,situation4,situation5;
        
        #region Singleton

        private static CharacterManager _instance;

        public static CharacterManager Instance { get { return _instance; } }
    
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

        private void Start()
        {
           _ingredientLoader = IngredientLoader.Instance;
           
        }

        private void Update()
        {
            AnalyzeSituation();
            // Update item in hand if empty-handed
            if (isEmptyHanded)
            {
                itemInHand = GameEnums.Item.Empty;
            }

            // Handle station interaction
            if (situation1 || situation2 || situation3|| situation4 || situation5)
            {
                _interactionTimer += Time.deltaTime;
                interactionTimerImage.fillAmount = _interactionTimer / interactionTime;
                interactionTimerImage.gameObject.SetActive(true);

                if (_interactionTimer >= interactionTime)
                {
                    InteractWithStation(_currentStation);
                    ResetInteraction();
                }
            }
            else
            {
                ResetInteraction();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("StationTag"))
            {
               
                _isInsideStation = true;
                _currentStation = other.gameObject;
                StationManager stationManager = _currentStation.GetComponent<StationManager>();
                _currentStationID = stationManager.stationID;
                _currentStationSO = stationManager.stationSO;
                if (_currentStationSO.stationType == GameEnums.StationType.AssembleStation)
                    _currentAssembleStation = _currentStation.GetComponent<AssembleStation>();
                _isCurrentStationBusy = stationManager.isActionInProgress;
                _currentStationIngredient = stationManager.currentIngredient;
                
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("StationTag"))
            {
                _isInsideStation = false;
                _currentStation = null;
                _currentStationSO = null;
                _currentAssembleStation = null;
                _isCurrentStationBusy = false;
                _currentStationIngredient = null;
                _currentStationID = 10000;
                ResetInteraction();
            }
        }

        public void StationAlertPlayer(int stationID, StationManager station)
        {
            if (_currentStationID == stationID)
            {
                ResetInteraction();
                _currentStation = station.gameObject;
                _currentStationSO = station.stationSO;
                _isCurrentStationBusy = station.isActionInProgress;
                _currentStationIngredient = station.currentIngredient;
            }
        }

        private void InteractWithStation(GameObject station)
        {
            
            StationManager stationManager;
            // if (stationManager is null)
            // {
            //     stationManager = station.GetComponent<AssembleStation>();
            // }
            //StationSO currentStation = stationManager.stationSO;
            StationSO currentStation;
            // Try to get the StationManager or AssembleStation component
           
            if (station.TryGetComponent<AssembleStation>(out var test2))
            {
                stationManager = station.GetComponent<AssembleStation>();
            }
            else
            {
                stationManager = station.GetComponent<StationManager>();
            }
            currentStation = stationManager.stationSO;
           
            if (currentStation.stationType == GameEnums.StationType.ResourceStation)
            {
                int ingredientAtStationID = stationManager.currentIngredient.ingredientID;
                _prefabInHand = Instantiate(_ingredientLoader.ingredientsPrefabs[ingredientAtStationID], characterHand);
                itemInHand = GameEnums.Item.Ingredient;
                _currentIngredient = stationManager.currentIngredient;
                isEmptyHanded = false;
            }
            else if (currentStation.stationType is GameEnums.StationType.ChoppingStation or GameEnums.StationType.HeatStation)
            {
                if (!isEmptyHanded && itemInHand == GameEnums.Item.Ingredient && _currentIngredient is not null &&
                    _currentIngredient.currentState == GameEnums.IngredientState.Default)
                {
                    stationManager.TakeItemFromPlayer(_ingredientLoader.IngredientSos[_currentIngredient.ingredientName],GameEnums.IngredientState.Default, _currentIngredient.prepTime);
                    ResetHand();
                }
                
                else if (isEmptyHanded  &&
                         _currentStationIngredient.currentState is GameEnums.IngredientState.Chopped or GameEnums.IngredientState.Heated)
                {
                    int ingredientAtStationID = stationManager.currentIngredient.ingredientID;
                    _prefabInHand = Instantiate(_ingredientLoader.preparedIngredientsPrefabs[ingredientAtStationID], characterHand);
                    itemInHand = GameEnums.Item.Ingredient;
                    _currentIngredient = stationManager.currentIngredient;
                    isEmptyHanded = false;
                    stationManager.ClearStation();
                    stationManager.ResetStation();
                }
                
            }
            
            else if (currentStation.stationType == GameEnums.StationType.AssembleStation &&
                     !_currentAssembleStation.IsCurrentMealReady)
            {
                stationManager = station.GetComponent<StationManager>() ?? station.GetComponent<AssembleStation>();
                currentStation = stationManager.stationSO;
                if (!isEmptyHanded && itemInHand == GameEnums.Item.Ingredient && _currentIngredient is not null)
                {
                    AssembleStation assembleStation = station.GetComponent<AssembleStation>();
                    MealAssemblyData meal = assembleStation.GetBestMatchingMeal(_currentIngredient);
                    if (meal is not null)
                    {
                        bool success = assembleStation.TryAssembleIngredient(_currentIngredient);
                        if (success)
                        {
                            Debug.Log("I ASSEMBLED");
                            ResetHand();
                        }
                        else
                        {
                            Debug.Log("I FAILED TO ASSEMBLE");
                        }
                    }
                }
            }
            else if (currentStation.stationType == GameEnums.StationType.AssembleStation && _currentAssembleStation.IsCurrentMealReady && isEmptyHanded)
            {
                MealAssemblyData tempMeal = _currentAssembleStation.currentMeal;
                GameObject pickedMeal = _currentAssembleStation.PickupMeal();
                if (pickedMeal is not null)
                {
                    isEmptyHanded = false;
                    _prefabInHand = Instantiate(pickedMeal, characterHead);
                    Transform uncookedIngredients = _prefabInHand.transform.Find("Uncooked_Ingridients");
                    if (uncookedIngredients is not null)
                    {
                        foreach (Transform child in uncookedIngredients)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                    _prefabInHand.transform.Find("Meal_Holder").GetChild(0).gameObject.SetActive(true);

                    _prefabInHand.SetActive(true);
                    heldMeal = tempMeal.mealSO;
                    isHoldingMeal = true;
                    isMealCooked = false;
                    itemInHand = GameEnums.Item.Meal;
                    
                }
            }
        }

        private void ResetInteraction()
        {
            _interactionTimer = 0f;
            interactionTimerImage.fillAmount = 0f;
            interactionTimerImage.gameObject.SetActive(false);
        }

        private void ResetHand()
        {
            itemInHand = GameEnums.Item.Empty;
            isEmptyHanded = true;
            _currentIngredient = null;
            situation4 = false; //because situation 4 is in an unreachable condition
            Destroy(_prefabInHand);
        }
        
        bool CheckIngredientValidity(Ingredient ingredient, GameEnums.IngredientState ingredientState, StationSO stationSo)
        {

            if (ingredient is not null && stationSo is not null)
            {
                foreach (StationSO.ReceivableItem item in stationSo.receivableItems)
                {
                    if (item.ingredient.ingredientID == ingredient.ingredientID && item.state == ingredientState)
                    {
                        return true;
                    }
                }
            }
           

            return false;
        }


        void AnalyzeSituation()
        {
            // Player is EMPTY Handed and interacting with a Resource Station
            situation1 = _isInsideStation && _currentStation is not null && isEmptyHanded &&
                        _currentStationSO.stationType == GameEnums.StationType.ResourceStation;
            // Player is NOT EMPTY Handed and interacting with an EMPTY Prep Station
            situation2 = _isInsideStation && _currentStation is not null && !isEmptyHanded &&
                         _currentStationSO.stationType is GameEnums.StationType.ChoppingStation or GameEnums.StationType.HeatStation
                         && !_isCurrentStationBusy && _currentStationIngredient is null && 
                         CheckIngredientValidity(_currentIngredient,_currentIngredient.currentState,_currentStationSO);
            
           // Player is EMPTY Handed and is going to take the PREPARED ingredient
            situation3 = _isInsideStation && _currentStation is not null && isEmptyHanded &&
                         _currentStationSO.stationType is GameEnums.StationType.ChoppingStation or GameEnums.StationType.HeatStation
                         && !_isCurrentStationBusy && _currentStationIngredient is not null  && _currentStationIngredient.currentState != GameEnums.IngredientState.Default;

           
            if (_currentAssembleStation is not null && _currentIngredient is not null)
            {
                MealAssemblyData meal = _currentAssembleStation.GetBestMatchingMeal(_currentIngredient);
                
                situation4 = _isInsideStation && !isEmptyHanded &&
                             _currentStationSO.stationType is GameEnums.StationType.AssembleStation && meal is not null;
            }
            else
            {
                situation4 = _isInsideStation && !isEmptyHanded &&
                             _currentStationSO.stationType is GameEnums.StationType.AssembleStation && !heldMeal;
            }

            if (_currentAssembleStation is not null)
            {
                situation5 = _isInsideStation && isEmptyHanded &&
                             _currentStationSO.stationType is GameEnums.StationType.AssembleStation &&
                             _currentAssembleStation.IsCurrentMealReady;
            }


            Debug.Log(situation1 + "   " + situation2  + "   " + situation3 + "  " + situation4 + "  " + situation5);
        }
    }
}
