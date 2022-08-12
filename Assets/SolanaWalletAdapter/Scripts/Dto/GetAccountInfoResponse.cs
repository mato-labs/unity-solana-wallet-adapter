using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SolanaWalletAdapter.Scripts.Dto
{
    public class GetAccountInfoResponse
    {
        [JsonProperty("lamports")]
        public ulong Lamports { get; private set; }

        [JsonProperty("owner")]
        public string Owner { get; private set; }
        
        [JsonProperty("executable")]
        public string IsExecutable { get; private set; }
        
        [JsonProperty("rentEpoch")]
        public ulong RentEpoch { get; private set; }

        [JsonProperty("data")] 
        private object _data;

        [JsonIgnore]
        public float SolBalance => (float)(Lamports / (double)1000000000);

        [JsonIgnore]
        public bool IsDataArray => _data is JArray;

        [JsonIgnore]
        public string Encoding
        {
            get
            {
                if (!IsDataArray)
                {
                    throw new InvalidOperationException("Available only if data is array");
                }

                return (_data as JArray).ToObject<string[]>()[1];
            }
        }

        [JsonIgnore]
        public string EncodedData
        {
            get
            {
                if (!IsDataArray)
                {
                    throw new InvalidOperationException("Available only if data is array");
                }
                
                return (_data as JArray).ToObject<string[]>()[0];
            }
        }

        [JsonIgnore] public object RawData => _data;


        public override string ToString()
        {
            return $"Lamports: {Lamports}, owner: {Owner}, executable? {IsExecutable}, rent epoch: {RentEpoch}, sol: {SolBalance}";
        }
    }
}