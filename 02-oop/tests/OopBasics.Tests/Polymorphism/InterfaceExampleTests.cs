using OopBasics.Examples.Polymorphism;
using Xunit;

namespace OopBasics.Tests.Polymorphism;

public class InterfaceExampleTests
{
    [Fact]
    public void CreditCardPayment_And_PaypalPayment_ImplementIPaymentService()
    {
        IPaymentService cc = new CreditCardPayment();
        IPaymentService paypal = new PaypalPayment();
        cc.Pay(100);
        paypal.Pay(200);
    }
}
