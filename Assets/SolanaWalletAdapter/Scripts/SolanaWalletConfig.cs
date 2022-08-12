using UnityEngine;

namespace SolanaWalletAdapter.Scripts
{
    [CreateAssetMenu(fileName = "SolanaWalletConfig", menuName = "ScriptableObjects/SolanaWalletConfig", order = 1)]
    public class SolanaWalletConfig: ScriptableObject
    {
        private static SolanaWalletConfig _i;

        public static SolanaWalletConfig I
        {
            get
            {
                if (_i == null)
                {
                    _i = Resources.Load<SolanaWalletConfig>("SolanaWalletConfig");
                }
                
                return _i;
            }
        }

        public string RpcUrl;
        
        public SolanaWalletData[] WalletData;

        public string EditorTestWallet;

        public string EditorNftMetadataUrl;

        public string[] OwnerAddresses;
    }
}