
namespace sql_profiler
{


    public class Program
    {
        
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/create-and-run-traces-using-transact-sql-stored-procedures
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/create-a-trace-transact-sql
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/set-a-trace-filter-transact-sql
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/modify-an-existing-trace-transact-sql
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/view-a-saved-trace-transact-sql
        // https://docs.microsoft.com/en-us/sql/relational-databases/sql-trace/delete-a-trace-transact-sql
        
        // Create a trace by using sp_trace_create.
        // Add events with sp_trace_setevent.
        // (Optional) Set a filter with sp_trace_setfilter.
        // Start the trace with sp_trace_setstatus.
        // Stop the trace with sp_trace_setstatus.
        // Close the trace with sp_trace_setstatus.
        
        // https://stackoverflow.com/questions/637013/how-do-i-find-running-traces-in-sql-server
        // SELECT * FROM sys.traces
        // SELECT * FROM::fn_trace_getinfo(trace_id)
        // SELECT * FROM::fn_trace_getfilterinfo(trace_id)
        
        // A trace must be stopped first before it can be closed.
        // EXEC sp_trace_setstatus [ @traceid = ] trace_id , [ @status = ] status  

        // EXEC sp_trace_setstatus 2, 0
        // EXEC sp_trace_setstatus 2, 2
        
        // Execute sp_trace_setstatus by specifying @status = 0 to stop the trace.
        // Execute sp_trace_setstatus by specifying @status = 2 to close the trace and delete its information from the server.
        
        private static ExpressProfiler.SqlServerProfiler s_profiler;


        public class Argument
        {
            public string Key;

            private object m_value;
            private object InternalValue
            {
                get
                {
                    if (this.m_value != null)
                        return this.m_value;

                    if (!CommandLine.ContainsKey(this.Key))
                        return null;

                    if (object.ReferenceEquals(@Type, typeof(string)))
                    {
                        this.m_value = CommandLine.GetValue<string>(this.Key);
                        return this.m_value;
                    } // End if (object.ReferenceEquals(@Type, typeof(string))) 

                    if (object.ReferenceEquals(@Type, typeof(bool)))
                    {
                        bool? arg = CommandLine.GetValue<bool?>(this.Key);
                        if (!arg.HasValue)
                            arg = false;

                        this.m_value = arg.Value;
                        return this.m_value;
                    } // End if (object.ReferenceEquals(@Type, typeof(bool))) 

                    if (object.ReferenceEquals(@Type, typeof(int)))
                    {
                        int? arg = CommandLine.GetValue<int?>(this.Key);
                        if (!arg.HasValue)
                            arg = 0;

                        this.m_value = arg.Value;
                        return this.m_value;
                    } // End if (object.ReferenceEquals(@Type, typeof(int))) 

                    if (object.ReferenceEquals(@Type, typeof(long)))
                    {
                        long? arg = CommandLine.GetValue<long?>(this.Key);
                        if (!arg.HasValue)
                            arg = 0;

                        this.m_value = arg.Value;
                        return this.m_value;
                    } // End if (object.ReferenceEquals(@Type, typeof(long))) 

                    return this.m_value;
                } // End Get 

            } // End Property InternalValue 


            public T Value<T>()
            {
                return (T) this.InternalValue;
            } // End Function Value 


            public string StringValue
            {
                get { return this.CommandLine[this.Key]; }
            } // End Property StringValue 


            public bool IsPresent
            {
                get { return this.CommandLine.ContainsKey(this.Key); }
            } // End Property IsPresent 


            public System.Type @Type;
            public string HelpText;
            public bool Required;
            public bool IsCommand;

            public sql_profiler.CommandLineArguments CommandLine;
        } // End Class Argument 


        public static System.Collections.Generic.Dictionary<string, Argument> 
        GetCommandLineArguments(string[] args
            , System.Collections.Generic.Dictionary<string, string> defaultValues)
        {
            sql_profiler.CommandLineArguments arguments =
                new sql_profiler.CommandLineArguments(args, defaultValues);


            System.Collections.Generic.Dictionary<string, Argument> ls = new 
                System.Collections.Generic.Dictionary<string, Argument>(
                    System.StringComparer.InvariantCultureIgnoreCase
            );
            
            ls.Add("server",
                new Argument()
                {
                    Key = "server"
                    , @Type=typeof(string)
                    , HelpText = @"The server\instance on which the DB to profile is"
                    , Required = true
                }
            );
            
            ls.Add("db",
                new Argument()
                {
                    Key = "db"
                    , @Type=typeof(string)
                    , HelpText = "The db to profile"
                    , Required = true
                }
            );
            
            ls.Add("username",
                new Argument()
                {
                    Key = "username"
                    , @Type=typeof(string)
                    , HelpText = "The SQL user-name; On Windows, uses integrated security if username is NULL or EMPTY."
                    , Required = false
                }
            );
            
            ls.Add("password",
                new Argument()
                {
                    Key = "password"
                    , @Type=typeof(string)
                    , HelpText = "The password for the sql-user"
                    , Required = false
                }
            );
            
            
            ls.Add("help",
                new Argument()
                {
                    Key = "help"
                    , @Type=typeof(string)
                    , HelpText = "Display help"
                    , Required = false
                    , IsCommand = true 
                }
            );
            
            
            ls.Add("list",
                new Argument()
                {
                    Key = "list"
                    , @Type=typeof(string)
                    , HelpText = "Show the command line values used."
                    , Required = false 
                    , IsCommand = true 
                }
            );
            
            
            ls.Add("showinput",
                new Argument()
                {
                    Key = "showinput"
                    , @Type=typeof(string)
                    , HelpText = "Show the command line values as passed from console."
                    , Required = false
                    , IsCommand = true
                }
            );

            if (arguments != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, Argument> kvp in ls)
                {
                    kvp.Value.CommandLine = arguments;
                } // Next kvp 

            } // End if (arguments != null)

