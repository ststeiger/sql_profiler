
using System;

namespace ExpressProfiler
{


    public class SqlServerProfiler
    {
        protected enum ProfilingStateEnum { psStopped, psProfiling, psPaused }
        protected ProfilingStateEnum m_ProfilingState;

        protected System.Data.SqlClient.SqlConnection m_Conn;
        protected RawTraceReader m_Rdr;
        protected bool m_NeedStop;
        protected System.Timers.Timer m_timer;
        protected System.Threading.Thread m_Thr;
        protected System.Collections.Concurrent.ConcurrentQueue<ProfilerEvent> m_events;
        protected readonly System.Collections.Generic.List<PerfColumn> m_columns;
        protected readonly YukonLexer m_Lex;
            
        
        protected readonly ProfilerEvent m_EventStarted = new ProfilerEvent();
        protected readonly ProfilerEvent m_EventStopped = new ProfilerEvent();
        protected readonly ProfilerEvent m_EventPaused = new ProfilerEvent();


        protected string m_server;
        protected string m_username;
        protected string m_password;
        protected bool m_integrated_security;
        protected string m_database;


        public SqlServerProfiler()
            :this(System.Environment.MachineName + @"\" + "SQLEXPRESS")
        { }

        public SqlServerProfiler(string server)
            : this(server, null)
        { }

        public SqlServerProfiler(string server, string database)
            : this(server, database, null, null)
        { }


        public SqlServerProfiler(string server, string database
            , string username, string password)
        {
            this.m_server = server;
            this.m_database = database;
            this.m_username = username;
            this.m_password = password;
            this.m_integrated_security = string.IsNullOrWhiteSpace(username);

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                   System.Runtime.InteropServices.OSPlatform.Windows
               );

            if (this.m_integrated_security && !isWindows)
            {
                throw new System.ArgumentException($"Username is NULL or empty. Cannot use integrated-security on non-windows platform.");
            }

            if (!this.m_integrated_security)
            {
                if (password == null)
                    throw new System.ArgumentException($"{nameof(password)} cannot be NULL when integrated security is false.");
            } // End if (!integratedSecurity)
            
            
            this.m_Lex = new YukonLexer();
            //this.m_events = new Queue<ProfilerEvent>(10);
            this.m_events = new System.Collections.Concurrent.ConcurrentQueue<ProfilerEvent>();
            this.m_timer = new System.Timers.Timer(1000);
            this.m_timer.AutoReset = false;
            this.m_timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            this.m_timer.Start();

            this.m_columns = new System.Collections.Generic.List<PerfColumn>();
            this.m_columns.Add(new PerfColumn { Caption = "Event Class", Column = ProfilerEventColumns.EventClass, Width = 122 });
            this.m_columns.Add(new PerfColumn { Caption = "Text Data", Column = ProfilerEventColumns.TextData, Width = 255 });
            this.m_columns.Add(new PerfColumn { Caption = "Login Name", Column = ProfilerEventColumns.LoginName, Width = 79 });
            this.m_columns.Add(new PerfColumn { Caption = "CPU", Column = ProfilerEventColumns.CPU, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            this.m_columns.Add(new PerfColumn { Caption = "Reads", Column = ProfilerEventColumns.Reads, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            this.m_columns.Add(new PerfColumn { Caption = "Writes", Column = ProfilerEventColumns.Writes, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            this.m_columns.Add(new PerfColumn { Caption = "Duration, ms", Column = ProfilerEventColumns.Duration, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            this.m_columns.Add(new PerfColumn { Caption = "SPID", Column = ProfilerEventColumns.SPID, Width = 50, Alignment = HorizontalAlignment.Right });

            // if (m_currentsettings.EventsColumns.StartTime) 
            m_columns.Add(new PerfColumn { Caption = "Start time", Column = ProfilerEventColumns.StartTime, Width = 140, Format = "yyyy-MM-ddThh:mm:ss.ffff" });
            // if (m_currentsettings.EventsColumns.EndTime) 
            m_columns.Add(new PerfColumn { Caption = "End time", Column = ProfilerEventColumns.EndTime, Width = 140, Format = "yyyy-MM-ddThh:mm:ss.ffff" });
            // if (m_currentsettings.EventsColumns.DatabaseName) 
            m_columns.Add(new PerfColumn { Caption = "DatabaseName", Column = ProfilerEventColumns.DatabaseName, Width = 70 });
            // if (m_currentsettings.EventsColumns.ObjectName) 
            m_columns.Add(new PerfColumn { Caption = "Object name", Column = ProfilerEventColumns.ObjectName, Width = 70 });
            // if (m_currentsettings.EventsColumns.ApplicationName) 
            m_columns.Add(new PerfColumn { Caption = "Application name", Column = ProfilerEventColumns.ApplicationName, Width = 70 });
            // if (m_currentsettings.EventsColumns.HostName) 
            m_columns.Add(new PerfColumn { Caption = "Host name", Column = ProfilerEventColumns.HostName, Width = 70 });

            // m_columns.Add(new PerfColumn { Caption = "#", Column = -1, Width = 53, Alignment = HorizontalAlignment.Right });
        } // End Constructor 



        protected void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.m_timer.Stop();

            try
            {
                int i = -1;
                while ((i = this.m_events.Count) > 0)
                {
                    ProfilerEvent evt;
                    if (this.m_events.TryDequeue(out evt))
                        NewEventArrived(evt, i == 1);
                    else
                        System.Threading.Thread.Sleep(50);
                } // Whend 

            } // End try 
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Timer_Elapsed Error: {ex.Message}\n{ex.StackTrace}");
            }

            this.m_timer.Start();
        } // End Sub Timer_Elapsed 


        protected System.Data.SqlClient.SqlConnection GetConnection()
        {
            string cs = string.Format(@"Data Source = {0}; Initial Catalog = master; 
Integrated Security=SSPI;Application Name=Express Profiler", this.m_server);

            if (!this.m_integrated_security)
            {
                cs = string.Format(@"Data Source={0};Initial Catalog=master;
User Id={1};Password='{2}';;Application Name=Express Profiler"
, this.m_server, this.m_username, this.m_password);
            } // End if (!this.m_integrated_security)

            return new System.Data.SqlClient.SqlConnection(cs);
        } // End Function GetConnection 


        protected void OpenConnection(System.Data.SqlClient.SqlConnection con)
        {
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
        } // End Sub OpenConnection 

        protected void CloseConnection(System.Data.SqlClient.SqlConnection con)
        {
            if (con.State != System.Data.ConnectionState.Closed)
                con.Close();
        } // End Sub CloseConnection 

    
        protected void ProfilerThread(object state)
        {
            try
            {
                while (!this.m_NeedStop && this.m_Rdr.TraceIsActive)
                {
                    ProfilerEvent evt = this.m_Rdr.Next();
                    if (evt != null)
                    {
                        // lock (this)
                        m_events.Enqueue(evt);
                    } // End if (evt != null)
                } // Whend 
            }
            catch (System.Exception e)
            {
                // lock (this)
                // {
                if (!this.m_NeedStop && this.m_Rdr.TraceIsActive)
                {
                    System.Console.WriteLine($"Error: {e.Message}\n{e.StackTrace}");
                }
                // } // End lock 
            } // End Catch 
        } // End Sub ProfilerThread


        /*
        protected void RunProfiling(bool showfilters)
        {
            if (showfilters)
            {
                // ts m_currentsettings.GetCopy();
                TraceProperties.TraceSettings ts = new TraceProperties.TraceSettings();
                using (TraceProperties frm = new TraceProperties())
                {
                    frm.SetSettings(ts);
                    // if (DialogResult.OK != frm.ShowDialog()) return;
                    // m_currentsettings = frm.m_currentsettings.GetCopy();
                }
            }

            StartProfiling();
        } // End Sub RunProfiling 
        */

        public enum StringFilterCondition
        {
            Like,
            NotLike
        } // End Enum StringFilterCondition 

        public enum IntFilterCondition
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan
        } // End Enum IntFilterCondition 


        protected void SetIntFilter(int? value, IntFilterCondition condition, int column)
        {
            int[] com = new[] {
                  ComparisonOperators.Equal
                , ComparisonOperators.NotEqual
                , ComparisonOperators.GreaterThan
                , ComparisonOperators.LessThan
            };

            if ((null != value))
            {
                long? v = value;
                this.m_Rdr.SetFilter(column, LogicalOperators.AND, com[(int)condition], v);
            } // End if ((null != value)) 

        } // End Sub SetIntFilter 


        protected void SetStringFilter(string value, StringFilterCondition condition, int column)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.m_Rdr.SetFilter(column, LogicalOperators.AND
                    , condition == StringFilterCondition.Like ? 
                    ComparisonOperators.Like : ComparisonOperators.NotLike
                    , value
                    );
            } // End if (!string.IsNullOrEmpty(value)) 

        } // End Sub SetStringFilter 


        public void StartProfiling()
        {
            this.m_events.Clear();

            this.m_Conn = GetConnection();
            OpenConnection(this.m_Conn);

            this.m_Rdr = new RawTraceReader(m_Conn);
            this.m_Rdr.CreateTrace();

            //if (m_currentsettings.EventsColumns.BatchCompleted)
            { 
                m_Rdr.SetEvent(ProfilerEvents.TSQL.SQLBatchCompleted,
                            ProfilerEventColumns.TextData,
                            ProfilerEventColumns.LoginName,
                            ProfilerEventColumns.CPU,
                            ProfilerEventColumns.Reads,
                            ProfilerEventColumns.Writes,
                            ProfilerEventColumns.Duration,
                            ProfilerEventColumns.SPID,
                            ProfilerEventColumns.StartTime,
                            ProfilerEventColumns.EndTime,
                            ProfilerEventColumns.DatabaseName,
                            ProfilerEventColumns.ApplicationName,
                            ProfilerEventColumns.HostName
                 );
            } // End if (m_currentsettings.EventsColumns.BatchCompleted)

            //if (m_currentsettings.EventsColumns.RPCCompleted)
            {
                m_Rdr.SetEvent(ProfilerEvents.StoredProcedures.RPCCompleted,
                               ProfilerEventColumns.TextData, ProfilerEventColumns.LoginName,
                               ProfilerEventColumns.CPU, ProfilerEventColumns.Reads,
                               ProfilerEventColumns.Writes, ProfilerEventColumns.Duration,
                               ProfilerEventColumns.SPID
                               , ProfilerEventColumns.StartTime, ProfilerEventColumns.EndTime
                               , ProfilerEventColumns.DatabaseName
                               , ProfilerEventColumns.ObjectName
                               , ProfilerEventColumns.ApplicationName
                               , ProfilerEventColumns.HostName

                    );
            } // End if (m_currentsettings.EventsColumns.RPCCompleted) 


            if (!string.IsNullOrWhiteSpace(this.m_database))
                SetStringFilter(this.m_database, StringFilterCondition.Like, ProfilerEventColumns.DatabaseName);

            this.m_Rdr.SetFilter(ProfilerEventColumns.ApplicationName, LogicalOperators.AND
                    , ComparisonOperators.NotLike, "Express Profiler");


            this.m_Rdr.StartTrace();
            this.m_NeedStop = false;
            this.m_Thr = new System.Threading.Thread(ProfilerThread) {
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.Lowest
            };
            
            this.m_ProfilingState = ProfilingStateEnum.psProfiling;
            NewEventArrived(m_EventStarted, true);
            m_Thr.Start();
        } // End Sub StartProfiling 



        public void PauseProfiling()
        {
            if (this.m_ProfilingState != ProfilingStateEnum.psProfiling)
                return;
            
            using (System.Data.SqlClient.SqlConnection cn = GetConnection())
            {
                OpenConnection(cn);
                this.m_Rdr.StopTrace(cn);
                CloseConnection(cn);
            } // End Using cn 

            this.m_ProfilingState = ProfilingStateEnum.psPaused;
            NewEventArrived(m_EventPaused, true);
        } // End Sub PauseProfiling 


        public void StopProfiling()
        {
            if (this.m_ProfilingState == ProfilingStateEnum.psStopped)
                return;
            
            using (System.Data.SqlClient.SqlConnection cn = GetConnection())
            {
                OpenConnection(cn);
                this.m_Rdr.StopTrace(cn);
                this.m_Rdr.CloseTrace(cn);
                CloseConnection(cn);
            } // End Using cn 

            this.m_NeedStop = true;
            this.m_ProfilingState = ProfilingStateEnum.psStopped;
            NewEventArrived(m_EventStopped, true);
        } // End Sub StopProfiling 
        

        protected string GetEventCaption(ProfilerEvent evt)
        {
            if (evt == m_EventStarted)
                return "Trace started";

            if (evt == m_EventPaused)
                return "Trace paused";

            if (evt == m_EventStopped)
                return "Trace stopped";

            return ProfilerEvents.Names[evt.EventClass];
        } // End Function GetEventCaption 


        protected void NewEventArrived(ProfilerEvent evt, bool last)
        {
            // if (evt == m_EventStarted || evt == m_EventPaused || evt == m_EventStopped) return;
            
            //m_columns.Add(new PerfColumn { Caption = "Event Class", Column = ProfilerEventColumns.EventClass, Width = 122 });
            //m_columns.Add(new PerfColumn { Caption = "Text Data", Column = ProfilerEventColumns.TextData, Width = 255 });
            //m_columns.Add(new PerfColumn { Caption = "Login Name", Column = ProfilerEventColumns.LoginName, Width = 79 });
            //m_columns.Add(new PerfColumn { Caption = "CPU", Column = ProfilerEventColumns.CPU, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            //m_columns.Add(new PerfColumn { Caption = "Reads", Column = ProfilerEventColumns.Reads, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            //m_columns.Add(new PerfColumn { Caption = "Writes", Column = ProfilerEventColumns.Writes, Width = 78, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            //m_columns.Add(new PerfColumn { Caption = "Duration, ms", Column = ProfilerEventColumns.Duration, Width = 82, Alignment = HorizontalAlignment.Right, Format = "#,0" });
            //m_columns.Add(new PerfColumn { Caption = "SPID", Column = ProfilerEventColumns.SPID, Width = 50, Alignment = HorizontalAlignment.Right });
            //m_columns.Add(new PerfColumn { Caption = "Start time", Column = ProfilerEventColumns.StartTime, Width = 140, Format = "yyyy-MM-ddThh:mm:ss.ffff" });
            //m_columns.Add(new PerfColumn { Caption = "End time", Column = ProfilerEventColumns.EndTime, Width = 140, Format = "yyyy-MM-ddThh:mm:ss.ffff" });
            //m_columns.Add(new PerfColumn { Caption = "DatabaseName", Column = ProfilerEventColumns.DatabaseName, Width = 70 });
            //m_columns.Add(new PerfColumn { Caption = "Object name", Column = ProfilerEventColumns.ObjectName, Width = 70 });
            //m_columns.Add(new PerfColumn { Caption = "Application name", Column = ProfilerEventColumns.ApplicationName, Width = 70 });
            //m_columns.Add(new PerfColumn { Caption = "Host name", Column = ProfilerEventColumns.HostName, Width = 70 });


            // System.Console.WriteLine(evt);
            string caption = GetEventCaption(evt);
            System.Console.Write(caption);
            System.Console.Write(new string(' ', System.Console.BufferWidth - System.Console.CursorLeft));
            
            string td = evt.GetFormattedData(ProfilerEventColumns.TextData, null);
            // System.Console.WriteLine(td);
            
            
            // System.Windows.Forms.RichTextBox rich, 
            // RTFBuilder rb = new RTFBuilder { 
            //     BackColor = System.Drawing.Color.White 
            // };
            // rich.Text = "";
            
            
            ConsoleOutputWriter cw = new ConsoleOutputWriter(){ 
                BackColor = System.Drawing.Color.White 
            };


            // var lex = new YukonLexer(); lex.SyntaxHighlight(cw, td);
            this.m_Lex.SyntaxHighlight(cw, td);
            // rich.Rtf = this.m_Lex.SyntaxHighlight(rb, td);

            // for(int i = 0; i < 65; ++i) // 65 is size of array
            // System.Console.WriteLine(evt.GetFormattedData(i, null));

            //for (int i = 1; i < this.m_columns.Count; ++i)
            //{
            //    System.Console.WriteLine(i);
            //    System.Console.WriteLine(this.m_columns[i].Caption);
            //    System.Console.WriteLine(evt.GetFormattedData(this.m_columns[i].Column, this.m_columns[i].Format));
            //} // Next i 

        } // End Sub NewEventArrived 


    } // End Class MyProfiler 


} // End Namespace ExpressProfiler
