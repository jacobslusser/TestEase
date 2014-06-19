#region using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion Using Directives

namespace TestEase
{
    public class Sample
    {
        #region Methods

        [Test(Name = "Example of Assert.Equal")]
        private void Example()
        {
            Assert.Equal(1, 1);
        }

        [Test(Name = "Example of Assert.True")]
        private static void Example2()
        {
            Assert.True(1 == 1);
        }

        [Test(Name = "Example of Assert.Throws", Skip = false)]
        private static void Example3()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                int i = 0;
                int j = 1 / i;
            });
        }

        private static void Main(string[] args)
        {
            foreach (var testResult in new TestRunner().RunAllTests())
                Console.WriteLine(testResult.TestMethod.Name + "... " + (testResult.Pass ? "PASS" : "FAIL"));

            Console.ReadKey();
        }

        #endregion Methods
    }
}
