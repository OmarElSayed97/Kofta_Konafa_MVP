using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace KoftaAndKonafa
{
    public class LeaderboardManager : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerScoreData
        {
            public string playerName;
            public int score;
            public RectTransform rowTransform;
            public Text playerNameText;
            public Text scoreText;
            public Image rankImage;
            [HideInInspector] public int rank;
        }

        [Header("Leaderboard Settings")]
        public List<PlayerScoreData> playerScores;
        public float animationDuration = 0.5f;
        public List<Sprite> rankSprites; // 4 rank sprites

        private void Start()
        {
            InitializeLeaderboard();
        }

        /// <summary>
        /// Initializes the leaderboard display.
        /// </summary>
        private void InitializeLeaderboard()
        {
            foreach (var player in playerScores)
            {
                player.playerNameText.text = player.playerName;
                player.scoreText.text = player.score.ToString();
                UpdatePlayerRank(player);
            }
        }

        /// <summary>
        /// Updates the score of a player and refreshes the leaderboard.
        /// </summary>
        public void UpdatePlayerScore(string playerName, int scoreChange)
        {
            var player = playerScores.Find(p => p.playerName == playerName);
            if (player == null) return;

            player.score += scoreChange;
            player.scoreText.text = player.score.ToString();

            RefreshLeaderboard();
        }

        /// <summary>
        /// Refreshes and reorders the leaderboard based on scores.
        /// </summary>
        private void RefreshLeaderboard()
        {
            playerScores.Sort((a, b) => b.score.CompareTo(a.score));

            for (int i = 0; i < playerScores.Count; i++)
            {
                var targetPosition = new Vector2(playerScores[i].rowTransform.anchoredPosition.x, -i * playerScores[i].rowTransform.sizeDelta.y - 50);
                playerScores[i].rowTransform.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.OutQuad);
                playerScores[i].rank = i + 1;
                UpdatePlayerRank(playerScores[i]);
            }
        }

        /// <summary>
        /// Updates a player's rank sprite.
        /// </summary>
        private void UpdatePlayerRank(PlayerScoreData player)
        {
            if (player.rank > 0 && player.rank <= rankSprites.Count)
            {
                player.rankImage.sprite = rankSprites[player.rank - 1];
            }
        }
    }
}
