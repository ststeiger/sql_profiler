
namespace ExpressProfiler
{
    
    
    public class ConsoleOutputWriter
        : OutputWriter
    {
        private static System.Collections.Generic.Dictionary<
            System.Drawing.Color, System.ConsoleColor> s_colorDict;


        private System.ConsoleColor m_backupForeground;
        private System.ConsoleColor m_backupBackground;


        static ConsoleOutputWriter()
        {
            s_colorDict = SetupColorDictionary();
        }



        public ConsoleOutputWriter()
        {
            this.m_backupForeground = System.Console.ForegroundColor;
            this.m_backupBackground = System.Console.BackgroundColor;
        }


        private static System.Collections.Generic.Dictionary<
            System.Drawing.Color, System.ConsoleColor> 
        SetupColorDictionary()
        {
            System.Collections.Generic.Dictionary<System.Drawing.Color, System.ConsoleColor>
                dict = new System.Collections.Generic.Dictionary<
                    System.Drawing.Color, System.ConsoleColor
                >();
            
            dict.Add(System.Drawing.Color.Black, System.ConsoleColor.Black);
            dict.Add(System.Drawing.Color.DarkBlue, System.ConsoleColor.DarkBlue);
            dict.Add(System.Drawing.Color.DarkGreen, System.ConsoleColor.DarkGreen);
            dict.Add(System.Drawing.Color.DarkCyan, System.ConsoleColor.DarkCyan);
            dict.Add(System.Drawing.Color.DarkRed, System.ConsoleColor.DarkRed);
            dict.Add(System.Drawing.Color.DarkMagenta, System.ConsoleColor.DarkMagenta);
            dict.Add(System.Drawing.Color.FromArgb(215, 195, 42), System.ConsoleColor.DarkYellow);
            dict.Add(System.Drawing.Color.Gray, System.ConsoleColor.Gray);
            dict.Add(System.Drawing.Color.DarkGray, System.ConsoleColor.DarkGray);
            dict.Add(System.Drawing.Color.Blue, System.ConsoleColor.Blue);
            dict.Add(System.Drawing.Color.Green, System.ConsoleColor.Green);
            dict.Add(System.Drawing.Color.Cyan, System.ConsoleColor.Cyan);
            dict.Add(System.Drawing.Color.Red, System.ConsoleColor.Red);
            dict.Add(System.Drawing.Color.Magenta, System.ConsoleColor.Magenta);
            dict.Add(System.Drawing.Color.Yellow, System.ConsoleColor.Yellow);
            dict.Add(System.Drawing.Color.White, System.ConsoleColor.White);
            dict.Add(System.Drawing.Color.Fuchsia, System.ConsoleColor.Red); // Correct ? 

            return dict;
        }


        public override System.Drawing.Color ForeColor
        {


            set
            {
                // if (!s_colorDict.ContainsKey(value)) System.Console.WriteLine(value);
                System.Console.ForegroundColor = s_colorDict[value];
            }
        }


        public override System.Drawing.Color BackColor
        {
            set
            {
                // if (!s_colorDict.ContainsKey(value)) System.Console.WriteLine(value);
                System.Console.BackgroundColor = s_colorDict[value];
                this.AppendLine();
                //System.Console.WriteLine(new string(' ', System.Console.BufferWidth));
            }
        }


        public override void AppendLine()
        {
            // Finish the line with empty color
            System.Console.Write(new string(' ', System.Console.BufferWidth - System.Console.CursorLeft));
            // System.Console.WriteLine();
        }


        public override void Append(string text)
        {
            System.Console.Write(text);
        }


        public override string ToString()
        {
            System.Console.ForegroundColor = this.m_backupForeground;
            System.Console.BackgroundColor = this.m_backupBackground;

            return "";
        }
        
        
    }
    
    
}