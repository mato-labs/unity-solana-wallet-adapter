using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolanaWalletAdapter.Demo.Scripts
{
    public class WalletButtonView: MonoBehaviour
    {
        public TextMeshProUGUI WalletNameLabel;
        public GameObject DetectedLabel;
        public GameObject CanSign;

        public string Id { get; set; }

        public Action<string> OnSelectedAction;
        
        public void OnSelected()
        {
            OnSelectedAction?.Invoke(Id);
        }
    }
}