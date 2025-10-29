namespace BugStore.Exception.ProjectException;
public class BugStoreException : System.Exception
{
    public IList<string> ErrorMessages { get; set; } = new List<string>();

    public BugStoreException(IList<string> errorMessages) : base()
    {
        ErrorMessages = errorMessages;
    }

    public BugStoreException(string errorMessage)
    {
        ErrorMessages.Add(errorMessage);
    }
}
