namespace ConcurrentTrieMap
{
    //public record CtrieOptions(int ConcurrencyLevel, int InitialChildNodeCapacity);

    public class CtrieOptions
    {
        public readonly int InitialChildNodeCapacity;

        public CtrieOptions(int initialChildNodeCapacity)
        {
            InitialChildNodeCapacity = initialChildNodeCapacity;
        }
    }
}