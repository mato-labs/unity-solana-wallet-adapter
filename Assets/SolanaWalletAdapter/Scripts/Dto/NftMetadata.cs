using Newtonsoft.Json;
using UnityEngine;

namespace SolanaWalletAdapter.Scripts.Dto
{
    public class NftCollectionMetadata
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("family")]
        public string Family { get; private set; }

        public override string ToString()
        {
            return $"Name: {Name}, Family: {Family}";
        }
    }

    public class NftAttributeMetadata
    {
        [JsonProperty("trait_type")]
        public string AttributeType { get; private set; }
        
        [JsonProperty("value")]
        public string AttributeValue { get; private set; }

        public override string ToString()
        {
            return $"Type: {AttributeType}, value: {AttributeValue}";
        }
    }
    
    public class NftMetadata
    {
        [JsonIgnore]
        public string Mint { get; set; }
        
        [JsonIgnore]
        public Sprite Sprite { get; set; }

        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("symbol")]
        public string Symbol { get; private set; }
        
        [JsonProperty("image")]
        public string ImageUrl { get; private set; }
        
        [JsonProperty("external_url")]
        public string Website { get; private set; }
        
        [JsonProperty("collection")]
        public NftCollectionMetadata CollectionMetadata { get; private set; }
        
        [JsonProperty("attributes")]
        public NftAttributeMetadata[] Attributes { get; private set; }

        public override string ToString()
        {
            return $"Mint: {Mint}, name: {Name}, image: {ImageUrl}";
        }
    }
}