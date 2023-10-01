﻿using ABCo.Multicam.Server;
using ABCo.Multicam.UI.ViewModels;
using Moq;

namespace ABCo.Multicam.Tests.UI.ViewModels
{
	[TestClass]
    public class ApplicationVMTests
    {
        [TestMethod]
        public void Ctor_Normal()
        {
            var vm = new ApplicationVM(Mock.Of<IServerInfo>(m => m.Get<IProjectVM>() == Mock.Of<IProjectVM>()));
            Assert.IsNotNull(vm.Project);
        }
    }
}
