using UnityEngine;

namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class SendTransactionErrorEvent: JsEvent<string>
    {
        public delegate void SendTransactionErrorDelegate();
        public new event SendTransactionErrorDelegate OnEvent;
        
        public SendTransactionErrorEvent() : base("OnSendTxErrorEvent")
        {
            base.OnEvent += (err) =>
            {
                Debug.LogError($"Send tx error: {err}");
                
                OnEvent();
            };
        }
    }
}