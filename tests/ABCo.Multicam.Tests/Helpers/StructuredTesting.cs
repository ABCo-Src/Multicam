using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.Helpers
{
    public static class StructuredTesting
    {
        public static void Test()
        {

        }
        //public static void InitializeAndCheck<TSource, TTarget>(Action<TSource> call, Expression<Action<TTarget>> expr, object[] mocks)
        //    where TSource : class
        //    where TTarget : class
        //{
        //    // Create an instance of the source with all mocks
        //    var srcConstructor = typeof(TSource).GetConstructors()[0];
        //    var constructorParams = srcConstructor.GetParameters();

        //    object? targetParam = null;
        //    var parameters = new object[constructorParams.Length];

        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        // Find the matching mock
        //        for (int j = 0; j < mocks.Length; j++)
        //            if (mocks[j].GetType().IsSubclassOf(constructorParams[i].ParameterType))
        //            {
        //                parameters[i] = mocks[j];

        //                if (mocks[j].GetType().IsSubclassOf(typeof(TTarget)))
        //                    break;
        //            }

        //        throw new Exception("No matching mock found");
        //    }

        //    if (targetParam == null) throw new Exception("Target mock not found");

        //    var newObj = srcConstructor.Invoke(parameters);
        //    call((TSource)newObj);
        //    Mock.Get((TTarget)targetParam).Verify(expr);
        //}

        //public static void InitializeAndCheck<TSource, TTarget>()
        //{
        //    // Get the constructor of the source
        //    var srcConstructor = typeof(TSource);
        //}
    }
}
