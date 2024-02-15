using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure
{
    public class CurrentTokenAssesor : ICurrentTokenSetter, ICurrentTokenAccessor
    {
        string _token = string.Empty;

        public string GetToken()
        {
            return _token;
        }

        public void SetToken(string token)
        {
            _token = token;
        }
    }
}
