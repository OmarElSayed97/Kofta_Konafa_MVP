using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KoftaAndKonafa;
using UnityEngine;

public class DeliveryStation : MonoBehaviour
{
    private CharacterManager _characterManager;

    [SerializeField] private Transform[] mealsPlaceholders;
    
    [SerializeField] private GameObject[] mealScoresUI;
    // Start is called before the first frame update
    void Start()
    {
        _characterManager = CharacterManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _characterManager.isHoldingMeal && _characterManager.isMealReady)
        {
            // heldMeal = stationManager.PickUpCookedMeal(characterHead);
            // characterHead.GetChild(0).transform.position = characterHead.position;
            // characterHead.GetChild(0).transform.Find("Meal_Holder").GetChild(0).gameObject.SetActive(true);
            // characterHead.GetChild(0).transform.Find("Cooked_Ingridients").gameObject.SetActive(true);

            for (int i = 0; i < mealsPlaceholders.Length; i++)
            {
                if (mealsPlaceholders[i].childCount == 0)
                {
                    GameObject mealsprefab = Instantiate(_characterManager.heldMeal.mealPrefabItem, mealsPlaceholders[i]);
                    mealsprefab.transform.Find("Meal_Holder").GetChild(0).gameObject.SetActive(true);
                    mealsprefab.transform.Find("Cooked_Ingridients").gameObject.SetActive(true); 
                    _characterManager.ResetMeal();
                    TweenMealAndSell(mealsprefab);
                    return;
                }
            }
           
        }
    }


    void TweenMealAndSell(GameObject mealObject)
    {
        mealObject.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(()=> Destroy(mealObject));
    }
}
