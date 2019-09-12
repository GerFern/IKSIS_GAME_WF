namespace EmptyTest.TStreamHandler
{
    public interface IReadOnlyDoubleDict<TKey, TValue>
    {
        TKey FindKey(TValue value);
        TValue FindValue(TKey key);
    }
}