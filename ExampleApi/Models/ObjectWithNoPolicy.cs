namespace ExampleApi.Models
{
    public class ObjectWithNoPolicy
    {
        public string UnknownData { get { return "This data could be sensitive, but without a policy, we have no way to verify."; } }
    }
}
