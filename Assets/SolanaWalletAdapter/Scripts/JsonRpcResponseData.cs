using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts
{
    public class JsonRpcResponseValue<T>
    {
        [JsonProperty("value")]
        public T Value;
    }
}