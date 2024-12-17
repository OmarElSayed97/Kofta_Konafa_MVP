using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KoftaAndKonafa.ScriptableObjects;

namespace KoftaAndKonafa
{
    public class DraftManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public List<Button> mealButtons;
        public List<Image> player1MealImages;
        public List<Image> player2MealImages;
        public List<Image> player3MealImages;
        public List<Image> player4MealImages;
        public TMP_Text currentTurnText, mealSelectionText;
        public Button continueButton;

        public Transform mealGridTransform;

        [Header("Draft Settings")]
        public float botDecisionTime = 2f;

        public GameObject draftPanel,hudPanel;

        private List<MealSO> player1Meals = new List<MealSO>();
        private List<MealSO> player2Meals = new List<MealSO>();
        private List<MealSO> player3Meals = new List<MealSO>();
        private List<MealSO> player4Meals = new List<MealSO>();

        private int currentRound = 0;
        private int currentPlayerIndex = 0;
        private bool isDrafting = true;

        private void Start()
        {
            ShuffleMenu(mealGridTransform);
            InitializeDraft();
        }

        /// <summary>
        /// Initializes the draft by enabling buttons and resetting states.
        /// </summary>
        private void InitializeDraft()
        {
            foreach (var button in mealButtons)
            {
                button.interactable = true;
                button.onClick.AddListener(() => OnMealSelected(button));
            }
            continueButton.interactable = false;
        }

        /// <summary>
        /// Handles player meal selection.
        /// </summary>
        private void OnMealSelected(Button selectedButton)
        {
            if (currentPlayerIndex != 0 || !isDrafting) return;

            MealSO selectedMeal = selectedButton.GetComponent<MealButton>().mealSO;
            AddMealToPlayer(0, selectedMeal);
            selectedButton.interactable = false;
            StartCoroutine(ProcessNextTurn());
        }

        /// <summary>
        /// Processes the next turn in the draft.
        /// </summary>
        private IEnumerator ProcessNextTurn()
        {
            isDrafting = false;

            // Bot turns
            for (int i = 1; i <= 2; i++)
            {
                UpdateTurnText("Bot_" + i + " is currently choosing");
                yield return new WaitForSeconds(botDecisionTime);
                MealSO selectedMeal = GetRandomAvailableMeal();
                AddMealToPlayer(i, selectedMeal);
                DisableMealButton(selectedMeal);
                if (i == 2)
                {
                    AddMealToPlayer(3, selectedMeal);
                }
            }

            currentRound++;

            if (currentRound >= 3)
            {
                continueButton.interactable = true;
                isDrafting = false;
                yield break;
            }

            isDrafting = true;
            currentPlayerIndex = 0;
            UpdateTurnText("Your Turn! Choose a Meal.");
        }

        /// <summary>
        /// Adds a meal to the appropriate player.
        /// </summary>
        private void AddMealToPlayer(int playerIndex, MealSO meal)
        {
            List<MealSO> targetList = playerIndex switch
            {
                0 => player1Meals,
                1 => player2Meals,
                2 => player3Meals,
                3 => player4Meals,
                _ => null
            };

            if (targetList == null || targetList.Contains(meal)) return;

            targetList.Add(meal);
            Image mealImage = playerIndex switch
            {
                0 => player1MealImages[player1Meals.Count - 1],
                1 => player2MealImages[player2Meals.Count - 1],
                2 => player3MealImages[player3Meals.Count - 1],
                3 => player4MealImages[player4Meals.Count - 1],
                _ => null
            };

            if (mealImage is not null)
            {
                mealImage.sprite = meal.mealIcon;
                mealImage.gameObject.SetActive(true);
            }

          
        }

        /// <summary>
        /// Disables the meal button after selection.
        /// </summary>
        private void DisableMealButton(MealSO meal)
        {
            foreach (var button in mealButtons)
            {
                if (button.GetComponent<MealButton>().mealSO == meal)
                {
                    button.interactable = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the text indicating whose turn it is.
        /// </summary>
        private void UpdateTurnText(string message)
        {
            currentTurnText.text = message;
        }

        /// <summary>
        /// Selects a random available meal for bot players.
        /// </summary>
        private MealSO GetRandomAvailableMeal()
        {
            List<Button> availableButtons = mealButtons.FindAll(button => button.interactable);
            if (availableButtons.Count == 0) return null;

            Button randomButton = availableButtons[Random.Range(0, availableButtons.Count)];
            return randomButton.GetComponent<MealButton>().mealSO;
        }

        /// <summary>
        /// Starts the game after selecting meals.
        /// </summary>
        public void StartGame()
        {
            KitchenManager.Instance.playerOneMealData.Clear();
            KitchenManager.Instance.playerOneMeals.Clear();
            foreach (var meal in player1Meals)
            {
                KitchenManager.Instance.playerOneMealData.Add(new MealAssemblyData { mealSO = meal });
                KitchenManager.Instance.playerOneMeals.Add(meal);
            }

            List<Bot> botList = new List<Bot>();
            for (int i = 1; i <= 3; i++)
            {
                
                List<MealSO> targetList = i switch
                {
                    1 => player2Meals,
                    2 => player3Meals,
                    3 => player4Meals,
                    _ => null
                };
                string botName = "bot_" + i;
                Bot bot = new Bot(botName, targetList, new Vector2(8,20) );
                botList.Add(bot);

            }
            hudPanel.SetActive(true);
            KitchenManager.Instance.BuildKitchen();
            GameManager.Instance.InitializeBots(botList);
            GameManager.Instance.StartGame();
            Debug.Log("Game Started!");
            draftPanel.SetActive(false);
           
        }
        
        
        public static void ShuffleMenu(Transform parent)
        {
            if (parent.childCount <= 1) return;

            // Store the children in a list
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < parent.childCount; i++)
            {
                children.Add(parent.GetChild(i));
            }

            // Shuffle the list using Fisher-Yates shuffle
            for (int i = children.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                //Swap
                (children[i], children[randomIndex]) = (children[randomIndex], children[i]);
            }

            // Reassign children in shuffled order
            foreach (Transform child in children)
            {
                child.SetSiblingIndex(children.IndexOf(child));
            }
        }
    }
}
