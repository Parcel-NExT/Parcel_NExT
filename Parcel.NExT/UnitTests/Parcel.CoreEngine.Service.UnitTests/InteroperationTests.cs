using Parcel.CoreEngine.Service.Interpretation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class InteroperationTests
    {
        [Fact]
        public void ShouldCoerceArrayLikeIncomingArgumentAsLoopedHandlingPerScalarFunction()
        {
            {
                MethodInfo stringTestMethod = GetType().GetMethod(nameof(TestSingleStringParameterFunc))!;
                Type[] stringTestTypes = [
                    typeof(string[]),
                    typeof(List<string>),
                    typeof(HashSet<string>),
                ];
                foreach (Type testType in stringTestTypes)
                    Assert.True(InteroperationHelper.ShouldCoerce(testType, stringTestMethod));
            }

            {
                MethodInfo numberTestMethod = GetType().GetMethod(nameof(TestSingleNumberParameterFunc))!;
                Type[] numberTestTypes = [
                    typeof(double[]),
                    typeof(List<double>),
                    typeof(IEnumerable<double>)
                ];
                foreach (Type testType in numberTestTypes)
                    Assert.True(InteroperationHelper.ShouldCoerce(testType, numberTestMethod));
            }
        }

        #region Tests
        public static int TestSingleStringParameterFunc(string inputString)
        {
            return 0;
        }
        public static double TestSingleNumberParameterFunc(double value)
        {
            return 0;
        }
        #endregion
    }
}
