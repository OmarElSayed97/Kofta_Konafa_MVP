using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    public class GameManager : MonoBehaviour
    {
        [Header("Order Settings")]
        public float orderLifetime = 60f; // Default lifetime for each order
        public float minOrderInterval = 5f; // Minimum time between new orders
        public float maxOrderInterval = 15f; // Maximum time between new orders
        public int maxActiveOrders = 4; // Maximum visible orders

        [Header("Game Settings")]
        public float gameDuration = 300f; // Total game time

        Queue<Order> orderQueue = new Queue<Order>();
        List<Order> activeOrders = new List<Order>();
        float remainingGameTime;
        bool isGameRunning;
        bool isGamePaused;

        
        
        #region Singleton

        private static GameManager _instance;

        public static GameManager Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        #endregion
        private void Start()
        {
            
        }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        private void InitializeGame()
        {
            ResetGame();
            StartCoroutine(GameTimer());
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            InitializeGame();
            isGameRunning = true;
            StartCoroutine(GenerateOrdersLoop());
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            isGamePaused = true;
            Debug.Log("Game Paused.");
        }

        /// <summary>
        /// Resumes the game after pausing.
        /// </summary>
        public void ResumeGame()
        {
            isGamePaused = false;
            Debug.Log("Game Resumed.");
        }

        /// <summary>
        /// Resets the game state.
        /// </summary>
        public void ResetGame()
        {
            isGameRunning = false;
            isGamePaused = false;
            remainingGameTime = gameDuration;
            orderQueue.Clear();
            activeOrders.Clear();
            Debug.Log("Game Reset.");
        }

        /// <summary>
        /// Manages the game timer.
        /// </summary>
        private IEnumerator GameTimer()
        {
            while (remainingGameTime > 0)
            {
                if (!isGameRunning || isGamePaused) yield return null;
                remainingGameTime -= Time.deltaTime;
                yield return null;
            }
            EndGame();
        }

        /// <summary>
        /// Ends the game and stops all activities.
        /// </summary>
        private void EndGame()
        {
            isGameRunning = false;
            Debug.Log("Game Over!");
        }

        /// <summary>
        /// Generates new orders at random intervals.
        /// </summary>
        private IEnumerator GenerateOrdersLoop()
        {
            while (isGameRunning && remainingGameTime > 0)
            {
                if (isGamePaused) yield return null;
                yield return new WaitForSeconds(Random.Range(minOrderInterval, maxOrderInterval));
                GenerateNewOrder();
            }
        }

        /// <summary>
        /// Generates a new order and manages its activation.
        /// </summary>
        private void GenerateNewOrder()
        {
            if (!isGameRunning || isGamePaused) return;
            
            var newOrder = new Order(GetRandomMealSO(), orderLifetime);
            orderQueue.Enqueue(newOrder);
            UpdateActiveOrders();
        }

        /// <summary>
        /// Updates active orders, ensuring a maximum of maxActiveOrders.
        /// </summary>
        private void UpdateActiveOrders()
        {
            Debug.Log(0);
            while (activeOrders.Count < maxActiveOrders && orderQueue.Any(o => !o.isActive))
            {
                Debug.Log(1);
                var nextOrder = orderQueue.FirstOrDefault(o => !o.isActive);
                if (nextOrder != null)
                {
                    Debug.Log(2);
                    nextOrder.isActive = true;
                    activeOrders.Add(nextOrder);
                }
            }
        }

        /// <summary>
        /// Handles player delivering a meal.
        /// </summary>
        public bool DeliverOrder(MealSO deliveredMeal)
        {
            var orderToDeliver = activeOrders
                .Where(o => o.meal == deliveredMeal && o.isActive && !o.isDelivered)
                .OrderBy(o => o.remainingTime)
                .FirstOrDefault();

            if (orderToDeliver != null)
            {
                orderToDeliver.isDelivered = true;
                activeOrders.Remove(orderToDeliver);
                orderQueue = new Queue<Order>(orderQueue.Where(o => !o.isDelivered));
                UpdateActiveOrders();
                Debug.Log($"Delivered {deliveredMeal.name} successfully!");
                return true;
            }

            Debug.LogWarning("No matching order found for delivery.");
            return false;
        }

        /// <summary>
        /// Updates all orders' timers and removes expired orders.
        /// </summary>
        private void Update()
        {
            if (!isGameRunning || isGamePaused) return;

            for (int i = activeOrders.Count - 1; i >= 0; i--)
            {
                activeOrders[i].remainingTime -= Time.deltaTime;
                if (activeOrders[i].remainingTime <= 0)
                {
                    Debug.LogWarning($"Order for {activeOrders[i].meal.name} expired.");
                    activeOrders.RemoveAt(i);
                }
            }
            UpdateActiveOrders();
        }

        /// <summary>
        /// Simulates fetching a random meal for a new order.
        /// </summary>
        private MealSO GetRandomMealSO()
        {
            return KitchenManager.Instance.playerOneMealData[Random.Range(0, KitchenManager.Instance.playerOneMealData.Count)].mealSO;
        }
    }

    /// <summary>
    /// Represents a customer order.
    /// </summary>
    [System.Serializable]
    public class Order
    {
        public MealSO meal;
        public bool isDelivered;
        public float remainingTime;
        public bool isActive;

        public Order(MealSO meal, float orderLifetime)
        {
            this.meal = meal;
            this.isDelivered = false;
            this.remainingTime = orderLifetime;
            this.isActive = false;
        }
    }
} 
