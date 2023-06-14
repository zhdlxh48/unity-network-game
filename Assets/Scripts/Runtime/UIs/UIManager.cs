using FiveBee.Runtime.Networks;
using TMPro;
using UnityEngine;

namespace FiveBee.Runtime.UIs
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }
                if (_instance == null)
                {
                    Debug.LogWarning($"Instance of {nameof(UIManager)} not found, creating new one...");
                    _instance = new GameObject("UIManager").AddComponent<UIManager>();
                }
                
                return _instance;
            }
        }
        
        public GameObject startMenu;
        public TMP_InputField usernameField;

        public void ConnectToServer()
        {
            startMenu.SetActive(false);
            usernameField.interactable = false;
            
            Client.Instance.ConnectToServer();
        }
    }
}