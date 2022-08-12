using SolanaWalletAdapter.Scripts;
using UnityEngine;

namespace SolanaMintAdapter.Scripts.Events
{
    public class CandyMachineInfoReceivedEvent: JsEvent<CandyMachineInfo>
    {
        public CandyMachineInfoReceivedEvent() : base("OnCandyMachineInfoEvent")
        {
            OnEvent += s =>
            {
                Debug.Log("Got: str");
                Debug.Log(s);
            };
        }
    }
}