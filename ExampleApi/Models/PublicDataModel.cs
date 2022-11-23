using SecurityBlanket.Interfaces;

namespace ExampleApi.Models
{
    /// <summary>
    /// Represents public data that isn't sensitive
    /// </summary>
    public class PublicDataModel : INoSecurity
    {
        public string News { get { return "Here is where we would place today's news"; } }
    }
}
