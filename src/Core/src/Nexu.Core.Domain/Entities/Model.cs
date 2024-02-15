namespace Nexu.Core.Domain.Entities
{
    public class Model //: BaseDomain
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int AveragePrice { get; set; }
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
