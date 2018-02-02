
// using System.Windows.Forms;


namespace ExpressProfiler
{



    public partial class TraceProperties // : Form
        : System.IDisposable 
    {


        public enum StringFilterCondition
        {
            Like,
            NotLike
        }

        public enum IntFilterCondition
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan
        }


        public static StringFilterCondition ParseStringCondition(string value)
        {
            switch (value.ToLower())
            {
                case "like":
                case "eq":
                case "=": return StringFilterCondition.Like;
                case "notlike": return StringFilterCondition.NotLike;
            }
            throw new System.Exception("Unknown filter condition:" + value);
        }


        public static IntFilterCondition ParseIntCondition(string value)
        {
            switch (value.ToLower())
            {
                case "equal":
                case "eq":
                case "=":
                    return IntFilterCondition.Equal;
                case "notequal":
                case "ne":
                case "!=":
                case "<>": return IntFilterCondition.NotEqual;
                case "greaterthan":
                case "ge":
                case ">": return IntFilterCondition.GreaterThan;
                case "lessthan":
                case "le":
                case "<": return IntFilterCondition.LessThan;
            }
            throw new System.Exception("Unknown filter condition:" + value);
        }
        /*
         declare @xml xml
        set @xml = '<root>
        <r text="LoginName" type = "String" />
        <r text="TextData"  type = "String"/>
        <r text="DatabaseName"  type = "String"/>
        <r text="Duration"  type = "Int"/>
        <r text="Reads"  type = "Int"/>
        <r text="Writes"  type = "Int"/>
        <r text="CPU"  type = "Int"/>
        </root>'

        select		'
                    [System.ComponentModel.Category(@"'+b.value('@text','varchar(512)')+'")]
                    [System.ComponentModel.DisplayName(@"Condition")]
                    public '+b.value('@type','varchar(512)')+'FilterComparison '+replace(b.value('@text','varchar(512)'),' ','')+'FilterComparison { get; set; }
                    [System.ComponentModel.Category(@"'+b.value('@text','varchar(512)')+'")]
                    [System.ComponentModel.DisplayName(@"Value")]
                    public '+lower(b.value('@type','varchar(512)'))+' '+replace(b.value('@text','varchar(512)'),' ','')+ '{ get; set; }'

        from @xml.nodes('/root/r') a(b)
        order by b.value('@text','varchar(512)')
         */

        [System.Serializable]
        public class TraceSettings
        {
            public TraceEventsColumns EventsColumns;
            public TraceFilters Filters;

            public TraceSettings()
            {

                EventsColumns = new TraceEventsColumns
                {
                    BatchCompleted = true,
                    RPCCompleted = true,
                    StartTime = true,
                    EndTime = true
                };
                Filters = new TraceFilters
                {
                    MaximumEventCount = 5000,
                    CpuFilterCondition = IntFilterCondition.GreaterThan,
                    ReadsFilterCondition = IntFilterCondition.GreaterThan,
                    WritesFilterCondition = IntFilterCondition.GreaterThan,
                    DurationFilterCondition = IntFilterCondition.GreaterThan
                };
            }

            public string GetAsXmlString()
            {
                System.Xml.Serialization.XmlSerializer x = 
                    new System.Xml.Serialization.XmlSerializer(typeof(TraceSettings));
                
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    x.Serialize(sw, this);
                    return sw.ToString();
                }
            }

            public static TraceSettings GetDefaultSettings()
            {
                return new TraceSettings { };
            }

