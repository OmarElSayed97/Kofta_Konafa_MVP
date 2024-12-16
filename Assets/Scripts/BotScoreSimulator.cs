using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    [System.Serializable]
    public class Bot
    {
        public string botName;
        public List<MealAssemblyData> botMeals;
        public Vector2 deliveryInterval;
        public int totalScore;
    }

    public class BotScoreSimulator : MonoBehaviour
    {
        [Header("Bot Settings")]
        public List<Bot> bots;
        public LeaderboardManager leaderboardManager;

        private void Start()
        {
            StartScoreSimulation();
        }

        /// <summary>
        /// Starts the bots' score simulation.
        /// </summary>
        public void StartScoreSimulation()
        {
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
                MealAssemblyData deliveredMeal = bot.botMeals[Random.Range(0, bot.botMeals.Count)];
                int mealPrice = deliveredMeal.mealSO.mealPrice;
                bot.totalScore += mealPrice;
                leaderboardManager.UpdatePlayerScore(bot.botName, mealPrice);

                Debug.Log($"{bot.botName} delivered {deliveredMeal.mealSO.name} for {mealPrice} points! Total Score: {bot.totalScore}");
            }
        }
    }
}