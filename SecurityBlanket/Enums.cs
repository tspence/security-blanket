namespace SecurityBlanket
{
    public enum SecurityAction
    {
        Create,
        Retrieve,
        Update,
        Delete,
    }

    public enum FailureType
    {
        MissingPolicy,
        FailedPolicy,
    }
}
