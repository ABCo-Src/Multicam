using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings
{
    public abstract class BindingViewModelBase<T> : ObservableObject, IBindableVM<T>, IDisposable 
        where T : IBindableVM<T>
    {
        public void ReenableModelBindingAndSend(params string[] toSend)
        {
            if (Binder == null) throw new Exception("Changing binding config during construction is not currently supported.");
            Binder.EnableVMAndSendToModel((T)(object)this, toSend);
        }

        public void DisableModelBinding()
        {
            if (Binder == null) throw new Exception("Changing binding config during construction is not currently supported.");
            Binder.DisableVM((T)(object)this);
        }

        public void Dispose() => Binder?.RemoveVM((T)(object)this);

        public void InitBinding(IVMBinder<T> binder) => Binder = binder;

        // Stored data for the binder
        public object? BindingInfoStore { get; set; }
        public object Parent { get; set; } = null!;
        public IVMBinder<T>? Binder { get; set; }
    }
}
