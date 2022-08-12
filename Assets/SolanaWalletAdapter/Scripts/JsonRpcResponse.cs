using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts
{
    public class JsonRpcResponse<T>
    {
        [JsonProperty("id")] 
        public int Id { get; set; }

        [JsonProperty("result")]
        public T Result;
    }
}