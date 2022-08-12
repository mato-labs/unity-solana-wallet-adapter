using UnityEngine;

namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class WalletErrorEvent: JsEvent<string>
    {
        public delegate void WalletErrorDelegate();
        public new event WalletErrorDelegate OnEvent;
        
        public WalletErrorEvent(): base("OnWalletErrorEvent")
        {
            base.OnEvent += (err) =>
            {
                Debug.LogError($"Wallet error: {err}");
                
                OnEvent();
            };
        }
    }
}