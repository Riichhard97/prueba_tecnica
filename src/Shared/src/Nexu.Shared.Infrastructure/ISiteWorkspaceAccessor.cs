namespace Nexu.Shared.Infrastructure
{
    public interface ISiteWorkspaceAccessor
    {
        string Get();
        string TryGet();
    }
}
