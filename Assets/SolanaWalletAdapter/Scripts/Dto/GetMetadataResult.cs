using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts.Dto
{
    public class GetMetadataResult
    {
        [JsonProperty("name")]
        public string NftName { get; private set; }
        
        [JsonProperty("url")]
        public string MetadataUrl { get; private set; }
        
        [JsonProperty("mint")]
        public string Mint { get; private set; }
        
        public GetMetadataResult(string name, string uri, string mint)
        {
            NftName = name;
            MetadataUrl = uri; 
            Mint = mint;
        }

        public override string ToString()
        {
            return $"Nft name: {NftName}, metadata url: {MetadataUrl}, mint: {Mint}";
        }
    }
}