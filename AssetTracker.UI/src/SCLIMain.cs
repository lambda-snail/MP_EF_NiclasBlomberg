using System;
using System.Collections.Generic;
using System.Linq;


using static SCLI.Core.IConsoleOutput;

namespace SCLI.Core
{
    /// <summary>
    /// Consumers of the interface must use methods of the type specified by this delegate to listen to
    /// commands input by the user.
    /// </summary>
    /// <param name="command">The name of the command</param>
    /// <param name="args">The parameters, if any. Note that the interface does not know which parameters a certain
    /// command requires, so if the user inputs superfluous arameters it is up to the implementation to decide whether to
    /// ignore them.</param>
    /// <returns>True if the operation was successfully completed.</returns>
    public delegate bool CommandHandle(string command, string[] args);

    public class SCLIMain : IConsoleOutput, IAutoCompleteHandler, IUserInput
    {
        public char[] Separators { get; set; } = new char[] { ' ', '\t', '/' };
        public Context CurrentContext
        {
            get
            {
                if (ContextStack.Count == 0)
                {
                    return null;
                }
                else
                {
                    return ContextStack.Peek();
                }
            }
            private set { throw new NotImplementedException("Setting of CurrentContext is not supported."); }
        }
        private Stack<Context> ContextStack { get; set; }

        private bool ExitFlag { get; set; }

        public SCLIMain()
        {
            ContextStack = new();
        }

        /// <summary>
        /// The main loop starts here.
        /// </summary>
        public void Run()
        {
            if (CurrentContext == null)
            {
                throw new InvalidProgramException("Attempted to start UI without loading a context.");
            }

            while (!ExitFlag)
            {
                string[] input = GetCommandFromUser();
                if (input == null)
                {
                    //Console.WriteLine("Error: Input field must not be empty");
                    continue;
                }

                string command = input[0];

                if (CurrentContext.Commands.ContainsKey(command))
                {
                    string[] args =
                        input.Skip(1) // Skip first item - the command name
                        .Where(s => !(string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s)))
                        .ToArray();

                    //Commands[command](command, args);
                    CurrentContext.Commands[command](command, args);
                }
            }
        }

        /// <summary>
        /// Reads one line of input from the user. This is a wrapper around Console.ReadLine().
        /// </summary>
        public string GetRawStringInputFromUser()
        {
            return ReadLine.Read(CurrentContext.PromptSymbol);
        }

        /// <summary>
        /// Gets one line of input from the user, splits the line by whitespace
        /// and returns the resulting tokens in a string array. This is intended as
        /// a simple parser for commands.
        /// 
        /// To get the entire string including whitespace, use GetRawStringInputFromUser.
        /// 
        /// To get normal input from the user, use GetEditableInputWithDefaultText.
        /// </summary>
        public string[] GetCommandFromUser(string defaultString = "")
        {
            string[] input = ReadLine.Read(CurrentContext.PromptSymbol).Split(Separators);

            bool IsEmpty = true;
            foreach (string str in input)
            {
                if (!string.IsNullOrEmpty(str))
                    IsEmpty = false;
            }

            return IsEmpty ? null : input;
        }

