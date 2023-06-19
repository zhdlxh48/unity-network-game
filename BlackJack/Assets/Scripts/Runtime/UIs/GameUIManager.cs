using System;
using System.Threading.Tasks;
using Runtime.Networks;
using ServerLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Runtime.UIs
{
    public class GameUIManager : MonoBehaviour
    {
        private static GameUIManager _instance;
        public static GameUIManager Instance => _instance;
        
        [SerializeField] private TextMeshProUGUI gameLabelText;
        [SerializeField] private Button cardDrawButton;
        
        [SerializeField] private TextMeshProUGUI playerCardText;
        [SerializeField] private TextMeshProUGUI enemyCardPrefab;
        
        [SerializeField] private RectTransform enemyCardParent;
        
        [SerializeField] private Button lobbyButton;
        
        [System.Serializable]
        public class PlayerCardDictionary : SerializableDictionary<int, TextMeshProUGUI> { }
        [SerializeField] private PlayerCardDictionary enemyCards = new();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            gameLabelText.text = "Game Start!";
            cardDrawButton.gameObject.SetActive(false);
            lobbyButton.gameObject.SetActive(false);
        }

        public void AddCard(int id, int num)
        {
            var text = Instantiate(enemyCardPrefab, enemyCardParent);
            text.gameObject.SetActive(false);
            enemyCards.Add(id, text);
            text.text = $"{num}";
        }
        
        public void SetCard(int id, int num)
        {
            enemyCards[id].text = $"{num}";
            enemyCards[id].gameObject.SetActive(false);
        }

        public void ShowDrawUI()
        {
            gameLabelText.text = "Draw a card!";
            cardDrawButton.gameObject.SetActive(true);
        }
        
        public void ShowWaitingUI()
        {
            gameLabelText.text = "Waiting...";
            cardDrawButton.gameObject.SetActive(false);
        }
        
        public void ShowMyNumberUI()
        {
            gameLabelText.text = "My number is...";
            cardDrawButton.gameObject.SetActive(false);
            
            playerCardText.text = $"{Client.Instance.DrawCard}";
        }

        public void ShowGameResultUI()
        {
            gameLabelText.text = "Result...";
            
            foreach (var enemyCard in enemyCards)
            {
                enemyCard.Value.gameObject.SetActive(true);
            }
        }

        public async void ShowWinUI()
        {
            var lower = int.MaxValue;
            foreach (var card in GameLogic.Instance.PlayerCards.Values)
            {
                lower = Math.Min(lower, card);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(2d));

            var getScore = Client.Instance.DrawCard - lower;
            gameLabelText.text = $"Win!! (+{getScore})";
            
            // 걍 여기서 DB
            
            await Task.Delay(TimeSpan.FromSeconds(2d));
            
            lobbyButton.gameObject.SetActive(true);
        }
        
        public async void ShowLoseUI()
        {
            await Task.Delay(TimeSpan.FromSeconds(2d));

            gameLabelText.text = "Lose...";
            
            await Task.Delay(TimeSpan.FromSeconds(2d));
            
            lobbyButton.gameObject.SetActive(true);
        }
        
        public void ReturnLobby()
        {
            ClientEvent.LeaveRoom(Client.Instance);
            SceneManager.LoadScene("MainScene");
        }
        
        public void DrawCard()
        {
            GameEvent.DrawCard(Client.Instance);

            ShowMyNumberUI();
        }
    }
}