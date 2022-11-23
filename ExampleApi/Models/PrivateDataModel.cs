using SecurityBlanket.Interfaces;

namespace ExampleApi.Models
{
    public class PrivateDataModel : ICustomSecurity
    {
        public int AccountId { get; set; }
        public string Name { get { return "The name of the account"; } }

        public bool IsVisible(HttpContext context)
        {
            return context.Items["AccountId"] as Int32? == AccountId;
        }
    }
}
