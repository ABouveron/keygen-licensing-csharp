// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace example_csharp_licensing_Docker.api;

public abstract class TestApi
{
    public static void MainApi(string[] args)
    {
        var license = License.Retrieve("d8347b2c-4670-471b-bce7-e9607e38238d");
        license.PrintInfos();
    }
}