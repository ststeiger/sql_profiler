
namespace ExpressProfiler
{


    public enum HorizontalAlignment
    {
        Left = 0,
        Right = 1,
        Center = 2
    }


    public class PerfColumn
    {
        public string Caption;
        public int Column;
        public int Width;
        public string Format;
        public HorizontalAlignment Alignment = HorizontalAlignment.Left;
    }


}
