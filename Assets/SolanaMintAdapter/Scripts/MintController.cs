using System.Runtime.InteropServices;

namespace SolanaMintAdapter.Scripts
{
    public static class MintController
    {
        [DllImport("__Internal")]
        private static extern string MintInternal(string cmId);
        
        [DllImport("__Internal")]
        private static extern string RequestCandyMachineDataInternal(string cmId);


        public static void RequestCandyMachineData(string cmId)
        {
            RequestCandyMachineDataInternal(cmId);
        }
        
        public static void Mint(string cmId)
        {
            MintInternal(cmId);
        }
    }
}