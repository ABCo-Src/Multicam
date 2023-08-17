using Moq;
using Moq.Language;
using Moq.Language.Flow;
using System.Linq.Expressions;

namespace ABCo.Multicam.Tests.Helpers
{
    public static class MoqExtensions
    {
        public static void New<T>(this Mock mock, Expression<Func<T, bool>> expr) where T : class
            => Mock.Get(Mock.Of(expr));

        public static void SetupTrueAsync<TMock, TRet>(this Mock<TMock> r, Expression<Func<TMock, Task<TRet>>> f) 
            where TMock : class
        {
            r.Setup(f).Returns(async () =>
            {
                await Task.Yield();
                return default!;
            });
        }

        public static ISetup<TMock> SetupDefaultArgs<TMock>(this Mock<TMock> mock, Func<TMock, Delegate> getMethod)
            where TMock : class
        {
            var method = typeof(TMock).GetMethod(getMethod(Mock.Of<TMock>()).Method.Name)!;

            var instance = Expression.Parameter(typeof(TMock), "m");
            var callExp = Expression.Call(instance, method, method.GetParameters().Select(p => GenerateItIsAny(p.ParameterType)));
            var exp = Expression.Lambda<Action<TMock>>(callExp, instance);
            return mock.Setup(exp);

            MethodCallExpression GenerateItIsAny(Type T) => Expression.Call(typeof(It).GetMethod("IsAny")!.MakeGenericMethod(T));
        }

        public static IReturns<TMock, TRet> SetupDefaultArgs<TMock, TRet>(this Mock<TMock> mock, Func<TMock, Delegate> getMethod)
            where TMock : class
        {
            var method = getMethod(Mock.Of<TMock>()).Method;

            var instance = Expression.Parameter(typeof(TMock), "m");
            var callExp = Expression.Call(instance, method, method.GetParameters().Select(p => GenerateItIsAny(p.ParameterType)));
            var exp = Expression.Lambda<Func<TMock, TRet>>(callExp, instance);
            return mock.Setup(exp);

            MethodCallExpression GenerateItIsAny(Type T) => Expression.Call(typeof(It).GetMethod("IsAny")!.MakeGenericMethod(T));
        }

        public static TRet ReturnsTrueAsync<TMock, TRet>(this IReturns<TMock, Task<TRet>> r, TRet val) where TMock : class
        {
            r.Returns(async () =>
            {
                await Task.Yield();
                return val;
            });
            return val;
        }
    }
}
