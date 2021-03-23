
namespace SCLI.Core
{
    public interface IConsoleInput
    {
        /// <summary>
        /// Divides input into tokens separated by whitesace.
        /// </summary>
        public string[] GetInputFromUser();

        /// <summary>
        /// Returns one line of unprocessed input.
        /// </summary>
        public string ReadLine();
    }
}
