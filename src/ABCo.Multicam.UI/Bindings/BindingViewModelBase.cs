using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.Bindings
{
    public abstract class BindingViewModelBase<TVMType> : ViewModelBase, IVMForBinder<TVMType>, IDisposable 
        where TVMType : IVMForBinder<TVMType>
    {
        public void ReenableModelBindingAndSend(params string[] toSend)
        {
            if (Binder == null) throw new Exception("Changing binding config during construction is not currently supported.");
            Binder.EnableVMAndSendToModel((TVMType)(object)this, toSend);
        }

        public void DisableModelBinding()
        {
            if (Binder == null) throw new Exception("Changing binding config during construction is not currently supported.");
            Binder.DisableVM((TVMType)(object)this);
        }

        public void Dispose() => Binder?.RemoveVM((TVMType)(object)this);

        public void InitBinding(IVMBinder<TVMType> binder) => Binder = binder;

        // Stored data for the binder
        public string? BindingInfoStore { get; set; }
        public object Parent { get; set; } = null!;
        public IVMBinder<TVMType>? Binder { get; set; }
    }
}
