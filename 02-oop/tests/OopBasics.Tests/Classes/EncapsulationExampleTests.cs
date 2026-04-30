using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class EncapsulationExampleTests
{
    [Fact]
    public void BankAccount_DepositAndWithdraw_UpdatesBalance()
    {
        var account = new BankAccount("Alice", 1000);
        account.Deposit(500);
        Assert.Equal(1500, account.Balance);
        account.Withdraw(300);
        Assert.Equal(1200, account.Balance);
    }

    [Fact]
    public void BankAccount_Withdraw_ThrowsOnOverdraw()
    {
        var account = new BankAccount("Alice", 1000);
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(5000));
    }
}
