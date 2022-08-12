namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class MintMetadataFailedToLoadEvent: JsEvent<string>
    {
        public delegate void MintMetadataFailedToLoadDelegate(string mint);
        public new event MintMetadataFailedToLoadDelegate OnEvent;
        
        public MintMetadataFailedToLoadEvent() : base("OnMintMetadataFailedToLoadedEvent")
        {
            base.OnEvent += (a) => OnEvent(a);
        }
    }
}