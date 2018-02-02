
namespace sql_profiler
{

    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/707e9ae1-a53f-4918-8ac4-62a1eddb3c4a/detecting-console-application-exit-in-c?forum=csharpgeneral
    internal static class WindowsFix
    {

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine_t(CtrlTypes CtrlType);

        public static void AddConsoleHandler(HandlerRoutine_t on)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows
            );

            if(isWindows)
                SetConsoleCtrlHandler(on, true);
        }


        private static bool isclosing = false;

 


        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine_t Handler, bool Add);

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            System.Console.WriteLine(isclosing);
            
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("CTRL+C received!");
                    break;
                case CtrlTypes.CTRL_BREAK_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("CTRL+BREAK received!");
                    break;
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("Program being closed!");
                    break;
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("User is logging off!");
                    break;
            }

            return true;
        }


    }


}
