using System.Collections.Generic;

namespace Nexu.Core.Domain.Entities
{
    public class Brand //: BaseDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Model> Models { get; set; }
    }
}
