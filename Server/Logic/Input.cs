using System;
using System.Linq;
using Extensions;
using System.Collections.Generic;

namespace Server.Logic {
    public static class Input {

        private static Dictionary<String, Action<Object[]>> Commands = new Dictionary<String, Action<Object[]>>() {
            { "close",  Shutdown}, { "exit",  Shutdown}, { "shutdown",  Shutdown},
            { "help", Help }
        };
        private static Dictionary<String, String> HelpList = new Dictionary<String, String>() {
            { "close", "Shuts down the server and saves all currently loaded information to disk." },
            { "exit", "Shuts down the server and saves all currently loaded information to disk." },
            { "shutdown", "Shuts down the server and saves all currently loaded information to disk." },
            { "help", "Provides help for every command available in this program.\n- Use 'help' to get a list of available commands.\n- Use 'help command' to get more detailed information about a command." },
        };

        public static void Process(String input) {
            // Parse our input command and pass it on to the appropriate method.
            var data = input.Split(' ');
            Object[] arguments;
            if (data.Length > 1) {
                arguments = new Object[data.Length - 1];
                for (var i = 0; i < data.Length - 1; i++) {
                    arguments[i] = data[i + 1];
                }
            } else {
                arguments = new Object[0];
            }
            Commands.TryGet(data[0], (o) => { Logger.Write("Unknown Command."); })(arguments);
        }

        private static void Shutdown(Object[] args) {
            Logger.Write("Received Shutdown Command.");
            Program.Running = false;
        }

        private static void Help(Object[] args) {
            // Show a generic help when there's no arguments provided.
            // Otherwise show the command's help where available.
            if (args.Length > 0) {
                Logger.Write(HelpList.TryGet((String)args[0], "Unknown Command."));
            } else {
                Logger.Write(String.Format("All available commands follow. Please use 'help command' to get a more detailed overview.\n{0}", (from c in Commands orderby c.Key select c.Key).ToArray().Aggregate((i,j) => i + ", " + j )));
            }
        }
    }
}
