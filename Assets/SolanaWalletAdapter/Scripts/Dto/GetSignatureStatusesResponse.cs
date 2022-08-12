using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolanaWalletAdapter.Scripts.Dto
{
    public class GetSignatureStatusesResponse
    {
        [JsonProperty("confirmations")]
        public int Confirmations { get; private set; }
        
        [JsonProperty("confirmationStatus")]
        public string ConfirmationStatus { get; private set; }
        
        [JsonProperty("err")]
        public JObject Error { get; private set; }

        [JsonIgnore]
        public bool IsProcessed => ConfirmationStatus == "processed";
        
        [JsonIgnore]
        public bool IsConfirmed => ConfirmationStatus == "confirmed";
        
        [JsonIgnore]
        public bool IsFinalized => ConfirmationStatus == "finalized";

        [JsonIgnore]
        public bool IsError => Error != null;

        public override string ToString()
        {
            return $"Stats: {ConfirmationStatus}, confirmations: {Confirmations}, error? {IsError}";
        }
    }
}