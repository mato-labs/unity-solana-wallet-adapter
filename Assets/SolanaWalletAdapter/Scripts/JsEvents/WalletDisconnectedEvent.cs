namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class WalletDisconnectedEvent: JsEvent<string>
    {
        public delegate void WalletDisconnectedDelegate(string walletname);
        public new event WalletDisconnectedDelegate OnEvent;
        
        public WalletDisconnectedEvent(): base("OnWalletDisconnectedEvent")
        {
            base.OnEvent += (a) => OnEvent(a);
        }
    }
}