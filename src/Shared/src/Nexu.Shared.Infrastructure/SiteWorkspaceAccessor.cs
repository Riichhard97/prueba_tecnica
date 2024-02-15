namespace Nexu.Shared.Infrastructure
{
    public class SiteWorkspaceAccessor : ISiteWorkspaceAccessor
    {
        private readonly string _id;

        public SiteWorkspaceAccessor(string workspaceId)
        {
            _id = workspaceId;
        }

        public string Get()
        {
            if (!string.IsNullOrEmpty(_id))
            {
                return _id;
            }

            throw new NoSiteWorkspaceException();
        }

        public string TryGet()
        {
            return _id;
        }
    }
}
