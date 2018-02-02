
// Traceutils assembly
// writen by Locky, 2009. 

namespace ExpressProfiler
{
    
    
    class RTFBuilder
        : OutputWriter
    {
        private readonly System.Text.StringBuilder m_Sb = new System.Text.StringBuilder();
        private readonly System.Collections.Generic.List<System.Drawing.Color> 
            m_Colortable = new System.Collections.Generic.List<System.Drawing.Color>();
        private readonly System.Collections.Specialized.StringCollection m_Fonttable = 
            new System.Collections.Specialized.StringCollection();
        private System.Drawing.Color m_Forecolor;

        public override System.Drawing.Color ForeColor
        {
            set
            {
                if (!m_Colortable.Contains(value))
                {
                    m_Colortable.Add(value);
                }

                if (value != m_Forecolor)
                {
                    m_Sb.Append(string.Format("\\cf{0} ", m_Colortable.IndexOf(value) + 1));
                }

                m_Forecolor = value;
            }
        }


        private System.Drawing.Color m_Backcolor;

        public override System.Drawing.Color BackColor
        {
            set
            {
                if (!m_Colortable.Contains(value))
                {
                    m_Colortable.Add(value);
                }

                if (value != m_Backcolor)
                {
                    m_Sb.Append(string.Format("\\highlight{0} ", m_Colortable.IndexOf(value) + 1));
                }

                m_Backcolor = value;
            }
        }


        public RTFBuilder()
        {
            ForeColor = System.Drawing.Color.Black; // Color.FromKnownColor(KnownColor.WindowText);
            BackColor = System.Drawing.Color.White; // Color.FromKnownColor(KnownColor.Window);
            m_DefaultFontSize = 20F;
        }

        public override void AppendLine()
        {
            m_Sb.AppendLine("\\line ");
        }

        public override void Append(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\r\n", "\n");

                // A sole carriage return should not be treated as Environment.NewLine
                value = value.Replace("\r", "");
                value = CheckChar(value);

                int newlineIndex = -1;
                while ((newlineIndex = value.IndexOf('\n')) != -1)
                {
                    string valueToAdd = value.Substring(0, newlineIndex);
                    if (!string.IsNullOrEmpty(valueToAdd))
                        m_Sb.Append(valueToAdd);

                    m_Sb.Append("\\line ");

                    value = value.Substring(newlineIndex + 1);
                } // Whend 

                if (!string.IsNullOrEmpty(value))
                    m_Sb.Append(value);
            }
        }

        private static readonly char[] Slashable = new[] {'{', '}', '\\'};
        private readonly float m_DefaultFontSize;

        private static string CheckChar(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.IndexOfAny(Slashable) >= 0)
                {
                    value = value.Replace("{", "\\{").Replace("}", "\\}").Replace("\\", "\\\\");
                }

                bool replaceuni = false;
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] > 255)
                    {
                        replaceuni = true;
                        break;
                    }
                }

                if (replaceuni)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] <= 255)
                        {
                            sb.Append(value[i]);
                        }
                        else
                        {
                            sb.Append("\\u");
                            sb.Append((int) value[i]);
                            sb.Append("?");
                        }
                    }

                    value = sb.ToString();
                }
            }


            return value;
        }

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang3081");
            result.Append("{\\fonttbl");
            for (int i = 0; i < m_Fonttable.Count; i++)
            {
                try
                {
                    result.Append(string.Format(m_Fonttable[i], i));
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }

            result.AppendLine("}");
            result.Append("{\\colortbl ;");
            foreach (System.Drawing.Color item in m_Colortable)
            {
                result.AppendFormat("\\red{0}\\green{1}\\blue{2};", item.R, item.G, item.B);
            }

            result.AppendLine("}");
            result.Append("\\viewkind4\\uc1\\pard\\plain\\f0");
            result.AppendFormat("\\fs{0} ", m_DefaultFontSize);
            result.AppendLine();
            result.Append(m_Sb.ToString());
            result.Append("}");
            return result.ToString();
        }
        
        
    }
    
    
}
