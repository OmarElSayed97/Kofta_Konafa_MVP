using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoftaAndKonafa.ScriptableObjects;
using Unity.VisualScripting;

namespace KoftaAndKonafa
{
    [System.Serializable]
    public class Bot
    {
        public string botName;
        public List<MealSO> botMeals;
        public Vector2 deliveryInterval;
        public int totalScore;

        public Bot(string name, List<MealSO> meals, Vector2 deliveryTime )
        {
            botName = name;
            botMeals = meals;
            deliveryInterval = deliveryTime;
            totalScore = 0;
        }
    }

    public class BotScoreSimulator : MonoBehaviour
    {
        [Header("Bot Settings")] public List<Vector2> botsIntervalTimes;

        public List<Bot> bots;
        public LeaderboardManager leaderboardManager;

        private void Start()
        {
            for (int i = 0; i < bots.Count; i++)
                bots[i].deliveryInterval = botsIntervalTimes[i];
            StartScoreSimulation();
        }

        /// <summary>
        /// Starts the bots' score simulation.
        /// </summary>
        public void StartScoreSimulation()
        {
            if(bots.Count == 0)
            {
              Debug.Log("NO BOTS");
              return;
            }
                
            foreach (var bot in bots)
            {
                StartCoroutine(SimulateBotScore(bot));
            }
        }

        /// <summary>
        /// Simulates a bot delivering orders at random intervals.
        /// </summary>
        private IEnumerator SimulateBotScore(Bot bot)
        {
            while (GameManager.Instance.isGameRunning)
            {
                yield return new WaitForSeconds(Random.Range(bot.deliveryInterval.x, bot.deliveryInterval.y));

                if (bot.botMeals.Count == 0) continue;

                // Pick a random meal and update the leaderboard
                MealSO deliveredMeal = bot.botMeals[Random.Range(0, bot.botMeals.Count)];
                int mealPrice = deliveredMeal.mealPrice;
                bot.totalScore += mealPrice;
                leaderboardManager.UpdatePlayerScore(bot.botName, mealPrice);

                Debug.Log($"{bot.botName} delivered {deliveredMeal.name} for {mealPrice} points! Total Score: {bot.totalScore}");
            }
        }
    }
}