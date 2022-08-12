namespace SolanaWalletAdapter.Scripts.Dto
{
    public class AccountBalance
    {
        public readonly ulong Lamports;
        public readonly float Sol;

        public AccountBalance(ulong l)
        {
            Lamports = l;
            Sol = (float) (Lamports / (double) 1000000000);
        }

        public override string ToString()
        {
            return $"Balance: {Sol} SOL, {Lamports} lamports";
        }
    }
}