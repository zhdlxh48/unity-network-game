using System;
using Runtime.Networks;
using TMPro;
using UnityEngine;

namespace Runtime.UIs
{
    public class UIManager : MonoBehaviour
    {
        public TMP_InputField emailText;
        
        public void Connect()
        {
            Client.Instance.Connect(emailText.text);
        }
        
        public void EnterRoom()
        {
            ClientEvent.EnterRoom(Client.Instance);
        }
        
        public void LeaveRoom()
        {
            ClientEvent.LeaveRoom(Client.Instance);
        }
    }
}