namespace Learning.Patterns.Behavioral.Strategy.Refactored;

/// <summary>
/// Selection belongs to composition/application policy, not to the pricing context. Centralizing a
/// small explicit switch is acceptable: Strategy removes algorithm coupling, not all conditional code.
/// Selection is not authorization; a real API must not trust a client claiming partner membership.
/// </summary>
public static class DiscountPolicySelector
{
    public static IDiscountPolicy Select(CustomerCategory category) => category switch
    {
        CustomerCategory.Standard => new StandardDiscountPolicy(),
        CustomerCategory.Member => new MemberDiscountPolicy(),
        CustomerCategory.Partner => new PartnerDiscountPolicy(),
        _ => throw new ArgumentOutOfRangeException(nameof(category))
    };
}
