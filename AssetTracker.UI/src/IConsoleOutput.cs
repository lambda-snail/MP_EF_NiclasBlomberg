namespace SCLI.Core
{
    public interface IConsoleOutput
    {
        /// <summary>
        /// Sets the level of severity of messages presented to the user. These are
        /// put on the screen using different colors:
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