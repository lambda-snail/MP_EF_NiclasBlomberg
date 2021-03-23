namespace SCLI.Core
{
    public interface IConsoleOutput
    {
        /// <summary>
        /// Sets the color of messages presented to the user.
        /// </summary>
        public enum Color
        {
            GREEN,
            WHITE,
            YELLOW,
            RED
        };
        public void ClearScreen();

        public void PutMessage(string msg, Color level = Color.WHITE, bool newLine = true);
    }
}