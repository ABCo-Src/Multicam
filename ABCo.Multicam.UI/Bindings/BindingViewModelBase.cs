using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings
{
    public abstract class BindingViewModelBase<T> : ObservableObject, IBindableVM, IDisposable 
        where T : IBindableVM
    {
        IVMBinder<T> _binder;

        public BindingViewModelBase(IVMBinder<T> binder)
        {
            _binder = binder;
            binder.AddVM((T)(object)this);
        }

        public void ReenableBindingAndSendToModel(params string[] toSend) => _binder.EnableVMAndSendToModel((T)(object)this, toSend);
        public void DisableBinding() => _binder.DisableVM((T)(object)this);
        public void Dispose() => _binder.RemoveVM((T)(object)this);

        public object? BindingInfoStore { get; set; }
    }
}
