using ABCo.Multicam.UI.Bindings;
using Moq;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ABCo.Multicam.Tests.UI.Bindings
{
	public abstract class VMBinderBaseTest<TType, TVM, TModel> 
        where TType : VMBinder<TVM>
        where TModel : class
        where TVM : class, IVMForBinder<TVM>
    {
        public record struct BaseMocks(
            Mock<TVM> VM,
            Mock<TModel> Model
        );

        TType _binder = null!;
        protected BaseMocks _baseMocks = new();

        public abstract VMTestProperty[] Props { get; }
        public abstract TType Create();
        public abstract void SetupModel(Mock<TModel> model);
        public abstract void ChangeModelInSomeWay(Mock<TModel> model);
        public abstract void SetupVM(Mock<TVM> model);

        [TestInitialize]
        public void InitInfo()
        {
            _baseMocks.VM = new();
            SetupVM(_baseMocks.VM);
            _baseMocks.Model = new();
            SetupModel(_baseMocks.Model);
            _binder = Create();
        }

        [TestMethod]
        public void Initial()
        {
            _baseMocks.VM.SetupGet(m => m.BindingInfoStore).Returns("");
            _binder.AddVM(_baseMocks.VM.Object);
            for (int i = 0; i < Props.Length; i++)
                if (!Props[i].VMVerify(_baseMocks.VM.Object)) Assert.Fail("Failed VM verify!");
        }

        [TestMethod]
        public void ModelChange()
        {
            _binder.AddVM(_baseMocks.VM.Object);
            ChangeModelInSomeWay(_baseMocks.Model);

            for (int i = 0; i < Props.Length; i++)
            {
                if (Props[i].ModelTrigger == null) continue;

                // Reset the VM
                _baseMocks.VM.Reset();
                _baseMocks.VM.SetupGet(m => m.BindingInfoStore).Returns("");
                SetupVM(_baseMocks.VM);

                // Call the trigger
                Props[i].ModelTrigger?.Invoke(_binder);

                // Verify this property was set correctly
                if (!Props[i].VMVerify(_baseMocks.VM.Object)) Assert.Fail("Property not set correctly!");
            }
        }

        [TestMethod]
        public void VMChange()
        {
            PropertyChangedEventHandler _propChangeCallback = null!;
            _baseMocks.VM.SetupAdd(m => m.PropertyChanged += (s, e) => { }).Callback<PropertyChangedEventHandler>(m => _propChangeCallback = m);

            _binder.AddVM(_baseMocks.VM.Object);

            for (int i = 0; i < Props.Length; i++)
            {
                if (Props[i].ModelVerify == null) continue;

                _propChangeCallback(_baseMocks.VM.Object, new PropertyChangedEventArgs(Props[i].Name));
                _baseMocks.Model.Verify(Props[i].ModelVerify);
            }
        }

        public record class VMTestProperty(string Name, Action<TType>? ModelTrigger, Expression<Action<TModel>>? ModelVerify, Func<TVM, bool> VMVerify);
    }
}