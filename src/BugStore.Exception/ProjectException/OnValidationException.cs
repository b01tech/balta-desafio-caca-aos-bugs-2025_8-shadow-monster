namespace BugStore.Exception.ProjectException;
public class OnValidationException : BugStoreException
{
    public OnValidationException(IList<string> errorMessages) : base(errorMessages)
    {
    }
    public OnValidationException(string errorMessage) : base(errorMessage)
    {
    }
}