        /// <summary>
        /// Obtains a string from the user, possibly empty. The user can edit the input by removing characters
        /// with backspace before submitting with enter.
        /// </summary>
        /// <param name="defaultString">A string of default input that the user can then edit before submitting.</param>
        public string GetEditableInputWithDefaultText(string defaultString = "")
        {
            int editablePositionStart = CurrentContext.PromptSymbol.Length;
            PutMessage(CurrentContext.PromptSymbol + defaultString, newLine: false);
            ConsoleKeyInfo info;

            List<char> editableCharacterBuffer = new List<char>();
            if (string.IsNullOrEmpty(defaultString) == false)
            {
                editableCharacterBuffer.AddRange(defaultString.ToCharArray());
            }

            while (true)
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace && Console.CursorLeft > editablePositionStart)
                {
                    editableCharacterBuffer.RemoveAt(editableCharacterBuffer.Count - 1);
                    Console.CursorLeft -= 1;
                    Console.Write(' ');
                    Console.CursorLeft -= 1;

                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    Console.Write(Environment.NewLine); break;
                }
                else if (char.IsLetterOrDigit(info.KeyChar) ||
                         char.IsWhiteSpace(info.KeyChar) ||
                         char.IsPunctuation(info.KeyChar) ||
                         char.IsSymbol(info.KeyChar))
                {
                    Console.Write(info.KeyChar);
                    editableCharacterBuffer.Add(info.KeyChar);
                }
            }
            return new string(editableCharacterBuffer.ToArray());
        }

        /// <summary>
        /// Add a new Context and change the current Context to the new context. Contexts are stored in a 
        /// stack, so consumers of the UI can easily revert to the previous Context with a call to PopContext().
        /// This method also enforces the commands 'help' and 'clear' on the context.
        /// </summary>
        public void PushContext(Context c)
        {
            if (c == null)
            {
                throw new ArgumentNullException("Error: Attempted to push null onto the Context stack.");
            }

            ContextStack.Push(c);

            if (!c.Commands.ContainsKey("help"))
                c.AddCommand("help", HelpCommand);

            if (!c.Commands.ContainsKey("clear"))
                c.AddCommand("clear", ClearScreenCommand);

            if (!c.Commands.ContainsKey("exit"))
                c.AddCommand("exit", ExitCommand);
        }

        /// <summary>
        /// Attempt to go up one level in the Context stack. There must always be one Context active, so
        /// this method does nothing if there is only one element on the stack.
        /// </summary>
        public void PopContext()
        {
            if (ContextStack.Count > 1)
            {
                ContextStack.Pop();
            }
        }

        /// <summary>
        /// Put a message to the screen with the given level of severity.
        /// </summary>
        /// /// <param name="newLine">If true, the message will be followed by a newline character. Defaults to true.</param>
        public void PutMessage(string msg, Color level = Color.WHITE, bool newLine = true)
        {
            switch (level)
            {
                case Color.GREEN:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Color.WHITE: // Do nothing - default color
                    break;
                case Color.YELLOW:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Color.RED:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:    // If we get here then something is wrong
                    throw new NotImplementedException("PutMessage: Unknown message level");
            }

            for (int i = 0; i < CurrentContext.IndentationLevel; i++)
            {
                Console.Write('\t');
            }

            if (newLine)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.Write(msg);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// This method is responsible for making the tab-completion work. When tab is pressed, this
        /// method retreives a list of suggestions based on the current context.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string[] GetSuggestions(string text, int index)
        {
            return CurrentContext.Commands.Keys.Where(c => c.ToLower().StartsWith(text.ToLower())).ToArray();
        }

        public void NewLine()
        {
            Console.WriteLine();
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        /// <summary>
        /// Convenience method to increase the indentation of the current Context.
        /// </summary>
        public void IncreaseIndentation()
        {
            CurrentContext.IncreaseIndentation();
        }

        /// <summary>
        /// Convenience method to decrease the indentation of the current Context.
        /// </summary>
        public void DecreaseIndentation()
        {
            CurrentContext.DecreaseIndentation();
        }

        /// <summary>
        /// Implements tab-completion logic.
        /// </summary>
        private void OnTabPressed()
        {
            //Console.In.ReadLine();
            PutMessage("Tab was pressed!");
        }

        private bool HelpCommand(string commandName, string[] args)
        {
            int order = 1;
            foreach (string cmd in CurrentContext.Commands.Keys)
            {
                if (cmd != commandName)
                {
                    PutMessage($"{order++}.".PadRight(4) + cmd);
                }
            }
            return true;
        }

        private bool ClearScreenCommand(string commandName, string[] args)
        {
            ClearScreen();
            PutMessage(CurrentContext.OnClearMessage());
            return true;
        }

        private bool ExitCommand(string commandName, string[] args)
        {
            ExitFlag = true;
            return true;
        }

        /// <summary>
        /// Turns off autocompletion for the current session.
        /// </summary>
        public void StopAutoCompletion()
        {
            ReadLine.AutoCompletionHandler = null;
        }

        /// <summary>
        /// Turns on autocompletion for the current session.
        /// </summary>
        public void StartAutoCompletion()
        {
            ReadLine.AutoCompletionHandler = this;
        }
    }
}
