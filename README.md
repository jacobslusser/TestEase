TestEase - Single File Test Framework
=====================================

TestEase is a [single file](https://github.com/jacobslusser/TestEase/blob/master/TestEase/TestEase.cs) that can be dropped into any project and give you instant support for writing and running unit tests. Add it to a console application and turn it into an instant test runner you can share with your team. Add it to your build process to smoke test your last commit. Wrap it with a GUI! The possibilities are endless.

You are the test writer and runner—all in one. No more clumsy executables, dependencies, or painful configuration.

# Philosophy

I don’t know about you but I find writing unit tests painful. It’s boring. It usually requires knowledge of a particular test suite. It can sometimes require elaborate configuration files. They can be difficult to distribute and run by a QA staff it you would like to share them. Some suites bring in lots of dependencies. And don’t get me started on how hard it can be to integrate unit tests into automated build processes. As a result I tend not to do it.

Separate from unit testing I find I also write a lot of one-off console applications. These are usually created to massage some remote service endpoints, or check for something in the database, or to perform some repetitive function that would normally be time consuming. When you think about it, most of these one-off applications are also unit tests. They are designed to perform a certain function and let you know the outcome.

TestEase is designed to solve all these problems and make it fun. Did we mention it was a single file? It requires zero configuration. It doesn't need a separate test runner application. There are no project dependencies or NuGet packages. Modify it to your heart’s content. 

You create the application. You run the tests. You process the results.  

# Walkthrough

1). Add the `TestEase.cs` file to your project. In this example we’ll be using it in a console application but it can just as easily be run by a GUI or be integrated into a build process step.

2). Create your test methods and mark them with the `TestAttribute`.

    [Test]
    private void InstanceTestMethod()
    {
    }

    [Test(Name = "Alternate test name", Skip = false)]
    private static void StaticTestMethod()
    {
    }

The `TestAttribute` allows two optional parameters: one to provide an alternate test name instead of using the fully qualified type and method name, and another to skip the test if you want to quickly disable it.

Tests should be easy to write and thus we don’t require an attribute on the test class and don’t believe in setup and teardown code. Static methods will be called directly. Instance methods will have their owning type instantiated first by calling the type's parameterless constructor. Following an instance method test, `Dispose` will be called on the owning type if `IDisposable` is implemented.

3). Make your assertions. The `Assert` class already contains the most common ones but you can create your own.

    Assert.Equal(1, 1);

    Assert.True(1 == 1);

    Assert.Throws<DivideByZeroException>(() =>
    {
        int i = 0;
        int j = 1 / i;
    });

An assertion is any method that can throw and `AssertionFailedException`. Thus, you don’t need to use the `Assert` helpers. Just throw an `AssertionFailedException` from your test method and TestEase will capture the result.

4). Run your tests with the `TestRunner` and process the results. 

    foreach (var testResult in new TestRunner().RunAllTests())
        Console.WriteLine(testResult.TestMethod.Name + "... " + (testResult.Pass ? "PASS" : "FAIL"));

The `RunAllTests` method will automatically discover and run all tests with the `TestAttribute`. If you would prefer more control over which tests you want run you can use the `FindTests` and `RunTest` methods to participate in the discovery process and individually run a test.

The `TestResult` objects return by calling `RunTest` or `RunAllTests` contain your results. Pass, fail, exception, etc…. Print them to the console, store them in a file, paste them on your neighbor’s cubical wall.

If you want to see all of this in action, just download the project and run the included console application.

# Requirements

.NET (or Mono) 3.5 or greater.

# Final Thoughts

I won’t lie. The project name is the best I’ve ever come up with.