            return ls;
        } // End Function GetCommandLineArguments 

        public static void Help(System.Collections.Generic.Dictionary<string, Argument> arg_list)
        {
            System.Console.WriteLine("Valid command-line arguments:");
            
            foreach(System.Collections.Generic.KeyValuePair<string, Argument> kvp in arg_list)
            {
                System.Console.WriteLine($"  --{kvp.Key}: {kvp.Value.HelpText}");
            } // Next kvp 

        } // End Sub Help 


        public static void ListValues(System.Collections.Generic.Dictionary<string, Argument> arg_list)
        {
            System.Console.WriteLine("Used command-line argument's values:");
            
            foreach (System.Collections.Generic.KeyValuePair<string, Argument> kvp in arg_list)
            {
                if (!kvp.Value.IsCommand)
                    System.Console.WriteLine($"{kvp.Key}: {kvp.Value.StringValue}");
            } // Next kvp 

        } // End Sub ListValues 


        public static void ShowInput(string[] args)
        {
            
            for (int i = 0; i < args.Length; ++i)
            {
                System.Console.WriteLine($"{i}: {args[i]}");
            } // Next i 

        } // End Sub ShowInput 


        public static string GetPlatformDefaultInstance()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                    System.Runtime.InteropServices.OSPlatform.Windows
            );

            // TODO: Check if there is an instance SQLExpress
            //  https://stackoverflow.com/questions/141154/how-can-i-determine-installed-sql-server-instances-and-their-versions
            // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\MSRS13.SQLEXPRESS
            // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL13.SQLEXPRESS
            // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL13E.LOCALDB
            // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\MSSQLServer

            if (isWindows)
                return System.Environment.MachineName + @"\" + "SQLEXPRESS";

            return  System.Environment.MachineName;
        } // End Function GetPlatformDefaultInstance 


        static void Main(string[] args)
        {
            // MainTest(args);
            DoProfiling(args);
            
            System.Drawing.Color.
            
        }

        static void MainTest(string[] args)
        {
            string instance = GetPlatformDefaultInstance();
            // dotnet run --server localhost --username MY_USER --password MY_PASSWORD --db MY_DB
            string ar = $"--server {instance} --username WebAppWebServices --password TOP_SECRET /Db COR_Basic_Demo_V4";
            DoProfiling(ar.Split(' '));
        } // End Sub Main 


        
        static void DoProfiling(string[] args)
        {
            // https://github.com/dotnet/coreclr/issues/8565
            // https://shazwazza.com/post/aspnet-core-application-shutdown-events/
            // Will not be executed when !windows
            WindowsFix.AddConsoleHandler(ConsoleCtrlCheck);

            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            System.Console.CancelKeyPress += Console_CancelKeyPress;

            System.Collections.Generic.Dictionary<string, string> defaultValues = 
                new System.Collections.Generic.Dictionary<string, string>();

            defaultValues["server"] = GetPlatformDefaultInstance();
            defaultValues["db"] = "COR_Basic_Demo_V4";

            System.Collections.Generic.Dictionary<string, Argument> arg_list = 
                GetCommandLineArguments(args, defaultValues);
            
            
            if (arg_list["showinput"].IsPresent)
            {
                ShowInput(args);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine("--- Press any key to stop continue --- ");
                System.Console.ReadKey();
                return;
            }
            
            if (arg_list["list"].IsPresent)
            {
                ListValues(arg_list);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine("--- Press any key to stop continue --- ");
                System.Console.ReadKey();
                return;
            }
            
            if (arg_list["help"].IsPresent)
            {
                Help(arg_list);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine("--- Press any key to stop continue --- ");
                System.Console.ReadKey();
                return;
            }
            
            
            string server = arg_list["server"].Value<string>();
            string db = arg_list["db"].Value<string>();
            string username = arg_list["username"].Value<string>();
            string password = arg_list["password"].Value<string>();
            

            s_profiler = new ExpressProfiler.SqlServerProfiler(server, db, username, password);


            s_profiler.StartProfiling();


            System.Console.WriteLine("--- Press any key to stop profiling --- ");
            System.Console.ReadKey();
            OnExit();
        } // End Sub Test 


        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)
        ]
        private static void OnExit()
        {
            if(s_profiler != null) // Is there any chance this could happen ? 
                s_profiler.StopProfiling();
        } // End Sub OnExit 


        private static bool ConsoleCtrlCheck(WindowsFix.CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case WindowsFix.CtrlTypes.CTRL_C_EVENT: // CTRL+C received!
                case WindowsFix.CtrlTypes.CTRL_BREAK_EVENT: // CTRL+BREAK received!
                case WindowsFix.CtrlTypes.CTRL_CLOSE_EVENT: // Program being closed!
                case WindowsFix.CtrlTypes.CTRL_LOGOFF_EVENT: // User is logging off!
                case WindowsFix.CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    OnExit();
                    break;
            } // End switch (ctrlType) 

            return true;
        } // End Function ConsoleCtrlCheck 


        // Catch CTRL+C
        private static void Console_CancelKeyPress(object sender, System.ConsoleCancelEventArgs e)
        {
            OnExit();
        } // End Sub Console_CancelKeyPress 


        // Does not catch anything... 
        private static void CurrentDomain_ProcessExit(object sender, System.EventArgs e)
        {
            OnExit();
        } // End Sub CurrentDomain_ProcessExit 


        // Catch unhandled exceptions 
        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine(((System.Exception)e.ExceptionObject).Message, "Error");
            OnExit();
        } // End Sub CurrentDomain_UnhandledException 


    } // End Class Program 


} // End Namespace sql_profiler 
