namespace example_csharp_licensing_Docker;

public class Test
{
    private readonly ITestOutputHelper _consoleOutput;

    public Test(ITestOutputHelper consoleOutput)
    {
        _consoleOutput = consoleOutput;
    }

    [Fact]
    public void UnitTest()
    {
        // Definition of all constant and variables needed to test
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Run the application
        MachineFile.Verification(
            new[]
            {
                "examples/license.lic",
                "examples/machine.lic",
                "198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39"
            }
        );

        // Assert the output is what we expected
        var output = stringWriter.ToString();
        _consoleOutput.WriteLine(output);
        const string expectedOutput = "Hello, World!";
        Assert.Contains(expectedOutput, output);
    }
}