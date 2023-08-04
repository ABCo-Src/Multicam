using ABCo.Multicam.Tests.Helpers;
using ABCo.Multicam.UI.Bindings;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings
{
    [TestClass]
    public class VMBinderTests
    {
        public abstract class DummyVM : IBindableVM
        {
            public object? BindingInfoStore { get; set; }
            public abstract object Prop { get; set; }
            public abstract event PropertyChangedEventHandler? PropertyChanged;
        }

        record struct Mocks(
            Mock<DummyVM>[] VMs,
            PropertyChangedEventHandler[] VMEvents,
            Mock<VMBinder<DummyVM>> Object
        );

        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.VMEvents = new PropertyChangedEventHandler[3] { (s, e) => { }, (s, e) => { }, (s, e) => { } };
            _mocks.VMs = new Mock<DummyVM>[] { new(), new(), new() };
            _mocks.Object = new();

            _mocks.VMs[0].SetupAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[0] = h);
            _mocks.VMs[1].SetupAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[1] = h);
        }

        VMBinder<DummyVM> Create() => _mocks.Object.Object;

        [TestMethod]
        public void AddVM()
        {
            var binder = Create();

            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            _mocks.Object.Verify(m => m.RefreshVMToModel(_mocks.VMs[0].Object), Times.Once);
            _mocks.Object.Verify(m => m.RefreshVMToModel(_mocks.VMs[1].Object), Times.Once);
            _mocks.VMs[0].VerifyAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
            _mocks.VMs[1].VerifyAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
        }

        [TestMethod]
        public void PropertyChanged()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs("abc"));
            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs(""));
            _mocks.VMEvents[1](_mocks.VMs[1].Object, new PropertyChangedEventArgs(null));

            _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[0].Object, "abc"));
            _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[0].Object, ""));
            _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[1].Object, null));
        }

        [TestMethod]
        public void PropertyChanged_Disabled()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.DisableVM(_mocks.VMs[0].Object);

            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs("abc"));
            _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[0].Object, "abc"), Times.Never);
        }

        [TestMethod]
        public void PropertyChanged_SameAsSet()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.SetVMProp(vm => vm.Prop = "abc", nameof(DummyVM.Prop));

            for (int i = 0; i < 2; i++)
            {
                _mocks.VMEvents[i](_mocks.VMs[i].Object, new PropertyChangedEventArgs(nameof(DummyVM.Prop)));
                _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[i].Object, nameof(DummyVM.Prop)), Times.Never);

                _mocks.VMEvents[i](_mocks.VMs[i].Object, new PropertyChangedEventArgs(nameof(DummyVM.Prop)));
                _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[i].Object, nameof(DummyVM.Prop)), Times.Once);
            }
        }

        [TestMethod]
        public void PropertyChanged_DifferentThanSet()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.SetVMProp(vm => { }, "def");

            for (int i = 0; i < 2; i++)
            {
                _mocks.VMEvents[i](_mocks.VMs[i].Object, new PropertyChangedEventArgs(nameof(DummyVM.Prop)));
                _mocks.Object.Verify(m => m.OnVMChange(_mocks.VMs[i].Object, nameof(DummyVM.Prop)), Times.Once);
            }
        }

        [TestMethod]
        public void SetVMProp()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.AddVM(_mocks.VMs[2].Object);
            binder.DisableVM(_mocks.VMs[1].Object);

            int state = 0;

            binder.SetVMProp(vm =>
            {
                state++;

                if (state == 1) Assert.AreEqual(_mocks.VMs[0].Object, vm);
                else if (state == 2) Assert.AreEqual(_mocks.VMs[2].Object, vm);
            }, "def");

            Assert.AreEqual(2, state);
        }

        [TestMethod]
        public void EnableAndSend()
        {
            var binder = Create();

            var sequence = _mocks.Object.SetupSequenceTracker(
                m => m.OnVMChange(_mocks.VMs[0].Object, "abc"),
                m => m.OnVMChange(_mocks.VMs[0].Object, "def"),
                m => m.RefreshVMToModel(_mocks.VMs[0].Object)
            );

            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.DisableVM(_mocks.VMs[0].Object);
            binder.EnableVMAndSendToModel(_mocks.VMs[0].Object, new string[] { "abc", "def" });

            sequence.Verify();
        }
    }
}
