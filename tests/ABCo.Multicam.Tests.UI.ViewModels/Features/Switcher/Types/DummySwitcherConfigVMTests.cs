//using ABCo.Multicam.Core.Features.Switchers;
//using ABCo.Multicam.Core.Features.Switchers.Types;
//using ABCo.Multicam.UI.ViewModels.Features.Switcher;
//using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ABCo.Multicam.Tests.UI.ViewModels.Features.Switcher.Types
//{
//    [TestClass]
//    public class DummySwitcherConfigVMTests
//    {
//        DummySwitcherConfig _config;
//        public record struct Mocks(Mock<ISwitcherFeatureVM> Parent);

//        Mocks _mocks;

//        [TestInitialize]
//        public void InitMocks()
//        {
//            _mocks.Parent = new();
//        }

//        DummySwitcherConfigVM Create()
//        {
//            var vm = new DummySwitcherConfigVM();
//            vm.FinishConstruction(_config, _mocks.Parent.Object);
//            return vm;
//        }

//        [TestMethod]
//        public void Ctor()
//        {
//            _config = new(4, 2);

//            var vm = Create();

//            _mocks.Parent.VerifySet(m => m.RawConfig = It.IsAny<SwitcherConfig>(), Times.Never);
//        }
//    }
//}
