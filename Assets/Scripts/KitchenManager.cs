using System.Collections;
using System.Collections.Generic;
using KoftaAndKonafa;
using KoftaAndKonafa.ScriptableObjects;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    #region Singleton

    private static KitchenManager _instance;
    private KitchenBuilder _kitchenBuilder;

    public static KitchenManager Instance { get { return _instance; } }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        _kitchenBuilder = GetComponent<KitchenBuilder>();
    }
    #endregion
    
    
   
    
    [SerializeField]
    public List<MealSO> playerOneMeals;
    public List<MealAssemblyData> playerOneMealData;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildKitchen()
    {
        _kitchenBuilder.StartKitchen();
    }
}
