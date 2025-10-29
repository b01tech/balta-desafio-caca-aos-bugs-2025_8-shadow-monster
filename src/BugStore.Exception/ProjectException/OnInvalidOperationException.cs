namespace BugStore.Exception.ProjectException;
public class OnInvalidOperationException : BugStoreException
{
    public OnInvalidOperationException(IList<string> errorMessages) : base(errorMessages)
    {
    }
    public OnInvalidOperationException(string errorMessage) : base(errorMessage)
    {
    }
}
