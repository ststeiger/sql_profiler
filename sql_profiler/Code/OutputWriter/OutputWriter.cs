
namespace ExpressProfiler
{
    
    
    public abstract class OutputWriter
    {
        public abstract System.Drawing.Color ForeColor { set; }
        public abstract System.Drawing.Color BackColor { set; }
        public abstract void AppendLine();
        public abstract void Append(string text);
        public new abstract string ToString();
    }
    
    
}
