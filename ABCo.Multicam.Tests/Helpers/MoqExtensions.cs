using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core.Strips.Switchers;
using Moq;
using Moq.Language;

namespace ABCo.Multicam.Tests.Helpers
{
    public static class MoqExtensions
    {
        public static void SetupTrueAsync<TMock, TRet>(this Mock<TMock> r, Expression<Func<TMock, Task<TRet>>> f) 
            where TMock : class
        {
            r.Setup(f).Returns(async () =>
            {
                await Task.Yield();
                return default!;
            });
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
