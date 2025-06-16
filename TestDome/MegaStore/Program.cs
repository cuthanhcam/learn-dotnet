using System;

public class MegaStore
{
    public enum DiscountType
    {
        Standard, // 6
        Seasonal, // 12
        Weight // <= 10 = 6, > 10 = 18
    }

    public static double GetDiscountedPrice(double cartWeight,
                                            double totalPrice,
                                            DiscountType discountType)
    {
        double discountedPrice = totalPrice;
        if (discountType == DiscountType.Standard)
        {
            discountedPrice *= 0.94;
        }
        else if (discountType == DiscountType.Seasonal)
        {
            discountedPrice *= 0.88;
        }
        else if (discountType == DiscountType.Weight)
        {
            if (cartWeight <= 10)
            {
                discountedPrice *= 0.94;
            }
            else
            {
                discountedPrice *= 0.82;
            }
        }
        return discountedPrice;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(GetDiscountedPrice(12, 100, DiscountType.Weight));
    }
}