using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolanaWalletAdapter.Scripts.Dto;
using SolanaWalletAdapter.Scripts.JsEvents;
using UnityEngine;

namespace SolanaWalletAdapter.Scripts
{
    public class FetchMetadataTask
    {
        public TaskCompletionSource<List<NftMetadata>> TaskSource { get; } = new TaskCompletionSource<List<NftMetadata>>();

        private readonly List<string> _mintIds;
        private readonly List<NftMetadata> _res = new List<NftMetadata>();

        public FetchMetadataTask(List<string> mintIds)
        {
            _mintIds = mintIds;

            if (mintIds.Count == 0)
            {
                TaskSource.SetResult(_res);
            }
            else
            {
                JsEventDispatcher.GetEvent<MintMetadataLoadedEvent>().OnEvent += OnMintMetadataReceived;
                JsEventDispatcher.GetEvent<MintMetadataFailedToLoadEvent>().OnEvent += OnMintMetadataRequestFiled;

                foreach (var mintId in mintIds)
                {
                    WalletAdapterController.RequestMintMetadata(mintId);
                }
            }
        }

        private void OnMintMetadataReceived(GetMetadataResult res)
        {
            FetchMetadata(res);
        }

        private async void FetchMetadata(GetMetadataResult res)
        {
            try
            {
                var meta = await SolanaApi.GetJson<NftMetadata>(res.MetadataUrl);
                meta.Mint = res.Mint;

                var tex = await SolanaApi.GetTexture(meta.ImageUrl);
                if (tex != null)
                {
                    // var scaledTex = Utils.ScaleTexture(tex, 500, 500);
                    meta.Sprite = Utils.TexToSprite(tex);
                }
                else
                {
                    Debug.LogError("FAILED TO GET TEXTURE");
                }

                _res.Add(meta);
                
                UpdateProgress(res.Mint);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get metadata: {e.Message}");
                
                _res.Add(new NftMetadata()
                {
                    Mint = res.Mint
                });
                
                UpdateProgress(res.Mint);
            }
        }

        private void UpdateProgress(string mint)
        {
            _mintIds.Remove(mint);

            if (_mintIds.Count == 0)
            {
                JsEventDispatcher.GetEvent<MintMetadataLoadedEvent>().OnEvent -= OnMintMetadataReceived;
                JsEventDispatcher.GetEvent<MintMetadataFailedToLoadEvent>().OnEvent -= OnMintMetadataRequestFiled;
                
                TaskSource.SetResult(_res);
            }
        }

        private void OnMintMetadataRequestFiled(string mint)
        {
            _res.Add(new NftMetadata()
            {
                Mint = mint
            });
            
            UpdateProgress(mint);
        }
    }
}