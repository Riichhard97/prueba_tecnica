using System.Threading.Tasks;

namespace Nexu.Core.Persistence.Seeds
{
    public interface ISeed
    {
        public Task Init();
    }
}
