using OopBasics.Exercises;

Console.WriteLine("OOP exercise runner");
Console.WriteLine(new string('=', 40));

var learner = new ClassesExercises.Person("Ada", 27);
learner.CelebrateBirthday();
Console.WriteLine($"Person: {learner.Name}, age {learner.Age}");

var account = new EncapsulationExercises.BankAccount("Ada", 100m);
account.Deposit(25m);
account.Withdraw(40m);
Console.WriteLine($"Account owner: {account.Owner}, balance: {account.Balance:C}");

InheritanceExercises.Animal[] animals =
[
    new InheritanceExercises.Dog("Rex", 4),
    new InheritanceExercises.Cat("Milo", 2)
];

foreach (InheritanceExercises.Animal animal in animals)
{
    // The runtime selects the override for the concrete animal.
    Console.WriteLine(animal.Speak());
}

PolymorphismExercises.Shape[] shapes =
[
    new PolymorphismExercises.Circle(2),
    new PolymorphismExercises.Rectangle(3, 4)
];

Console.WriteLine($"Combined shape area: {PolymorphismExercises.SumAreas(shapes):F2}");
