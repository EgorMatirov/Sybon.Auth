namespace Sybon.Auth
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}