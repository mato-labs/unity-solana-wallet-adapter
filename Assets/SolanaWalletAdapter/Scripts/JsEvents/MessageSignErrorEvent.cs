using UnityEngine;

namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class MessageSignErrorEvent: JsEvent<string>
    {
        public delegate void MessageSignErrorDelegate();
        public new event MessageSignErrorDelegate OnEvent;
        
        public MessageSignErrorEvent(): base("OnMessageSignErrorEvent")
        {
            base.OnEvent += (err) =>
            {
                Debug.LogError($"Message sign error: {err}");
                
                OnEvent();
            };
        }
    }
}