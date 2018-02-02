
namespace sql_profiler
{


    /// <summary>
    /// Arguments class
    /// </summary>
    public class CommandLineArguments
    {


        private System.Collections.Specialized.StringDictionary Parameters;


        public CommandLineArguments(string[] Args)
            : this(Args, null)
        { } // End Constructor 


        public CommandLineArguments(string[] Args
            , System.Collections.Generic.Dictionary<string, string> defaultValues)
        {
            Parameters = new System.Collections.Specialized.StringDictionary();
            System.Text.RegularExpressions.Regex Spliter =
                new System.Text.RegularExpressions.Regex(@"^-{1,2}|^/|=|:",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    | System.Text.RegularExpressions.RegexOptions.Compiled
            );

            System.Text.RegularExpressions.Regex Remover =
                new System.Text.RegularExpressions.Regex(@"^['""]?(.*?)['""]?$",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    | System.Text.RegularExpressions.RegexOptions.Compiled
            );

            string Parameter = null;
            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string Txt in Args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                Parts = Spliter.Split(Txt, 3);

                switch (Parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                            {
                                Parts[0] =
                                    Remover.Replace(Parts[0], "$1");

                                Parameters.Add(Parameter, Parts[0]);
                            }
                            Parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                                Parameters.Add(Parameter, "true");
                        }
                        Parameter = Parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                                Parameters.Add(Parameter, "true");
                        } // End if(Parameter != null) 

                        Parameter = Parts[1];

                        // Remove possible enclosing characters (",')
                        if (!Parameters.ContainsKey(Parameter))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            Parameters.Add(Parameter, Parts[2]);
                        } // End if(!Parameters.ContainsKey(Parameter)) 

                        Parameter = null;
                        break;
                } // End switch(Parts.Length) 

            } // Next Txt 

            // In case a parameter is still waiting
            if (Parameter != null)
            {
                if (!Parameters.ContainsKey(Parameter))
                    Parameters.Add(Parameter, "true");
            } // End if(Parameter != null) 

            // Add default values
            if (defaultValues != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in defaultValues)
                {
                    if (!this.Parameters.ContainsKey(kvp.Key))
                        this.Parameters[kvp.Key] = kvp.Value;
                } // Next kvp 
            } // End if (defaultValues != null) 

        } // End Constructor 


        public int? GetInt(string param)
        {
            string str = this[param];
            int iValue;
            if (int.TryParse(str, out iValue))
                return iValue;

            return null;
        } // End Function GetInt 


        public long? GetLong(string param)
        {
            string str = this[param];
            long lngValue;
            if (long.TryParse(str, out lngValue))
                return lngValue;

            return null;
        } // End Function GetLong 


        public bool? GetBool(string param)
        {
            string str = this[param];
            bool bValue;
            if (bool.TryParse(str, out bValue))
                return bValue;

            return null;
        } // End Function GetBool 


        public T GetValue<T>(string name)
        {
            System.Type t = typeof(T);
            bool nullable = false;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Nullable<>))
            {
                nullable = true;
                t = System.Nullable.GetUnderlyingType(t);
            }


            if (!this.ContainsKey(name))
                return (T)(object)null;

            if (object.ReferenceEquals(t, typeof(string)))
            {
                return (T)(object)this[name];
            }

            if (object.ReferenceEquals(t, typeof(bool)))
            {
                bool? val = GetBool(name);
                return (T)(object)val;
            }

            if (object.ReferenceEquals(t, typeof(int)))
            {
                int? val = GetInt(name);
                return (T)(object)val;
            }

            if (object.ReferenceEquals(t, typeof(long)))
            {
                long? val = GetLong(name);
                return (T)(object)val;
            }

            throw new System.NotSupportedException($"Type {t.FullName} not supported.");
            return (T)(object)null;
        } // End Function GetValue 


        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string Param]
        {
            get
            {
                return (Parameters[Param]);
            }
            set
            {
                this.Parameters[Param] = value;
            }
        } // End Default-Property 


        public bool ContainsKey(string key)
        {
            return Parameters.ContainsKey(key);
        } // End Function ContainsKey 


    } // End Class CommandLineArguments


} // End Namespace sql_profiler 
