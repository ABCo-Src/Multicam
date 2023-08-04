using ABCo.Multicam.UI.Bindings;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings
{
    [TestClass]
    public class BindingViewModelBaseTests
    {
        class Sub : BindingViewModelBase<IBindableVM>, IBindableVM
        {
            public Sub(IVMBinder<IBindableVM> binder) : base(binder) { }
        }

        record struct Mocks(Mock<IVMBinder<IBindableVM>> Binder);

        Mocks _mocks = new();

        Sub Create() => new Sub(_mocks.Binder.Object);

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.Binder = new();

        }

        [TestMethod]
        public void Ctor_AddsToBinding()
        {
            var vm = Create();
            _mocks.Binder.Verify(m => m.AddVM(vm));
        }

        [TestMethod]
        public void EnableBinding()
        {
            var vm = Create();
            vm.ReenableBindingAndSendToModel("ghi", "abc");
            _mocks.Binder.Verify(m => m.EnableVMAndSendToModel(vm, new string[] { "ghi", "abc" }));
        }

        [TestMethod]
        public void DisableBinding()
        {
            var vm = Create();
            vm.DisableBinding();
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
