using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KoftaAndKonafa;
using KoftaAndKonafa.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryStation : MonoBehaviour
{
    private CharacterManager _characterManager;

    [SerializeField] private Transform[] mealsPlaceholders;
    
    [SerializeField] private GameObject[] mealScoresUI;

    [SerializeField] private Image scoreUI;
    [SerializeField] private TextMeshProUGUI scoreUIText;
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
                    TweenMealAndSell(mealsprefab,_characterManager.heldMeal);
                    _characterManager.ResetMeal();
                    return;
                }
            }
           
        }
    }

    void TweenMealAndSell(GameObject mealObject, MealSO mealSO)
    {
        AnimateScore(mealSO.mealPrice,scoreUI,scoreUIText);
        mealObject.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => DeliverOrderInStation(mealObject,mealSO));
        
        
    }

    void DeliverOrderInStation(GameObject mealObject, MealSO mealSO)
    {
       GameManager.Instance.DeliverOrder(mealSO);
       Destroy(mealObject);
    }
    
    public void AnimateScore(int targetScore, Image scoreImage, TextMeshProUGUI scoreText)
    {
        // Activate the image
        scoreImage.gameObject.SetActive(true);

        // Reset initial values
        scoreImage.rectTransform.localScale = Vector3.one * 0.1f;
        scoreImage.rectTransform.localPosition = Vector3.zero;
        scoreText.text = "0";

        // Activate image
        scoreImage.gameObject.SetActive(true);

        Sequence scoreSequence = DOTween.Sequence();

        // Move up and scale simultaneously
        scoreSequence.Append(scoreImage.rectTransform
            .DOMoveY(scoreImage.rectTransform.position.y + 0.8f, 0.9f)
            .SetEase(Ease.OutQuad));

        scoreSequence.Join(scoreImage.rectTransform
            .DOScale(Vector3.one, 0.9f)
            .SetEase(Ease.OutBack));

        // Count score during upward movement
        scoreSequence.Join(DOTween.To(() => 0, value => scoreText.text = "+" +value + "$", targetScore, 0.9f)
            .SetEase(Ease.Linear));

        // Final scale punch after reaching position
        scoreSequence.Append(scoreImage.rectTransform
            .DOPunchScale(Vector3.one * 0.3f, 0.4f, 1, 0));

        // Deactivate after completion
        scoreSequence.OnComplete(() => scoreImage.gameObject.SetActive(false));

        scoreSequence.Play();
    }


}
