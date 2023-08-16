using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.ViewModels.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings.Features
{
    [TestClass]
    public class FeatureBinderTests
    {
        record struct Mocks(Mock<IFeatureManager> Manager, Mock<IFeatureContainer> Container);
        Mocks _mocks = new();

        [TestInitialize]
        public void SetupMocks()
        {
            _mocks.Manager = new();
            _mocks.Container = new();
        }

        public FeatureVMBinder Create()
        {
            var vm = new FeatureVMBinder(Mock.Of<IServiceSource>());
            vm.FinishConstruction(_mocks.Manager.Object, _mocks.Container.Object);
            return vm;
        }

        [TestMethod]
        public void Init_NoException() => Create();
    }
}