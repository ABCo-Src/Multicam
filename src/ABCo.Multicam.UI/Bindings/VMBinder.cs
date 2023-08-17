using ABCo.Multicam.Core;
using System.ComponentModel;

namespace ABCo.Multicam.UI.Bindings
{
    public interface IVMBinder<T> where T : IVMForBinder<T>
    {
        TNew GetVM<TNew>(object parentVM) where TNew : class, T;
        void AddVM(T targetVM); 
        void RemoveVM(T targetVM); 
        void DisableVM(T targetVM);
        void EnableVMAndSendToModel(T targetVM, string[] propsToSend);
    }

    // TODO: Add dispose control over this...
    /// <summary>
    /// Base class for the binders that link the view-models to the underlying models.
    /// </summary>
    public abstract class VMBinder<TVM> : IVMBinder<TVM> where TVM : IVMForBinder<TVM>
    {
        protected IServiceSource _servSource;
        TVM[] _registeredVMs = Array.Empty<TVM>();

        public abstract class PropertyBinding
        {
            public record struct VMInfo(Action<TVM> UpdateModel, string PropertyName);
            public VMInfo? VMChange;

            public abstract void UpdateCache();
            public abstract void UpdateVM(TVM vm);
        }

        public class PropertyBinding<TItem> : PropertyBinding
        {
            public record struct ModelInfo(Func<TItem> UpdateCache, Action<(TVM VM, TItem NewVal)> UpdateVM);
            public ModelInfo ModelChange;

            TItem? _valueCache = default!;

            public override void UpdateCache() => _valueCache = ModelChange.UpdateCache();
            public override void UpdateVM(TVM vm) => ModelChange.UpdateVM((vm, _valueCache!));
        }

        public PropertyBinding[] Properties { get; private set; } = null!;

        public virtual PropertyBinding[] CreateProperties() => Array.Empty<PropertyBinding>();

        public VMBinder(IServiceSource source) => _servSource = source;

        public void Init()
        {
            Properties = CreateProperties();
            for (int i = 0; i < Properties.Length; i++)
                Properties[i].UpdateCache();
        }

        public TNew GetVM<TNew>(object parentVM) where TNew : class, TVM
        {
            for (int i = 0; i < _registeredVMs.Length; i++)
                if (_registeredVMs[i].Parent == parentVM && _registeredVMs[i].GetType().IsAssignableTo(typeof(TNew)))
                    return (TNew)(object)_registeredVMs[i];

            var newVM = _servSource.Get<TNew>();
            AddVM((TVM)(object)newVM);
            newVM.Parent = parentVM;
            newVM.Binder = this;
            return newVM;
        }

        public void AddVM(TVM targetVM)
        {
            Array.Resize(ref _registeredVMs, _registeredVMs.Length + 1);
            _registeredVMs[^1] = targetVM;

            targetVM.BindingInfoStore = "";
            targetVM.PropertyChanged += VM_PropertyChanged;

            for (int i = 0; i < Properties.Length; i++)
                Properties[i].UpdateVM(targetVM);
        }

        public void RemoveVM(TVM targetVM)
        {
            targetVM.PropertyChanged -= VM_PropertyChanged;

            // Remove the vm from the array
            int val = Array.IndexOf(_registeredVMs, targetVM);
            if (val == -1) return;

            TVM[] newArr = new TVM[_registeredVMs.Length - 1];
            Array.Copy(_registeredVMs, newArr, val);
            Array.Copy(_registeredVMs, val + 1, newArr, val, newArr.Length - val);
            _registeredVMs = newArr;
        }

        public void DisableVM(TVM targetVM)
        {
            targetVM.BindingInfoStore = null;
        }

        public void EnableVMAndSendToModel(TVM targetVM, string[] propsToSend)
        {
            targetVM.BindingInfoStore = "";

            // Update the model for properties we're sending
            for (int i = 0; i < Properties.Length; i++)
            {
                if (Properties[i].VMChange == null) continue;

                for (int j = 0; j < propsToSend.Length; j++)
                    if (Properties[i].VMChange!.Value.PropertyName == propsToSend[j])
                        Properties[i].VMChange!.Value.UpdateModel(targetVM);
            }

            // Update the VM
            for (int i = 0; i < Properties.Length; i++)
                Properties[i].UpdateVM(targetVM);
        }

        private void VM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var vm = (TVM)sender!;
            var store = vm.BindingInfoStore;

            // Stop if syncing is disabled.
            if (store == null) return;

            // Stop if we're suppresing the event
            if (store != "")
            {
                if (store == e.PropertyName)
                {
                    vm.BindingInfoStore = "";
                    return;
                }
            }

            // Update the model
            for (int i = 0; i < Properties.Length; i++)
            {
                if (Properties[i].VMChange == null) continue;

                if (Properties[i].VMChange!.Value.PropertyName == e.PropertyName)
                {
                    Properties[i].VMChange!.Value.UpdateModel(vm);
                    break;
                }
            } 
        }

        public void ReportModelChange(PropertyBinding prop)
        {
            // Update the cache
            prop.UpdateCache();

            // Update the VMs
            for (int i = 0; i < _registeredVMs.Length; i++)
            {
                // If the VM is disabled, don't do anything
                if (_registeredVMs[i].BindingInfoStore == null) continue;

                // If there's a VM change associated with this property, suppress the property change that'll come from this.
                if (prop.VMChange != null)
                    _registeredVMs[i].BindingInfoStore = prop.VMChange.Value.PropertyName;

                // Update the VM 
                prop.UpdateVM(_registeredVMs[i]);
            } 
        }
    }

    public interface IVMForBinder<TVM> : INotifyPropertyChanged where TVM : IVMForBinder<TVM>
    {
        /// <summary>
        /// Space used by the binding system to store information about the VM's config.
        /// </summary>
        // null if binding is disabled
        // "" if binding is enabled and no property to suppress
        // "propdata" if binding is enabled and property is present
        string? BindingInfoStore { get; set; }

        /// <summary>
        /// The parent of this view-model.
        /// </summary>
        object Parent { get; set; }

        IVMBinder<TVM>? Binder { get; set; }
    }
}
