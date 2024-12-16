using System.Collections;
using System.Collections.Generic;
using KoftaAndKonafa.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MealButton : MonoBehaviour
{
    [SerializeField] public MealSO mealSO;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mealSO.name;
        transform.GetChild(1).GetComponent<Image>().sprite = mealSO.mealImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
