using Microsoft.Identity.Client;
using SecurityBlanket.Interfaces;

namespace ExampleApi.Models
{
    public class CompoundDataModel : ICompoundSecurity, ICustomSecurity
    {
        public int AccountId { get; set; }
        public string Description { get { return "A compound object that has children that also implement security policies."; } }
        public List<PrivateDataModel> Items { get; set; } = new List<PrivateDataModel>();

        public IEnumerable<object> GetChildren()
        {
            return Items;
        }

        public bool IsVisible(HttpContext context)
        {
            return AccountId == (int?)context.Items["AccountId"];
        }
    }
}
