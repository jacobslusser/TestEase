#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Linq;

#endregion Using Directives

namespace TestEase
{
    #region Assert

    /// <summary>
    /// Helper class to assert specific test conditions.
    /// </summary>
    public static class Assert
    {
        #region Methods

        /// <summary>
        /// Tests whether the values are equal.
        /// </summary>
        /// <typeparam name="T">The type of the values to compare.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="comparer">Comparer used to determine equality. (optional)</param>
        public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            if (!comparer.Equals(expected, actual))
                throw new AssertionFailedException(expected, actual);
        }

        /// <summary>
        /// This method is not an assertion test.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is not an assertion test.", true)]
        public new static bool Equals(object a, object b)
        {
            throw new InvalidOperationException("This method is not an assertion test.");
        }

        /// <summary>
        /// This method is not an assertion test.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is not an assertion test.", true)]
        public new static bool ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("This method is not an assertion test.");
        }

        /// <summary>
        /// Tests whether the specified exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception expected.</typeparam>
        /// <param name="action">The test code to execute.</param>
        public static void Throws<T>(Action action) where T : Exception
        {
            Exception exception = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
                throw new AssertionFailedException(typeof(T), null);
            else if (!(exception is T))
                throw new AssertionFailedException(typeof(T), exception.GetType());
        }

        /// <summary>
        /// Tests whether the condition is true.
        /// </summary>
        /// <param name="condition">The condition to test.</param>
        public static void True(bool condition)
        {
            if (!condition)
                throw new AssertionFailedException(true, false);
        }

        #endregion Methods
    }

    #endregion Assert

    #region AssertionFailedException

    /// <summary>
    /// Represents the result of an test assertion failure.
    /// </summary>
    [Serializable]
    public class AssertionFailedException : Exception
    {
        #region Methods

        private static string ConvertToString(object value)
        {
            if (value == null)
                return "(null)";

            return Convert.ToString(value);
        }

        private static string GetMessage(object expected, object actual)
        {
            return string.Format(
                "Assertion failed! Expected: {0}, Actual: {1}.",
                ConvertToString(expected),
                ConvertToString(actual));
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets a string representation of the actual asserted value.
        /// </summary>
        /// <returns>The actual value which caused assertion to fail.</returns>
        public string Actual { get; private set; }

        /// <summary>
        /// Gets a string representation of the expected asserted value.
        /// </summary>
        /// <returns>The expected value asserted.</returns>
        public string Expected { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AssertionFailedException" /> class.
        /// </summary>
        /// <param name="expected">The expected value asserted.</param>
        /// <param name="actual">The actual value that failed assertion.</param>
        public AssertionFailedException(object expected, object actual) : base(GetMessage(expected, actual))
        {
            Expected = ConvertToString(expected);
            Actual = ConvertToString(actual);
        }

        #endregion Construtors
    }

    #endregion AssertionFailedException

    #region TestAttribute

    /// <summary>
    /// Used to identify test methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets an alternate display name for the test.
        /// </summary>
        /// <returns>An alternate display name if set; otherwise, null. The default is null.</returns>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the test should be skipped (not run).
        /// </summary>
        /// <returns>true to skip the test; otherwise, false. The default is false.</returns>
        public bool Skip { get; set; }
    }

    #endregion TestAttribute

    #region TestMethod

    /// <summary>
    /// Contains information about a method to test.
    /// </summary>
    public class TestMethod
    {
        #region Properties

        /// <summary>
        /// The reflected test method metadata.
        /// </summary>
        /// <returns>The reflected test method metadata.</returns>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// Gets the test name.
        /// </summary>
        /// <returns>The test name.</returns>
        public string Name { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod" /> class.
        /// </summary>
        /// <param name="methodInfo">The method to test.</param>
        /// <param name="name">An alternate name for the test method. (optional)</param>
        public TestMethod(MethodInfo methodInfo, string name = null)
        {
            MethodInfo = methodInfo;
            Name = name ?? (methodInfo.DeclaringType.Namespace + "." + methodInfo.Name);
        }

        #endregion Constructors
    }

    #endregion TestMethod

    #region TestResult

    /// <summary>
    /// Represents the result of a test method.
    /// </summary>
    public class TestResult
    {
        #region Properties

        /// <summary>
        /// Inidicates whether the test failed as the result of an assertion.
        /// </summary>
        /// <returns>true if the test failed because an <see cref="AssertionFailedException" /> was thrown; otherwise, false.</returns>
        public bool AssertionFailed
        {
            get
            {
                return (Exception != null && Exception.GetBaseException() is AssertionFailedException);
            }
        }

        /// <summary>
        /// Gets the exception (if any) that was generated by the test.
        /// </summary>
        /// <returns>The exception generated by the test; otherwise, null.</returns>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the test failed.
        /// </summary>
        /// <returns>true if the test failed; otherwise, false.</returns>
        public bool Fail
        {
            get
            {
                return Exception != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the test passed.
        /// </summary>
        /// <returns>true if the test passed; otherwise, false.</returns>
        public bool Pass
        {
            get
            {
                return !Fail;
            }
        }

        /// <summary>
        /// Gets the method tested.
        /// </summary>
        /// <returns>The method tested.</returns>
        public TestMethod TestMethod { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult" /> class.
        /// </summary>
        /// <param name="testMethod">The test method which generated the result.</param>
        /// <param name="exception">Any exception that was generated performing the test. (optional)</param>
        public TestResult(TestMethod testMethod, Exception exception = null)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            TestMethod = testMethod;
            Exception = exception;
        }

        #endregion Constructors
    }

    #endregion TestResult

    #region TestRunner

    /// <summary>
    /// A test client capable of finding and running test methods.
    /// </summary>
    public class TestRunner
    {
        #region Methods

        /// <summary>
        /// Finds all methods marked with a <see cref="TestAttribute" /> within the app domain.
        /// </summary>
        /// <param name="includeSkipped">
        /// true to include tests marked to be skiped; otherwise, false. The default is false. (optional)
        /// </param>
        /// <returns>A list of test methods.</returns>
        public static IList<TestMethod> FindTests(bool includeSkipped = false)
        {
            var testMethods = new List<TestMethod>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        var testAttributes = methodInfo.GetCustomAttributes(typeof(TestAttribute), false);
                        if (testAttributes == null || testAttributes.Length < 1)
                            continue;

                        var testAttribute = (TestAttribute)testAttributes[0];
                        if (testAttribute.Skip && !includeSkipped)
                            continue;

                        testMethods.Add(new TestMethod(methodInfo, testAttribute.Name));
                    }
                }
            }

            return testMethods;
        }

        /// <summary>
        /// Runs all the tests found (or specified) and returns the results.
        /// </summary>
        /// <param name="testMethods">
        /// List of methods to test. If not specified, any method marked with a <see cref="TestAttribute" /> within the app domain is used. (optional)
        /// </param>
        /// <returns>A list of test results.</returns>
        public IList<TestResult> RunAllTests(IList<TestMethod> testMethods = null)
        {
            if (testMethods == null)
                testMethods = FindTests();

            var testResults = new List<TestResult>();
            foreach (var testMethod in testMethods)
                testResults.Add(RunTest(testMethod));

            return testResults;
        }

        /// <summary>
        /// Runs a single test method and returns the test result.
        /// </summary>
        /// <param name="testMethod">The method to test.</param>
        /// <returns>The result of the test.</returns>
        public TestResult RunTest(TestMethod testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            object instance;
            Exception exception = null;

            try
            {
                // Make sure the test method has no I/O params
                if (testMethod.MethodInfo.GetParameters().Length > 0 || testMethod.MethodInfo.ReturnType != typeof(void))
                    throw new ArgumentException("Test methods may not have input or output parameters.", "testMethod");

                if (!testMethod.MethodInfo.IsStatic)
                {
                    instance = Activator.CreateInstance(testMethod.MethodInfo.DeclaringType);
                    testMethod.MethodInfo.Invoke(instance, null);
                    if (instance is IDisposable)
                        ((IDisposable)instance).Dispose();
                }
                else
                {
                    testMethod.MethodInfo.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                // Since we are dynamically invoking test methods, any exceptions
                // generated will be wrapped by a TargetInvocationException.
                exception = ex.InnerException;
            }

            return new TestResult(testMethod, exception);
        }

        #endregion Methods
    }

    #endregion TestRunner
}
