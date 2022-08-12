
namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class WalletConnectedEvent: JsEvent<string, string>
    {
        public delegate void WalletConnectedDelegate(string walletname, string pubkey);
        public new event WalletConnectedDelegate OnEvent;
        
        public WalletConnectedEvent(): base("OnWalletConnectedEvent")
        {
            base.OnEvent += (a, b) => OnEvent(a, b);
        }
    }
}