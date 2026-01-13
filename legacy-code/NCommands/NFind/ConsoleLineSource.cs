namespace NFind
{
    internal class ConsoleLineSource : ILineSource
    {
        private int number = 0;

        public ConsoleLineSource()
        {
        }

        public string Name => string.Empty;

        public void Close()
        {
        }

        public void Open()
        {
        }

        public Line? ReadLine()
        {
            var s = Console.ReadLine();

            if (s == null)
            {
                return null;
            }
            else
            {
                return new Line() { LineNumber = ++number, Text = s }; 
            }
        }
    }
}