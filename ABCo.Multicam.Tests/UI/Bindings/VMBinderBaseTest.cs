using ABCo.Multicam.Core.Features;
using ABCo.Multicam.UI.Bindings;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.Bindings
{
    public abstract class VMBinderBaseTest<TType, TVMType, TModel> 
        where TType : VMBinder<TVMType>
        where TModel : class
        where TVMType : class, IBindableVM<TVMType>
    {
        public record struct Mocks(
            Mock<TVMType> VM,
            Mock<TModel> Model
        );

        TType _binder = null!;
        protected Mocks _mocks = new();

        public abstract VMTestProperty[] Props { get; }
        public abstract TType Create();
        public abstract void SetupModel(Mock<TModel> model);

        [TestInitialize]
        public void InitInfo()
        {
            _mocks.VM = new();
            _mocks.Model = new();
            SetupModel(_mocks.Model);
            _binder = Create();
        }

        [TestMethod]
        public void Initial()
        {
            _mocks.VM.SetupGet(m => m.BindingInfoStore).Returns("");
            _binder.AddVM(_mocks.VM.Object);
            for (int i = 0; i < Props.Length; i++)
                _mocks.VM.VerifySet(Props[i].VMVerify);
        }

        [TestMethod]
        public void ModelChange()
        {
            _binder.AddVM(_mocks.VM.Object);

            for (int i = 0; i < Props.Length; i++)
            {
                if (Props[i].ModelTrigger == null) continue; 

                // Reset the VM
                _mocks.VM.Reset();
                _mocks.VM.SetupGet(m => m.BindingInfoStore).Returns("");

                // Call the trigger
                Props[i].ModelTrigger?.Invoke(_binder);

                // Verify that only this property was set
                _mocks.VM.VerifySet(Props[i].VMVerify);

                for (int j = 0; j < Props.Length; j++)
                    if (i != j)
                        _mocks.VM.VerifySet(Props[j].VMVerify, Times.Never);
            }
        }

        [TestMethod]
        public void VMChange()
        {
            PropertyChangedEventHandler _propChangeCallback = null!;
            _mocks.VM.SetupAdd(m => m.PropertyChanged += (s, e) => { }).Callback<PropertyChangedEventHandler>(m => _propChangeCallback = m);

            _binder.AddVM(_mocks.VM.Object);

            for (int i = 0; i < Props.Length; i++)
            {
                if (Props[i].ModelVerify == null) continue;

                _propChangeCallback(_mocks.VM.Object, new PropertyChangedEventArgs(Props[i].Name));
                _mocks.Model.Verify(Props[i].ModelVerify);
            }
        }

        public record class VMTestProperty(string Name, Action<TType>? ModelTrigger, Expression<Action<TModel>>? ModelVerify, Action<TVMType> VMVerify);
    }
}