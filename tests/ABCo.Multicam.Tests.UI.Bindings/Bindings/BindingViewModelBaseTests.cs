using ABCo.Multicam.UI.Bindings;
using Moq;

namespace ABCo.Multicam.Tests.UI.Bindings
{
    [TestClass]
    public class BindingViewModelBaseTests
    {
        public class Sub : BindingViewModelBase<Sub>, IVMForBinder<Sub>
        {
            public Sub() { }
        }

        record struct Mocks(Mock<IVMBinder<Sub>> Binder);

        Mocks _mocks = new();

        Sub Create()
        {
            var vm = new Sub();
            vm.InitBinding(_mocks.Binder.Object);
            return vm;
        }

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.Binder = new();
        }

        [TestMethod]
        public void EnableBinding_PreInit()
        {
            Assert.ThrowsException<Exception>(() => new Sub().ReenableModelBindingAndSend());
        }

        [TestMethod]
        public void DisableBinding_PreInit()
        {
            Assert.ThrowsException<Exception>(() => new Sub().DisableModelBinding());
        }

        [TestMethod]
        public void Dispose_PreInit()
        {
            new Sub().Dispose();
        }

        [TestMethod]
        public void EnableBinding()
        {
            var vm = Create();
            vm.ReenableModelBindingAndSend("ghi", "abc");
            _mocks.Binder.Verify(m => m.EnableVMAndSendToModel(vm, new string[] { "ghi", "abc" }));
        }

        [TestMethod]
        public void DisableBinding()
        {
            var vm = Create();
            vm.DisableModelBinding();
            _mocks.Binder.Verify(m => m.DisableVM(vm));
        }

        [TestMethod]
        public void Dispose()
        {
            var vm = Create();
            vm.Dispose();
            _mocks.Binder.Verify(m => m.RemoveVM(vm));
        }
    }
}
