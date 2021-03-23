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

    public class SCLIMain : IConsoleOutput, IConsoleInput
    {
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
                string[] input = GetInputFromUser();
                if (input == null)
                {
                    Console.WriteLine("Error: Input field must not be empty");
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
                else
                {
                    PutMessage($"No command named '{command}' is available.", Color.YELLOW);
                    continue;
                }
            }
        }

        public string[] GetInputFromUser()
        {
            PutMessage(CurrentContext.PromptSymbl, Color.WHITE, false);
            string[] output = Console.ReadLine().Split(CurrentContext.IgnoreChars);

            bool IsEmpty = true;
            foreach (string str in output)
            {
                if (!(string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str)))
                    IsEmpty = false;
            }

            return IsEmpty ? null : output;
        }

        public string ReadLine()
        {
            return Console.ReadLine();
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

            if(!c.Commands.ContainsKey("help"))
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
    }
}
