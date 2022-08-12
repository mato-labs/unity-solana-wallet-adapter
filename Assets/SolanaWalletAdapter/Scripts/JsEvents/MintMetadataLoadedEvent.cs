using SolanaWalletAdapter.Scripts.Dto;

namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class MintMetadataLoadedEvent: JsEvent<string, string, string>
    {
        public delegate void MintMetadataLoadedDelegate(GetMetadataResult dto);
        public new event MintMetadataLoadedDelegate OnEvent;
        
        public MintMetadataLoadedEvent() : base("OnMintMetadataLoadedEvent")
        {
            base.OnEvent += (a, b, c) => OnEvent(new GetMetadataResult(a, b, c));
        }
    }
}