using System;

namespace Server.Logic {
    public static class Logger {

        public static void Write(String data) {
            Console.WriteLine(String.Format("[{0}] {1}", DateTime.UtcNow.ToString("dd-MM HH:mm:ss"), data));
        }

    }
}
