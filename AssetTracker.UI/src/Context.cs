using System;
using System.Collections.Generic;
using System.Linq;

namespace SCLI.Core
{
    /// <summary>
    /// A context is the analog of a "window" or "menu" in a GUI. It keeps track of its own commands that are valid
    /// in that context, its own indentation level, prompt symbol, etc. Intended to be subclassed for more fine-grained
    /// control of the UI, but simple UIs can also be implemented with the base class and some custom commands.
    /// </summary>
    public class Context
    {
        public string PromptSymbol { get; set; }
        public Dictionary<string, CommandHandle> Commands { get; private set; }

        /// <summary>
        /// The indentation level of all subsequent output. One indentation corresponds to
        /// one tab character (\t) for simplicity.
        /// </summary>
        public uint IndentationLevel { get; private set; }
        //public char[] Separators { get; set; } = new char [] { ' ', '\t', '/' };
        
        /// <summary>
        /// The interface to use when putting messages to the screen.
        /// </summary>
        protected IConsoleOutput OutputHandle { get; set; }

        protected IUserInput InputHandle { get; set; }

        protected string OnScreenClearMessage { get; set; }

        /// <summary>
        /// Creates a new context that writes output to the given IConsoleOutput object.
        /// </summary>
        /// <param name="clearMessage">A default message shown every time the screen is cleared.</param>
        public Context(IConsoleOutput console, IUserInput inputHandle, string clearMessage = "")
        {
            Commands = new();
            PromptSymbol = "-> ";
            IndentationLevel = 0;
            OnScreenClearMessage = clearMessage ?? "";

            OutputHandle = console;
            InputHandle = inputHandle;
        }

        /// <summary>
        /// Add a new command to the current context.
        /// </summary>
        /// <param name="commandName">String describing which command was use to invoke the method.</param>
        /// <param name="commandHandle">The parameters to the command, if any.</param>
        public void AddCommand(string commandName, CommandHandle commandHandle)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException("AddCommand: parameter commandName null or empty");
            }
            else if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ArgumentNullException("AddCommand: parameter commandName name consists of whitespace");
            }
            else if (commandHandle == null)
            {
                throw new ArgumentNullException("AddCommand: null parameter commandHandle");
            }
            else
            {
                Commands.Add(commandName, commandHandle);
            }
        }

        /// <summary>
        /// Increases the indentaion level of all subsequent output with one. The implementation does not 
        /// limit how many indentation levels are possible - only the size of the screen and the size of an uint.
        /// </summary>
        public void IncreaseIndentation()
        {
            IndentationLevel++;
        }

        /// <summary>
        /// Decreases the indentaion level of all subsequent output with one. If the indentation level is already
        /// 0, a InvalidOperationException will be thrown.
        /// </summary>
        public void DecreaseIndentation()
        {
            if (IndentationLevel == 0)
            {
                throw new InvalidOperationException("Error: Attempted to decrease the indentation when indentation level is 0.");
            }
            else
            {
                IndentationLevel--;
            }
        }

        /// <summary>
        /// This method is automatically called whenever the screen is cleared. Consumers of the UI that wish
        /// to display their own context sensitive message after clearing teh screen should override this method.
        /// </summary>
        public virtual string OnClearMessage()
        {
            return OnScreenClearMessage;
        }
    }
}
