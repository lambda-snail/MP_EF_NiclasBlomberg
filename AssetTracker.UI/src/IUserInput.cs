namespace SCLI.Core
{
    /// <summary>
    /// An interface for objects that read and parse input from the user.
    /// </summary>
    public interface IUserInput
    {
        string GetEditableInputWithDefaultText(string defaultString = "");
        string GetRawStringInputFromUser();
    }
}