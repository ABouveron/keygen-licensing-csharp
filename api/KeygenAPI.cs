// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace example_csharp_licensing_Docker.api;

public abstract class KeygenApi
{
    public static void Main(string[] args)
    {
        Env.Load();
        Console.WriteLine(Product.ProductCreation("test", "https://test.com").Content);
    }
}