            public TraceSettings GetCopy()
            {
                return new TraceSettings
                {

                    EventsColumns = new TraceEventsColumns
                    {
                        BatchCompleted = EventsColumns.BatchCompleted,
                        BatchStarting = EventsColumns.BatchStarting,
                        ExistingConnection = EventsColumns.ExistingConnection,
                        LoginLogout = EventsColumns.LoginLogout,
                        RPCCompleted = EventsColumns.RPCCompleted,
                        RPCStarting = EventsColumns.RPCStarting,
                        SPStmtCompleted = EventsColumns.SPStmtCompleted,
                        SPStmtStarting = EventsColumns.SPStmtStarting,
                        UserErrorMessage = EventsColumns.UserErrorMessage,
                        ApplicationName = EventsColumns.ApplicationName,
                        HostName = EventsColumns.HostName,
                        DatabaseName = EventsColumns.DatabaseName,
                        EndTime = EventsColumns.EndTime,
                        ObjectName = EventsColumns.ObjectName,
                        StartTime = EventsColumns.StartTime,
                        BlockedProcessPeport = EventsColumns.BlockedProcessPeport,
                        SQLStmtStarting = EventsColumns.SQLStmtStarting,
                        SQLStmtCompleted = EventsColumns.SQLStmtCompleted
                    }
                               ,
                    Filters = new TraceFilters
                    {
                        CPU = Filters.CPU,
                        CpuFilterCondition = Filters.CpuFilterCondition,
                        DatabaseName = Filters.DatabaseName,
                        DatabaseNameFilterCondition = Filters.DatabaseNameFilterCondition,
                        Duration = Filters.Duration,
                        DurationFilterCondition = Filters.DurationFilterCondition,
                        LoginName = Filters.LoginName,
                        HostName = Filters.HostName,
                        HostNameFilterCondition = Filters.HostNameFilterCondition,
                        LoginNameFilterCondition = Filters.LoginNameFilterCondition,
                        Reads = Filters.Reads,
                        ReadsFilterCondition = Filters.ReadsFilterCondition,
                        TextData = Filters.TextData,
                        TextDataFilterCondition = Filters.TextDataFilterCondition,
                        Writes = Filters.Writes,
                        WritesFilterCondition = Filters.WritesFilterCondition,
                        MaximumEventCount = Filters.MaximumEventCount,
                        SPID = Filters.SPID,
                        SPIDFilterCondition = Filters.SPIDFilterCondition,
                        ApplicationName = Filters.ApplicationName,
                        ApplicationNameFilterCondition = Filters.ApplicationNameFilterCondition,

                    }
                }
                    ;
            }

        }

        internal TraceSettings m_currentsettings;
        [System.Serializable]
        public class TraceEventsColumns
        {
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"ExistingConnection")]
            [System.ComponentModel.Description(@"The ExistingConnection event class indicates the properties of existing user connections when the trace was started. The server raises one ExistingConnection event per existing user connection.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool ExistingConnection { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"LoginLogout")]
            [System.ComponentModel.Description(@"The Audit Login event class indicates that a user has successfully logged in to SQL Server. Events in this class are fired by new connections or by connections that are reused from a connection pool. The Audit Logout event class indicates that a user has logged out of (logged off) Microsoft SQL Server. Events in this class are fired by new connections or by connections that are reused from a connection pool.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool LoginLogout { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"RPC:Starting")]
            [System.ComponentModel.Description(@"The RPC:Starting event class indicates that a remote procedure call has started.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool RPCStarting { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"RPC:Completed")]
            [System.ComponentModel.Description(@"The RPC:Completed event class indicates that a remote procedure call has been completed. ")]
            [System.ComponentModel.DefaultValue(false)]
            public bool RPCCompleted { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SQL:BatchStarting")]
            [System.ComponentModel.Description(@"The SQL:BatchStarting event class indicates that a Transact-SQL batch is starting.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool BatchStarting { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SQL:BatchCompleted")]
            [System.ComponentModel.Description(@"The SQL:BatchCompleted event class indicates that the Transact-SQL batch has completed. ")]
            [System.ComponentModel.DefaultValue(false)]
            public bool BatchCompleted { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SP:StmtCompleted")]
            [System.ComponentModel.Description(@"The SP:StmtCompleted event class indicates that a Transact-SQL statement within a stored procedure has completed. ")]
            [System.ComponentModel.DefaultValue(false)]
            public bool SPStmtCompleted { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SP:StmtStarting")]
            [System.ComponentModel.Description(@"The SP:StmtStarting event class indicates that a Transact-SQL statement within a stored procedure has started. ")]
            [System.ComponentModel.DefaultValue(false)]
            public bool SPStmtStarting { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"User Error Message")]
            [System.ComponentModel.Description(@"The User Error Message event class displays the error message as seen by the user in the case of an error or exception. The error message text appears in the TextData field.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool UserErrorMessage { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"Blocked process report")]
            [System.ComponentModel.Description(@"The Blocked Process Report event class indicates that a task has been blocked for more than a specified amount of time. This event class does not include system tasks or tasks that are waiting on non deadlock-detectable resources.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool BlockedProcessPeport { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SQL:StmtStarting")]
            [System.ComponentModel.Description(@"The SQL:StmtStarting event class indicates that a Transact-SQL statement has started.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool SQLStmtStarting { get; set; }
            [System.ComponentModel.Category(@"Events")]
            [System.ComponentModel.DisplayName(@"SQL:StmtCompleted")]
            [System.ComponentModel.Description(@"The SQL:StmtCompleted event class indicates that a Transact-SQL statement has completed. ")]
            [System.ComponentModel.DefaultValue(false)]
            public bool SQLStmtCompleted { get; set; }




            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"Start time")]
            [System.ComponentModel.Description(@"The time at which the event started, when available.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool StartTime { get; set; }
            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"End time")]
            [System.ComponentModel.Description(@"The time at which the event ended. This column is not populated for event classes that refer to an event that is starting, such as SQL:BatchStarting or SP:Starting.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool EndTime { get; set; }
            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"DatabaseName")]
            [System.ComponentModel.Description(@"The name of the database in which the user statement is running.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool DatabaseName { get; set; }
            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"Application name")]
            [System.ComponentModel.Description(@"The name of the client application that created the connection to an instance of SQL Server. This column is populated with the values passed by the application and not the name of the program.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool ApplicationName { get; set; }
            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"Object name")]
            [System.ComponentModel.Description(@"The name of the object that is referenced.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool ObjectName { get; set; }
            [System.ComponentModel.Category(@"Columns")]
            [System.ComponentModel.DisplayName(@"Host name")]
            [System.ComponentModel.Description(@"Name of the client computer that originated the request.")]
            [System.ComponentModel.DefaultValue(false)]
            public bool HostName { get; set; }

        }



        [System.Serializable]
        public class TraceFilters
        {

            [System.ComponentModel.Category(@"CPU")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public IntFilterCondition CpuFilterCondition { get; set; }
            [System.ComponentModel.Category(@"CPU")]
            [System.ComponentModel.DisplayName(@"Value")]
            public int? CPU { get; set; }

            [System.ComponentModel.Category(@"DatabaseName")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public StringFilterCondition DatabaseNameFilterCondition { get; set; }
            [System.ComponentModel.Category(@"DatabaseName")]
            [System.ComponentModel.DisplayName(@"Value")]
            public string DatabaseName { get; set; }

            [System.ComponentModel.Category(@"Duration")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public IntFilterCondition DurationFilterCondition { get; set; }
            [System.ComponentModel.Category(@"Duration")]
            [System.ComponentModel.DisplayName(@"Value")]
            public int? Duration { get; set; }

            [System.ComponentModel.Category(@"LoginName")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public StringFilterCondition LoginNameFilterCondition { get; set; }
            [System.ComponentModel.Category(@"LoginName")]
            [System.ComponentModel.DisplayName(@"Value")]
            public string LoginName { get; set; }

            [System.ComponentModel.Category(@"HostName")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public StringFilterCondition HostNameFilterCondition { get; set; }
            [System.ComponentModel.Category(@"HostName")]
            [System.ComponentModel.DisplayName(@"Value")]
            public string HostName { get; set; }


            [System.ComponentModel.Category(@"Reads")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public IntFilterCondition ReadsFilterCondition { get; set; }
            [System.ComponentModel.Category(@"Reads")]
            [System.ComponentModel.DisplayName(@"Value")]
            public int? Reads { get; set; }

            [System.ComponentModel.Category(@"TextData")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public StringFilterCondition TextDataFilterCondition { get; set; }
            [System.ComponentModel.Category(@"TextData")]
            [System.ComponentModel.DisplayName(@"Value")]
            public string TextData { get; set; }

            [System.ComponentModel.Category(@"Writes")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public IntFilterCondition WritesFilterCondition { get; set; }

            [System.ComponentModel.Category(@"Writes")]
            [System.ComponentModel.DisplayName(@"Value")]
            public int? Writes { get; set; }

            [System.ComponentModel.Category(@"Maximum events count")]
            [System.ComponentModel.DisplayName(@"Maximum events count")]
            //            [System.ComponentModel.DefaultValue(5000)]
            public int MaximumEventCount { get; set; }

            [System.ComponentModel.Category(@"SPID")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public IntFilterCondition SPIDFilterCondition { get; set; }
            [System.ComponentModel.Category(@"SPID")]
            [System.ComponentModel.DisplayName(@"Value")]
            public int? SPID { get; set; }

            [System.ComponentModel.Category(@"ApplicationName")]
            [System.ComponentModel.DisplayName(@"Condition")]
            public StringFilterCondition ApplicationNameFilterCondition { get; set; }
            [System.ComponentModel.Category(@"ApplicationName")]
            [System.ComponentModel.DisplayName(@"Value")]
            public string ApplicationName { get; set; }

        }

        public TraceProperties()
        {
            // InitializeComponent();
        }

        public void SetSettings(TraceSettings st)
        {
            m_currentsettings = st;
            // edEvents.SelectedObject = m_currentsettings.EventsColumns;
            // edFilters.SelectedObject = m_currentsettings.Filters;
        }

        private void btnReset_Click(object sender, System.EventArgs e)
        {
            SetSettings(TraceSettings.GetDefaultSettings());
        }

        private void btnSaveAsDefault_Click(object sender, System.EventArgs e)
        {
            // Properties.Settings.Default.TraceSettings = m_currentsettings.GetAsXmlString();
            // Properties.Settings.Default.Save();
        }


        internal static bool AtLeastOneEventSelected(TraceSettings ts)
        {
            return ts.EventsColumns.BatchCompleted
                    || ts.EventsColumns.BatchStarting
                    || ts.EventsColumns.LoginLogout
                    || ts.EventsColumns.ExistingConnection
                    || ts.EventsColumns.RPCCompleted
                    || ts.EventsColumns.RPCStarting
                    || ts.EventsColumns.SPStmtCompleted
                    || ts.EventsColumns.SPStmtStarting
                    || ts.EventsColumns.UserErrorMessage
                    || ts.EventsColumns.BlockedProcessPeport
                    || ts.EventsColumns.SQLStmtStarting
                    || ts.EventsColumns.SQLStmtCompleted;

        }

        private void btnRun_Click(object sender, System.EventArgs e)
        {
            if (!AtLeastOneEventSelected(m_currentsettings))
            {
                System.Console.WriteLine("Error Starting trace: You should select at least 1 event");
                
                // tabControl1.SelectedTab = tabPage2;
                return;
            }
            // DialogResult = DialogResult.OK;
        }

        /*
        public bool IsIncluded(ListViewItem lvi)
        {
            bool included = true;

            //Fragile here to hard coding the columns, but they are currently this way.
            included &= IsIncluded(m_currentsettings.Filters.ApplicationNameFilterCondition, m_currentsettings.Filters.ApplicationName, lvi.SubItems[0].Text);
            included &= IsIncluded(m_currentsettings.Filters.TextDataFilterCondition, m_currentsettings.Filters.TextData, lvi.SubItems[1].Text);
            included &= IsIncluded(m_currentsettings.Filters.LoginNameFilterCondition, m_currentsettings.Filters.LoginName, lvi.SubItems[2].Text);
            included &= IsIncluded(m_currentsettings.Filters.CpuFilterCondition, m_currentsettings.Filters.CPU, lvi.SubItems[3].Text);
            included &= IsIncluded(m_currentsettings.Filters.ReadsFilterCondition, m_currentsettings.Filters.Reads, lvi.SubItems[4].Text);
            included &= IsIncluded(m_currentsettings.Filters.WritesFilterCondition, m_currentsettings.Filters.Writes, lvi.SubItems[5].Text);
            included &= IsIncluded(m_currentsettings.Filters.DurationFilterCondition, m_currentsettings.Filters.Duration, lvi.SubItems[6].Text);
            included &= IsIncluded(m_currentsettings.Filters.SPIDFilterCondition, m_currentsettings.Filters.SPID, lvi.SubItems[7].Text);

            return included;
        }
        */
        

        private bool IsIncluded(StringFilterCondition filterCondition, string filter, string entryToCheck)
        {
            bool included = true; //Until removed.  Negative logic is applied here.
            if (string.IsNullOrEmpty(filter) == false)
            {
                if (filterCondition == StringFilterCondition.Like)
                {
                    if (entryToCheck.Contains(filter) == false)
                    {
                        included = false;
                    }
                }
                else if (filterCondition == StringFilterCondition.NotLike)
                {
                    if (entryToCheck.Contains(filter) == true)
                    {
                        included = false;
                    }
                }
            }
            return included;
        }


        private bool IsIncluded(IntFilterCondition filterCondition, int? filter, string entryToCheck)
        {
            bool included = true; //Until removed.  Negative logic is applied here.

            int intEntry;
            if ((int.TryParse(entryToCheck, out intEntry)) && (filter.HasValue))
            {
                if (filterCondition == IntFilterCondition.Equal)
                {
                    if (filter != intEntry)
                    {
                        included = false;
                    }
                }
                else if (filterCondition == IntFilterCondition.GreaterThan)
                {
                    if (filter >= intEntry) // <= because we are using negative logic here.
                    {
                        included = false;
                    }
                }
                else if (filterCondition == IntFilterCondition.LessThan)
                {
                    if (filter <= intEntry) // >= because we are using negative logic here.
                    {
                        included = false;
                    }
                }
                else if (filterCondition == IntFilterCondition.NotEqual)
                {
                    if (filter == intEntry)
                    {
                        included = false;
                    }
                }
            }
            return included;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
                }

                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~TraceProperties() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        void System.IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
