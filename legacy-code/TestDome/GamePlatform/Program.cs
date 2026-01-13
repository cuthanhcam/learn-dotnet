namespace GamePlatform
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(CalculateFinalSpeed(60, new int[] { 0, 30, 0, -45, 0 }));
        }

        public static double CalculateFinalSpeed(double initialSpeed, int[] inclinations)
        {
            double speed = initialSpeed;
            for (int i = 0; i < inclinations.Length; i++)
            {
                speed -= inclinations[i];

                if (speed < 0)
                {
                    speed = 0;
                }
            }
            return speed;
        }
    }
}
