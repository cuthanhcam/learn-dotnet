namespace PART12POLYMORPHISM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var rand = new Random();

            //Animal animal = GetAnimal(rand.Next(0, 3));

            //animal.Move();

            Animal animal = new Animal();
            animal.A();

            Animal dog = new Dog();
            dog.A();

            Bird bird = new Bird(); // Bird is derived from Animal, so it can be assigned to an Animal reference
            bird.A();
        }

        static Animal GetAnimal(int id)
        {
            switch (id)
            {
                case 0:
                    return new Dog();
                case 1:
                    return new Bird();
                case 2:
                    return new Fish();
                default:
                    return new Animal();
            }
        }
    }
}
