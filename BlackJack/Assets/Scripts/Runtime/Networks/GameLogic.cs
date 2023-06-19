using System;
using System.Collections.Generic;
using Runtime.UIs;
using ServerLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Networks
{
    public class GameLogic : MonoBehaviour
    {
        private static GameLogic _instance;
        public static GameLogic Instance => _instance;
        
        [System.Serializable]
        public class PlayerCardDictionary : SerializableDictionary<int, int> { }
        [field: SerializeField] public PlayerCardDictionary PlayerCards { get; set; } = new();

        [field: SerializeField] public GameEventData.EventType CurrentEventType { get; set; } = GameEventData.EventType.Start;

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
            
            DontDestroyOnLoad(gameObject);
        }
        
        public void HandleLogic(GameEventData data)
        {
            if (data.Type == GameEventData.EventType.Start)
            {
                SceneManager.LoadScene("GameScene");
            }
            else if (data.Type == GameEventData.EventType.CardDrawWait)
            {
                var playerId = data.PlayerId;
                
                if (CurrentEventType != GameEventData.EventType.CardDrawEnd)
                {
                    CurrentEventType = GameEventData.EventType.CardDrawWait;
                    GameUIManager.Instance.ShowWaitingUI();
                }
                
                if (PlayerCards.TryGetValue(playerId, out var value))
                {
                    Debug.Log($"{playerId} was draw a card: {data.DrawCard}");
                    PlayerCards[playerId] = data.DrawCard;
                    GameUIManager.Instance.SetCard(playerId, data.DrawCard);
                }
                else
                {
                    Debug.Log($"Can not find player {playerId} in PlayerCards! Add!");
                    PlayerCards.Add(playerId, data.DrawCard);
                    GameUIManager.Instance.AddCard(playerId, data.DrawCard);
                }
            }
            else if (data.Type == GameEventData.EventType.CardDrawStart)
            {
                var playerId = data.PlayerId;
                
                CurrentEventType = GameEventData.EventType.CardDrawStart;
                GameUIManager.Instance.ShowDrawUI();
                Debug.Log("Card draw start!");
                
                Debug.Log($"You are draw a card: {data.DrawCard}");
                Client.Instance.DrawCard = data.DrawCard;
            }
            else if (data.Type == GameEventData.EventType.Win)
            {
                GameUIManager.Instance.ShowGameResultUI();
                GameUIManager.Instance.ShowWinUI();
            }
            else if (data.Type == GameEventData.EventType.Lose)
            {
                GameUIManager.Instance.ShowGameResultUI();
                GameUIManager.Instance.ShowLoseUI();
            }
        }
    }
}