using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolanaMintAdapter.Scripts.Events;
using SolanaWalletAdapter.Scripts;
using SolanaWalletAdapter.Scripts.JsEvents;
using TMPro;
using UnityEngine;


namespace SolanaWalletAdapter.Demo.Scripts
{
    public class UiController: MonoBehaviour
    {
        public GameObject WalletButtonPrefab;
        public RectTransform WalletListScrollTransform;

        public GameObject SelectWalletWindow;
        public GameObject LoadingWidnow;

        public TextMeshProUGUI WalletAddressLabel;

        public GameObject SignProgress;
        public GameObject SignDialog;
        public TextMeshProUGUI Signature;

        public TextMeshProUGUI SignatureVerificationResult;

        public GameObject NftViewPrefab;
        public RectTransform NftListScrollTransform;
        
        private string _lastSignature;
        private string _lastAddr;
        private string _lastMsg;


        private readonly List<string> _loadedNfts = new List<string>();

        private void Start()
        {
            SelectWalletWindow.SetActive(true);
            SignProgress.SetActive(false);
            LoadingWidnow.SetActive(false);
            SignDialog.SetActive(false);

            JsEventDispatcher.GetEvent<WalletErrorEvent>().OnEvent += () =>
            {
                WalletAddressLabel.text = string.Empty;
                
                LoadingWidnow.SetActive(false);
                SelectWalletWindow.SetActive(true);
            };

            JsEventDispatcher.GetEvent<WalletConnectedEvent>().OnEvent += (walletname, pubkey) =>
            {
                WalletAddressLabel.text = pubkey;
                
                LoadNFTs(pubkey);
            };

            JsEventDispatcher.GetEvent<WalletMessageSignedEvent>().OnEvent += (walletname, pubkey, msg, signature) =>
            {
                Debug.Assert(pubkey == WalletAdapterController.WalletAddress);
                
                SignProgress.SetActive(false);
                Signature.text = $"Signature: {signature}";
                SignDialog.SetActive(true);

                _lastSignature = signature;
                _lastAddr = pubkey;
                _lastMsg = msg;
            };

            JsEventDispatcher.GetEvent<MessageSignErrorEvent>().OnEvent += () =>
            {
                SignProgress.SetActive(false);
            };

            var addedIds = new HashSet<string>();
            
            Debug.Log($"Len: {WalletAdapterController.WalletConfig.walletSpecs.Length}");
            
            foreach (var walletSpec in WalletAdapterController.WalletConfig.walletSpecs)
            {
                if (addedIds.Contains(walletSpec.id))
                {
                    continue;
                }
                
                var g = Instantiate(WalletButtonPrefab);
                var walletView = g.GetComponent<WalletButtonView>();
                walletView.WalletNameLabel.text = walletSpec.id;
                walletView.Id = walletSpec.id;
                walletView.DetectedLabel.SetActive(walletSpec.installed);
                walletView.CanSign.SetActive(walletSpec.canSign);
                
                walletView.OnSelectedAction = walletId =>
                {
                    SelectWalletWindow.SetActive(false);
                    LoadingWidnow.SetActive(true);
                    
                    WalletAdapterController.ConnectWalletById(walletSpec.id);
                };

                var rt = g.GetComponent<RectTransform>();
                rt.SetParent(WalletListScrollTransform, false);
            }


            JsEventDispatcher.GetEvent<CandyMachineInfoReceivedEvent>().OnEvent += info =>
            {
            };

            JsEventDispatcher.GetEvent<MintFailedEvent>().OnEvent += () =>
            {
            };

            JsEventDispatcher.GetEvent<MintSuccessEvent>().OnEvent += tx =>
            {
            };
        }


        private IEnumerator CheckTxRoutine(string txid)
        {
            var startDate = DateTime.Now;
            
            while (true)
            {
                var resTask = SolanaApi.IsTransactionFinalized(txid);

                while (!resTask.IsCompleted) //|| (DateTime.Now - startDate).TotalSeconds > 60
                {
                    yield return new WaitForSeconds(0.1f);
                }
                
                if (resTask.Result != null)
                {
                    Debug.Log(resTask.Result);

                    foreach (var postTokenBalance in resTask.Result.Meta.PostTokenBalances)
                    {
                        if (SolanaWalletConfig.I.OwnerAddresses.Contains(postTokenBalance.Owner))
                        {
                            Debug.Log("New mint: " + postTokenBalance.Mint);

                            GetNft(postTokenBalance.Mint);
                        }
                    }
                    
                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }
            
            yield return new WaitForSeconds(2f);
            
            var oldNfts = new List<string>(_loadedNfts);
            
            LoadNFTs(WalletAddressLabel.text);

            var diff = _loadedNfts.Except(oldNfts).ToList();
        }

        private async void GetNft(string mint)
        {
            var r = new List<string>();
            r.Add(mint);
            
            var fetchMetaTask = new FetchMetadataTask(r);
            var meta = await fetchMetaTask.TaskSource.Task;
        }

        private void ClearNfts()
        {
            var objs = new List<GameObject>();
            for (int i = 0; i < NftListScrollTransform.transform.childCount; i++)
            {
                objs.Add(NftListScrollTransform.transform.GetChild(i).gameObject);
            }

            foreach (var o in objs)
            {
                Destroy(o);
            }
        }

        private async void LoadNFTs(string wallet)
        {
            var r = await SolanaApi.GetWalletMints(wallet);
            
            var fetchMetaTask = new FetchMetadataTask(r);
            var meta = await fetchMetaTask.TaskSource.Task;
            
            ClearNfts();
            _loadedNfts.Clear();

            foreach (var nftMetadata in meta)
            {
                if (nftMetadata.Sprite == null || string.IsNullOrEmpty(nftMetadata.Name))
                {
                    continue;
                }
                
                var g = Instantiate(NftViewPrefab);
                var view = g.GetComponent<NftView>();
                view.Init(nftMetadata);
                
                var rt = g.GetComponent<RectTransform>();
                rt.SetParent(NftListScrollTransform, false);
                
                _loadedNfts.Add(nftMetadata.Mint);
            }
            
            LoadingWidnow.SetActive(false);
            SelectWalletWindow.SetActive(false);

        }

        public static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public void DisconnectWallet()
        {
            WalletAddressLabel.text = string.Empty;
            SelectWalletWindow.SetActive(true);

            ClearNfts();
            
            WalletAdapterController.DisconnectWallet();
        }

        public void SignBtn()
        {
            SignProgress.SetActive(true);
            
            SignatureVerificationResult.gameObject.SetActive(false);
            
            WalletAdapterController.SignMessage("Hello, world!");
        }

        public void CloseSignDialog()
        {
            SignDialog.SetActive(false);
        }

        public void VerifySignature()
        {
            var verified = WalletAdapterController.VerifySignature(_lastAddr, _lastMsg, _lastSignature);
            SignatureVerificationResult.text = $"Verified? {verified}";
            SignatureVerificationResult.gameObject.SetActive(true);
        }

        public void MintCommand()
        {
            // todo: implement
        }

    }
}