using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts.Dto
{

    public class TokenBalance
    {
        [JsonProperty("mint")]
        public string Mint;
        
        [JsonProperty("owner")]
        public string Owner;

        [JsonProperty("uiTokenAmount")]
        public UiTokenAmount UiTokenAmount;

        public override string ToString()
        {
            return $"Mint: {Mint}, ownder: {Owner}, amount: {UiTokenAmount}";
        }
    }

    public class UiTokenAmount
    {
        [JsonProperty("amount")]
        public int Amount;
        
        [JsonProperty("decimals")]
        public int Decimals;
        
        [JsonProperty("uiAmount")]
        public float UiAmount;
        
        [JsonProperty("uiAmountString")]
        public string UiAmountStr;

        public override string ToString()
        {
            return $"Amount: {Amount}, decimals: {Decimals}, ui amount: {UiAmountStr}";
        }
    }

    public class TxMeta
    {
        [JsonProperty("postTokenBalances")]
        public TokenBalance[] PostTokenBalances;

        public override string ToString()
        {
            var res = string.Empty;

            foreach (var postTokenBalance in PostTokenBalances)
            {
                res += $"{postTokenBalance}\n";
            }
            
            return res;
        }
    }
    
    
    public class GetTransactionResponse
    {
        [JsonProperty("meta")]
        public TxMeta Meta;

        public override string ToString()
        {
            return $"Meta: {Meta}";
        }
    }
}