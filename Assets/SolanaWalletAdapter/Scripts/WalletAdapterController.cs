using System;
using System.Runtime.InteropServices;
using System.Text;
using Chaos.NaCl;
using Merkator.BitCoin;
using SolanaWalletAdapter.Scripts.JsEvents;
using UnityEngine;

namespace SolanaWalletAdapter.Scripts
{
    
    [Serializable]
    public class WalletSpecs
    {
        public string id;
        public bool installed;
        public bool canSign;

        public override string ToString()
        {
            return $"{id}: installed? {installed}, can sign? {canSign}";
        }
    }

    [Serializable]
    public class WalletConfig
    {
        public WalletSpecs[] walletSpecs;
    }
    
    public class WalletAdapterController
    {
        [DllImport("__Internal")]
        private static extern string GetWalletConfigInternal();
        
        [DllImport("__Internal")]
        private static extern void InitInternal();
        
        [DllImport("__Internal")]
        private static extern void ConnectWalletByNameInternal(string walletId);
        
        [DllImport("__Internal")]
        private static extern void DisconnectWalletByNameInternal(string walletId);
        
        [DllImport("__Internal")]
        private static extern void SignMessageInternal(string walletId, string messageStr);

        [DllImport("__Internal")]
        private static extern string RequestMintMetadataInternal(string mint);
        
        [DllImport("__Internal")]
        private static extern void SendTransactionInternal(string walletId, string base64tx);
        
        
        private const string GameObjectName = "JsEventDispatcher";
        
 
        public static WalletConfig WalletConfig { get; private set; }

        private static string _connectedWalletId;
        public static string WalletAddress { get; private set; }

        public static bool IsConnected => !string.IsNullOrEmpty(_connectedWalletId);
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            // Create event dispatcher
            var go = new GameObject(GameObjectName);
            GameObject.DontDestroyOnLoad(go);
            var dispatcher = go.AddComponent<JsEventDispatcher>();


#if UNITY_EDITOR
            WalletConfig = new WalletConfig();
            var wallets = SolanaWalletConfig.I.WalletData;
            WalletConfig.walletSpecs = new WalletSpecs[wallets.Length];
            
            for (int i = 0; i < wallets.Length; i++)
            {
                var w = wallets[i];
                WalletConfig.walletSpecs[i] = new WalletSpecs()
                {
                    id = w.Id,
                    installed = true,
                    canSign = true
                };
            }
#endif
            
#if UNITY_WEBGL && !UNITY_EDITOR
            InitInternal();

            var configStr = GetWalletConfigInternal();
            WalletConfig = JsonUtility.FromJson<WalletConfig>(configStr);
#endif

            JsEventDispatcher.GetEvent<WalletConnectedEvent>().OnEvent += (walletname, pubkey) =>
            {
                _connectedWalletId = walletname;
                WalletAddress = pubkey;
            };

            JsEventDispatcher.GetEvent<WalletDisconnectedEvent>().OnEvent += walletname =>
            {
                Debug.Assert(walletname == _connectedWalletId);
                
                _connectedWalletId = string.Empty;
            };
        }
        
        public static void ConnectWalletById(string walletId)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("Wallet is already connected");
            }
            
#if UNITY_WEBGL && UNITY_EDITOR
            (JsEventDispatcher.GetEvent<WalletConnectedEvent>() as JsEvent<string, string>).OnEvent("phantom", SolanaWalletConfig.I.EditorTestWallet);
            return;
#endif
            
#if UNITY_WEBGL && !UNITY_EDITOR
            ConnectWalletByNameInternal(walletId);
            return;
#endif
        }

        public static void DisconnectWallet()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }
            
#if UNITY_WEBGL && UNITY_EDITOR
            (JsEventDispatcher.GetEvent<WalletDisconnectedEvent>() as JsEvent<string>).OnEvent("phantom");
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            DisconnectWalletByNameInternal(_connectedWalletId);
#endif
        }

        public static void SignMessage(string msg)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Wallet is not connected");
            }

            
#if UNITY_WEBGL && UNITY_EDITOR
            (JsEventDispatcher.GetEvent<WalletMessageSignedEvent>() as JsEvent<string, string, string, string>).OnEvent("phantom", SolanaWalletConfig.I.EditorTestWallet, "Test sign message", "test_signature");
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            SignMessageInternal(_connectedWalletId, msg);
#endif
        }

        public static bool VerifySignature(string base58publicKey, string messageStr, string base64signature)
        {
#if UNITY_EDITOR
            return true;
#endif 
            var signatureBytes = System.Convert.FromBase64String(base64signature);
            var pubKeyBytes = Base58Encoding.Decode(base58publicKey);
            var msgBytes = Encoding.ASCII.GetBytes(messageStr);

            return Ed25519.Verify(signatureBytes, msgBytes, pubKeyBytes);
        }

        public static void RequestMintMetadata(string mint)
        {
#if UNITY_WEBGL && UNITY_EDITOR
            (JsEventDispatcher.GetEvent<MintMetadataLoadedEvent>() as JsEvent<string, string, string>).OnEvent("Test NFЕ", SolanaWalletConfig.I.EditorNftMetadataUrl, mint);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            RequestMintMetadataInternal(mint);
#endif
        }

        public static void SendTransaction(string base64tx)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SendTransactionInternal(_connectedWalletId, base64tx);
#endif
        }

    }
}