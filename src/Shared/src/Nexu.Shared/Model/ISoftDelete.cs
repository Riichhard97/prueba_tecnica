namespace Nexu.Shared.Model
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; }

        void SetDeleted();
    }
}
