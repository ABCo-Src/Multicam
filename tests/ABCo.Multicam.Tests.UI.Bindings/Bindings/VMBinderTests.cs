using ABCo.Multicam.Core;
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
        public interface IVMType1 : IVMForDummyBinder { }
        public interface IVMType2 : IVMForDummyBinder { }

        record struct Mocks(
            Mock<IVMType1> VMType1,
            Mock<IVMType2> VMType2,
            Mock<DummyVM>[] VMs,
            PropertyChangedEventHandler[] VMEvents,
            Mock<IServiceSource> ServSource,
            Mock<IVMBinderOperationLogger> OpLogger,
            object Model
        );

        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.VMEvents = new PropertyChangedEventHandler[3] { (s, e) => { }, (s, e) => { }, (s, e) => { } };
            _mocks.VMType1 = new();
            _mocks.VMType1.SetupProperty(m => m.Parent);
            _mocks.VMType2 = new();
            _mocks.VMType2.SetupProperty(m => m.Parent);

            _mocks.ServSource = new();
            _mocks.ServSource.Setup(m => m.Get<IVMType1>()).Returns(_mocks.VMType1.Object);
            _mocks.ServSource.Setup(m => m.Get<IVMType2>()).Returns(_mocks.VMType2.Object);

            _mocks.OpLogger = new();
            _mocks.OpLogger.Setup(m => m.UpdateCache<string>(0)).Returns("abc");
            _mocks.OpLogger.Setup(m => m.UpdateCache<int>(1)).Returns(5);
            _mocks.OpLogger.Setup(m => m.UpdateCache<bool>(2)).Returns(true);

            _mocks.Model = new();
            _mocks.VMs = new Mock<DummyVM>[] { new(), new(), new() };

            _mocks.VMs[0].SetupAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[0] = h);

            _mocks.VMs[1].SetupAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[1] = h);
            _mocks.VMs[1].SetupRemove(m => m.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[1] = (s, e) => { });

            _mocks.VMs[2].SetupAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>()).Callback<PropertyChangedEventHandler>(h => _mocks.VMEvents[2] = h);
        }

        VMBinder<IVMForDummyBinder> Create()
        {
            var binder = new DummyBinder(_mocks.OpLogger.Object, _mocks.ServSource.Object);
            binder.Init();
            return binder;
        }

        [TestMethod]
        public void GetVM_NotRegistered()
        {
            var binder = Create();

            var parent = new object();
            var vm = binder.GetVM<IVMType1>(parent);

            Assert.AreEqual(_mocks.VMType1.Object, vm);
            _mocks.ServSource.Verify(m => m.Get<IVMType1>());
            _mocks.VMType1.VerifySet(m => m.Parent = parent);
            _mocks.VMType1.VerifySet(m => m.Binder = binder);
        }

        [TestMethod]
        public void GetVM_RegWithDiffType()
        {
            var binder = Create();

            object parent = new();
            var vm = binder.GetVM<IVMType1>(parent);
            var vm2 = binder.GetVM<IVMType2>(parent);

            Assert.AreEqual(_mocks.VMType2.Object, vm2);
            _mocks.ServSource.Verify(m => m.Get<IVMType1>(), Times.Once);
            _mocks.ServSource.Verify(m => m.Get<IVMType2>(), Times.Once);
        }

        [TestMethod]
        public void GetVM_RegWithDiffParent()
        {
            var binder = Create();

            var vm = binder.GetVM<IVMType1>(new());
            var vm2 = binder.GetVM<IVMType1>(new());

            Assert.AreEqual(_mocks.VMType1.Object, vm2);
            _mocks.ServSource.Verify(m => m.Get<IVMType1>(), Times.Exactly(2));
        }

        [TestMethod]
        public void GetVM_AlreadyReg()
        {
            var binder = Create();

            object parent = new();
            var vm = binder.GetVM<IVMType1>(parent);
            var vm2 = binder.GetVM<IVMType1>(parent);

            Assert.AreEqual(_mocks.VMType1.Object, vm2);
            _mocks.ServSource.Verify(m => m.Get<IVMType1>(), Times.Once);
        }

        [TestMethod]
        public void AddVM()
        {
            var binder = Create();

            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            // Verify it refreshed
            for (int i = 0; i < 2; i++)
            {
                _mocks.OpLogger.Verify(m => m.UpdateVM(0, _mocks.VMs[i].Object, "abc"), Times.Once);
                _mocks.OpLogger.Verify(m => m.UpdateVM(1, _mocks.VMs[i].Object, 5), Times.Once);
                _mocks.OpLogger.Verify(m => m.UpdateVM(2, _mocks.VMs[i].Object, true), Times.Once);
            }
            
            _mocks.VMs[0].VerifyAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
            _mocks.VMs[1].VerifyAdd(m => m.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
        }

        [TestMethod]
        [DataRow("Prop1", 0)]
        [DataRow("Prop2", 1)]
        [DataRow("Prop3", 2)]
        [DataRow("", -1)]
        [DataRow(null, -1)]
        public void VMChange(string? prop, int idx)
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            // First VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs(prop));
            VerifyUpdateModelCalls(0, idx);

            // Second VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[1](_mocks.VMs[1].Object, new PropertyChangedEventArgs(prop));
            VerifyUpdateModelCalls(1, idx);
        }

        [TestMethod]
        public void VMChange_Null()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            // First VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs(null));
            VerifyUpdateModelCalls(0, -1);

            // Second VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[1](_mocks.VMs[1].Object, new PropertyChangedEventArgs(null));
            VerifyUpdateModelCalls(1, -1);
        }

        [TestMethod]
        [DataRow("Prop1", 0)]
        [DataRow("Prop2", 1)]
        [DataRow("Prop3", 2)]
        [DataRow("", -1)]
        [DataRow(null, -1)]
        public void VMChange_Disabled(string prop, int idx)
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.DisableVM(_mocks.VMs[0].Object);

            // First VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[0](_mocks.VMs[0].Object, new PropertyChangedEventArgs(prop));

            for (int i = 0; i < 3; i++)
            {
                _mocks.OpLogger.Verify(m => m.UpdateModel(i, _mocks.VMs[0].Object), Times.Never);
                _mocks.OpLogger.Verify(m => m.UpdateModel(i, _mocks.VMs[1].Object), Times.Never);
            }

            // Second VM
            _mocks.OpLogger.Reset();
            _mocks.VMEvents[1](_mocks.VMs[1].Object, new PropertyChangedEventArgs(prop));
            VerifyUpdateModelCalls(1, idx);
        }

        void VerifyUpdateModelCalls(int vm, int idx)
        {
            for (int i = 0; i < 3; i++)
                _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[vm == 0 ? 1 : 0].Object), Times.Never);

            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[vm].Object), idx == 0 ? Times.Once : Times.Never);
            _mocks.OpLogger.Verify(m => m.UpdateModel(1, _mocks.VMs[vm].Object), Times.Never);
            _mocks.OpLogger.Verify(m => m.UpdateModel(2, _mocks.VMs[vm].Object), idx == 2 ? Times.Once : Times.Never);
        }

        [TestMethod]
        public void ModelChange_CorrectCalls()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);

            _mocks.OpLogger.Reset();
            _mocks.OpLogger.Setup(m => m.UpdateCache<string>(0)).Returns("def");
            binder.ReportModelChange(binder.Properties[0]);

            _mocks.OpLogger.Verify(m => m.UpdateCache<string>(0), Times.Once);
            _mocks.OpLogger.Verify(m => m.UpdateVM(0, _mocks.VMs[0].Object, "def"), Times.Once);
        }

        [TestMethod]
        public void ModelChange_CorrectOrder()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);

            _mocks.OpLogger.Reset();
            var seq = _mocks.OpLogger.SetupSequenceTracker(
                m => m.UpdateCache<string>(0),
                m => m.UpdateVM(0, _mocks.VMs[0].Object, It.IsAny<object>())
            );

            binder.ReportModelChange(binder.Properties[0]);
            seq.Verify();
        }

        [TestMethod]
        public void ModelChange_DisabledVM()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            _mocks.OpLogger.Reset();
            binder.DisableVM(_mocks.VMs[0].Object);
            binder.ReportModelChange(binder.Properties[0]);
            _mocks.OpLogger.Verify(m => m.UpdateVM(0, _mocks.VMs[0].Object, It.IsAny<object>()), Times.Never);
        }

        [TestMethod]
        public void ModelChange_SuppressChangedEvents()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.ReportModelChange(binder.Properties[0]);

            // Report the property change for VM #1
            TriggerEvent(0, "Prop1");
            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[0].Object), Times.Never);

            // Report the property change for VM #1 again
            TriggerEvent(0, "Prop1");
            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[0].Object), Times.Once);

            // Report a different property
            TriggerEvent(0, "Prop3");
            _mocks.OpLogger.Verify(m => m.UpdateModel(2, _mocks.VMs[0].Object), Times.Once);

            // Report the property change for VM #2
            TriggerEvent(1, "Prop1");
            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[1].Object), Times.Never);

            // Report a different property
            TriggerEvent(1, "Prop3");
            _mocks.OpLogger.Verify(m => m.UpdateModel(2, _mocks.VMs[1].Object), Times.Once);

            // Report the property change for VM #2 again
            TriggerEvent(1, "Prop1");
            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[1].Object), Times.Once);
        }

        [TestMethod]
        public void ModelChange_SuppressionDoesNotEnable()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.DisableVM(_mocks.VMs[1].Object);
            binder.ReportModelChange(binder.Properties[0]);
            Assert.AreEqual(null, _mocks.VMs[1].Object.BindingInfoStore);
        }

        void TriggerEvent(int vm, string val) => _mocks.VMEvents[vm](_mocks.VMs[vm].Object, new PropertyChangedEventArgs(val));

        [TestMethod]
        public void RemoveVM()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);
            binder.AddVM(_mocks.VMs[2].Object);
            binder.RemoveVM(_mocks.VMs[1].Object);

            // Verify property changed no longer applies
            _mocks.VMEvents[1](_mocks.VMs[1].Object, new PropertyChangedEventArgs("abc"));
            _mocks.OpLogger.Verify(m => m.UpdateModel(0, _mocks.VMs[1].Object), Times.Never);
        }

        [TestMethod]
        public void RemoveVM_Doubled()
        {
            var binder = Create();
            binder.AddVM(_mocks.VMs[0].Object);
            binder.RemoveVM(_mocks.VMs[1].Object);
            binder.RemoveVM(_mocks.VMs[1].Object);
        }

        [TestMethod]
        public void EnableAndSend()
        {
            var binder = Create();

            binder.AddVM(_mocks.VMs[0].Object);
            binder.AddVM(_mocks.VMs[1].Object);

            var seq = _mocks.OpLogger.SetupSequenceTracker(
                m => m.UpdateModel(0, _mocks.VMs[0].Object),
                m => m.UpdateModel(2, _mocks.VMs[0].Object),
                m => m.UpdateVM(0, _mocks.VMs[0].Object, "abc"),
                m => m.UpdateVM(1, _mocks.VMs[0].Object, 5),
                m => m.UpdateVM(2, _mocks.VMs[0].Object, true),
                m => m.UpdateVM(3, _mocks.VMs[0].Object, false)
            );

            binder.DisableVM(_mocks.VMs[0].Object);
            binder.EnableVMAndSendToModel(_mocks.VMs[0].Object, new string[] { "Prop1", "Prop3" });

            seq.Verify();
            _mocks.OpLogger.Verify(m => m.UpdateModel(3, _mocks.VMs[0].Object), Times.Never);
        }

        public interface IVMForDummyBinder : IVMForBinder<IVMForDummyBinder> { object Prop { get; set; } }
        public abstract class DummyVM : IVMForDummyBinder
        {
            public object? BindingInfoStore { get; set; }
            public object Parent { get; set; } = new();

            public abstract object Prop { get; set; }
            public abstract IVMBinder<IVMForDummyBinder>? Binder { get; set; }

            public abstract event PropertyChangedEventHandler? PropertyChanged;
        }

        public static Mock<IServiceSource> ServSourceMock = null!;

        public class DummyBinder : VMBinder<IVMForDummyBinder>
        {
            IVMBinderOperationLogger _logger;

            public override PropertyBinding[] CreateProperties() => new PropertyBinding[] 
            {
                // Prop1
                new PropertyBinding<string>()
                {
                    ModelChange = new(() => _logger.UpdateCache<string>(0), v => _logger.UpdateVM(0, v.VM, v.NewVal)),
                    VMChange = new(v => _logger.UpdateModel(0, v), "Prop1")
                },

                // Prop2
                new PropertyBinding<int>()
                {
                    ModelChange = new(() => _logger.UpdateCache<int>(1), v => _logger.UpdateVM(1, v.VM, v.NewVal))
                },

                // Prop3
                new PropertyBinding<bool>()
                {
                    ModelChange = new(() => _logger.UpdateCache<bool>(2), v => _logger.UpdateVM(2, v.VM, v.NewVal)),
                    VMChange = new(v => _logger.UpdateModel(2, v), "Prop3")
                },

                // Prop4
                new PropertyBinding<bool>()
                {
                    ModelChange = new(() => _logger.UpdateCache<bool>(3), v => _logger.UpdateVM(3, v.VM, v.NewVal)),
                    VMChange = new(v => _logger.UpdateModel(3, v), "Prop4")
                }
            };

            public DummyBinder(IVMBinderOperationLogger logger, IServiceSource servSource) : base(servSource) => _logger = logger;
        }

        public interface IVMBinderOperationLogger
        {
            T UpdateCache<T>(int propNo);
            void UpdateVM(int propNo, object vm, object newVal);
            void UpdateModel(int propNo, object vm);
        }
    }
}
