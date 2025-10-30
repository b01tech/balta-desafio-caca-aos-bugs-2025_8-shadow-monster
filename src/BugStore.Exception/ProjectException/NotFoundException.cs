namespace BugStore.Exception.ProjectException
{
    public class NotFoundException: BugStoreException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
