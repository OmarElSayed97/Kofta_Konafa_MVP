using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using KoftaAndKonafa;
using KoftaAndKonafa.Enums;
using KoftaAndKonafa.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class StationManager : MonoBehaviour
{

    [SerializeField] public StationSO stationSO;

    [SerializeField] public GameObject stationItemPlaceholder;
    
    [SerializeField] public Ingredient currentIngredient;

    [HideInInspector] public int stationID;
    

    private IngredientLoader _ingredientLoader;
    private CharacterManager _characterManager;
    private KitchenManager _kitchenManager;
    
    // Indicates if the station is currently in use
    public bool isBusy;

    private GameObject currentItemPrefab;
    
    [Header("UI Elements")]
    public Image stationTimerImage;

    private float _actionTimer;
    private bool _isPlayerInside;
    [HideInInspector]
    public bool isActionInProgress;
    private GameObject _currentItem;
    private float _requiredTime;
    // Start is called before the first frame update
    void Start()
    {
        _kitchenManager = KitchenManager.Instance;
        _ingredientLoader = IngredientLoader.Instance;
        _characterManager = CharacterManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isActionInProgress)
        {
            _actionTimer += Time.deltaTime;
            stationTimerImage.fillAmount = _actionTimer / _requiredTime;

            if (_actionTimer >= _requiredTime)
            {
                CompleteAction();
                ResetStation();
            }
        }
    }


    public void TakeItemFromPlayer(IngredientSO item, GameEnums.IngredientState itemState, float actionTime)
    {
        if (isActionInProgress) return;
            _requiredTime = actionTime;
        if (itemState == GameEnums.IngredientState.Default)
        { 
            currentItemPrefab = Instantiate(_ingredientLoader.ingredientsPrefabs[item.ingredientID], stationItemPlaceholder.transform);
            currentIngredient = new Ingredient(item);
        }
        
        if (stationSO.stationType == GameEnums.StationType.HeatStation)
        {
            StartAction();
        }

        if (stationSO.stationType == GameEnums.StationType.ChoppingStation && _isPlayerInside)
        {
            StartAction();
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && stationSO.stationType == GameEnums.StationType.ChoppingStation)
        {
            _isPlayerInside = true;
            if(currentItemPrefab is not null && currentIngredient.currentState == GameEnums.IngredientState.Default)
                StartAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && stationSO.stationType == GameEnums.StationType.ChoppingStation)
        {
            _isPlayerInside = false;
            ResetStation();
        }
    }

    private void StartAction()
    {
        isActionInProgress = true;
        stationTimerImage.gameObject.SetActive(true);
    }

    private void CompleteAction()
    {
        Debug.Log($"{stationSO.stationType} action completed.");
        Destroy(currentItemPrefab);
        if (stationSO.stationType == GameEnums.StationType.ChoppingStation)
            currentIngredient.currentState = GameEnums.IngredientState.Chopped;
        if (stationSO.stationType == GameEnums.StationType.HeatStation)
            currentIngredient.currentState = GameEnums.IngredientState.Heated;

        currentItemPrefab = Instantiate(_ingredientLoader.preparedIngredientsPrefabs[currentIngredient.ingredientID],
            stationItemPlaceholder.transform);
        
        ResetStation();
        _characterManager.StationAlertPlayer(stationID,this);
        // Add logic for completing the action here
    }

    public void ResetStation()
    {
        _actionTimer = 0f;
        isActionInProgress = false;
        stationTimerImage.fillAmount = 0f;
        stationTimerImage.gameObject.SetActive(false);
    }

    public void ClearStation()
    {
        Destroy(currentItemPrefab);
        currentItemPrefab = null;
        currentIngredient = null;
    }
    
    
   

   
}
