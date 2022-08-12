using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolanaWalletAdapter.Scripts.Dto;
using UnityEngine;
using UnityEngine.Networking;


namespace SolanaWalletAdapter.Scripts
{
    public static class SolanaApi
    {

        public const string SOLANA_TOKEN_PROGRAM = "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA";
        
        private static async Task<T> MakeRequest<T>(JsonRpcRequest requestData) where T: class
        {
            string requestJson = JsonConvert.SerializeObject(requestData);
            
            using (var request = new UnityWebRequest(SolanaWalletConfig.I.RpcUrl, "POST"))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestJson);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("MakeRequest(): connection error");
                    return null;
                }
                
                while (!request.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = request.downloadHandler.text;
                    
                    try
                    {
                        var response = JsonConvert.DeserializeObject<T>(data);
                        return response;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse data: request: {requestJson}");
                    }
                }
                else
                {
                    Debug.LogError("MakeRequest(): request error");
                    return null;
                }
                
                return null;
            }
        }
        
        public static async Task<T> GetJson<T>(string url) where T: class
        {
            using (var request = UnityWebRequest.Get(url))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("GetJson(): connection error");
                    return null;
                }
                
                while (!request.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = request.downloadHandler.text;
                    
                    try
                    {
                        var response = JsonConvert.DeserializeObject<T>(data);
                        return response;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"GetJson():Failed to parse data from url: {url}");
                    }
                }
                else
                {
                    Debug.LogError($"GetJson(): request error for url {url}");
                    return null;
                }
                
                return null;
            }
        }
        
        public static async Task<Texture2D> GetTexture(string url)
        {
            using (var request = UnityWebRequestTexture.GetTexture(url))
            {
                request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("GetJson(): connection error");
                    return null;
                }
                
                while (!request.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        return DownloadHandlerTexture.GetContent(request);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"GetJson():Failed to parse data from url: {url}");
                    }
                }
                else
                {
                    Debug.LogError($"GetJson(): request error for url {url}");
                    return null;
                }
                
                return null;
            }
        }


        public static async Task<GetAccountInfoResponse> GetAccountInfo(string base58pubKey)
        {
            var request = new JsonRpcRequest("getAccountInfo", new object[]{ base58pubKey, new Dictionary<string, string>(){{"encoding", "jsonParsed"}}});
            var resp = await MakeRequest<JsonRpcResponse<JsonRpcResponseValue<GetAccountInfoResponse>>>(request);

            if (resp == null)
            {
                return null;
            }

            return resp.Result.Value;
        }
        
        public static async Task<AccountBalance> GetBalance(string base58pubKey)
        {
            var request = new JsonRpcRequest("getBalance", new object[]{ base58pubKey});
            var resp = await MakeRequest<JsonRpcResponse<JsonRpcResponseValue<ulong>>>(request);

            if (resp == null)
            {
                return null;
            }

            return new AccountBalance(resp.Result.Value);
        }
        
        public static async Task<List<TokenAccountData>> GetTokenAccountsByOwnerWithProgramId(string base58pubKey, string programId)
        {
            var request = new JsonRpcRequest("getTokenAccountsByOwner", new object[]
            {
                base58pubKey, 
                new Dictionary<string, string>(){{"programId", programId}},
                new Dictionary<string, object>(){{"encoding", "jsonParsed"}}
            });
            
            
            var resp = await MakeRequest<JsonRpcResponse<JsonRpcResponseValue<object>>>(request);

            if (resp == null)
            {
                return null;
            }
            
            JArray jarr = resp.Result.Value as JArray;

            var res = new List<TokenAccountData>(jarr.Count);

            for (int i = 0; i < jarr.Count; i++)
            {
                var x = jarr[i];
                var data = new TokenAccountData();
                
                data.PublicKey = x["pubkey"].Value<string>();
                data.Owner = x["account"]["owner"].Value<string>();
                data.RentEpoch = x["account"]["rentEpoch"].Value<int>();
                data.Lamports = x["account"]["lamports"].Value<ulong>();
                data.IsExecutable = x["account"]["executable"].Value<bool>();;
                data.Space = x["account"]["data"]["space"].Value<int>();
                data.Mint = x["account"]["data"]["parsed"]["info"]["mint"].Value<string>();
                data.Amount = x["account"]["data"]["parsed"]["info"]["tokenAmount"]["amount"].Value<ulong>();
                data.Decimals = x["account"]["data"]["parsed"]["info"]["tokenAmount"]["decimals"].Value<int>();
                data.UiAmount = x["account"]["data"]["parsed"]["info"]["tokenAmount"]["uiAmountString"].Value<float>();
                
                res.Add(data);
            }
            
            return res;
        }

        public static async Task<List<string>> GetWalletMints(string walletPubKey)
        {
            var res = new List<string>();
            
            var accData = await GetTokenAccountsByOwnerWithProgramId(walletPubKey, SOLANA_TOKEN_PROGRAM);
            
            foreach (var accountData in accData)
            {
                if (accountData.Decimals == 0 && accountData.Amount == 1)
                {
                    res.Add(accountData.Mint);
                }
            }

            return res;
        }
        
        
        public static async Task<GetTransactionResponse> IsTransactionFinalized(string base58transactionSignature)
        {
            var request = new JsonRpcRequest("getTransaction", new object[]{ base58transactionSignature, new Dictionary<string, string>(){{"encoding", "jsonParsed"}}});
            var resp = await MakeRequest<JsonRpcResponse<GetTransactionResponse>>(request);
            
            return resp?.Result;
        }

        
        public static async Task<GetSignatureStatusesResponse> GetSignatureStatus(string tx)
        {
            var request = new JsonRpcRequest("getSignatureStatuses", new object[]{ new string[] {tx} });
            var resp = await MakeRequest<JsonRpcResponse<JsonRpcResponseValue<GetSignatureStatusesResponse>>>(request);

            return resp?.Result.Value;
        }
    }
}