using SolanaWalletAdapter.Scripts;

namespace SolanaMintAdapter.Scripts.Events
{
    public class MintSuccessEvent: JsEvent<string>
    {
        public MintSuccessEvent() : base("OnMintSuccessEvent")
        {
        }
    }
}