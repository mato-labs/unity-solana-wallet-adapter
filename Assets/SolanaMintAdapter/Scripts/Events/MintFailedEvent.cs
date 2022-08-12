using SolanaWalletAdapter.Scripts;
using UnityEngine;

namespace SolanaMintAdapter.Scripts.Events
{
    public class MintFailedEvent: JsEvent<string>
    {
        public delegate void MintFailedEventDelegate();
        public new event MintFailedEventDelegate OnEvent;
        
        
        public MintFailedEvent() : base("OnMintFailedEvent")
        {
            base.OnEvent += err =>
            {
                Debug.LogError($"Mint failed: error: {err}");
                
                OnEvent.Invoke();
            };
        }
    }
}