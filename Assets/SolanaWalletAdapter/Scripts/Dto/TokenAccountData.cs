using Newtonsoft.Json;

namespace SolanaWalletAdapter.Scripts.Dto
{
    public class TokenAccountData
    {
        public string PublicKey;
        public string Owner;
        public int RentEpoch;
        public ulong Lamports;
        public bool IsExecutable;
        public int Space;
        public string Mint;
        public ulong Amount;
        public int Decimals;
        public float UiAmount;
    }
}