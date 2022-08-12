namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class WalletMessageSignedEvent: JsEvent<string, string, string, string>
    {
        public delegate void WalletMessageSignedDelegate(string walletname, string pubkey, string msg, string signature);
        public new event WalletMessageSignedDelegate OnEvent;
        
        public WalletMessageSignedEvent() : base("OnWalletMessageSignedEvent")
        {
            base.OnEvent += (a, b, c, d) => OnEvent(a, b, c, d);
        }
    }
}