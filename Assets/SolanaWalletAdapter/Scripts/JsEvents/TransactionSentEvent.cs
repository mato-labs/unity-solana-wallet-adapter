namespace SolanaWalletAdapter.Scripts.JsEvents
{
    public class TransactionSentEvent: JsEvent<string, string>
    {
        public delegate void TransactionSentDelegate(string signature);
        public new event TransactionSentDelegate OnEvent;
        
        public TransactionSentEvent() : base("OnTransactionSentEvent")
        {
            base.OnEvent += (a, b) => OnEvent(b);
        }
    }
}