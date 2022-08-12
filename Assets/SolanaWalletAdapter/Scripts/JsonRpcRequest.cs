using System;
using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts
{
    [Serializable]
    public class JsonRpcRequest
    {
        [JsonIgnore]
        private static int _id;
        
        [JsonProperty("jsonrpc")]
        public readonly string JsonRpcVersion = "2.0";

        [JsonProperty("id")] 
        public readonly int Id;
        
        [JsonProperty("method")]
        public readonly string Method;
        
        [JsonProperty("params")]
        public readonly object[] Params;

        public JsonRpcRequest(string method, object[] prms)
        {
            Id = _id++;
            Method = method;
            Params = prms;
        }
    }
}