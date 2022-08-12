using System;
using Newtonsoft.Json;

namespace SolanaMintAdapter.Scripts
{
    public class CandyMachineInfo
    {
        [JsonProperty("total")]
        public int Total { get; private set; }
        
        [JsonProperty("minted")]
        public int Minted { get; private set; }
        
        [JsonProperty("remaining")]
        public int Remaining { get; private set; }
        
        [JsonProperty("price")]
        public float Price { get; private set; }

        [JsonProperty("isSoldOut")]
        public bool IsSoldOut { get; private set; }

        [JsonProperty("isPresale")]
        public bool IsPresale { get; private set; }

        [JsonProperty("goLiveDate")]
        public DateTime GoLiveDate { get; private set; }


        public override string ToString()
        {
            return $"Cm: total: {Total}, minted: {Minted}, price: {Price} SOL, mint start: {GoLiveDate}, presale? {IsPresale}";
        }
    }
}