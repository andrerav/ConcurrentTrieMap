namespace ConcurrentTrieMap
{
    //public record CtrieOptions(int ConcurrencyLevel, int InitialChildNodeCapacity);

    public class CtrieOptions
    {
        public readonly int ConcurrencyLevel;
        public readonly int InitialChildNodeCapacity;

        public CtrieOptions(int concurrencyLevel, int initialChildNodeCapacity)
        {
            ConcurrencyLevel = concurrencyLevel;
            InitialChildNodeCapacity = initialChildNodeCapacity;
        }
    }
}