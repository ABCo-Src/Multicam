using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Bindings
{
    public interface IVMBinder<T> where T : IBindableVM
    {
        TNew GetVM<TNew>(object parentVM) where TNew : class, T;
        void AddVM(T targetVM); 
        void RemoveVM(T targetVM); 
        void DisableVM(T targetVM);
        void EnableVMAndSendToModel(T targetVM, string[] propsToSend);
    }

    /// <summary>
    /// Base class for the binders that link the view-models to the underlying models.
    /// </summary>
    public abstract class VMBinder<T> : IVMBinder<T> where T : IBindableVM
    {
        IServiceSource _source;
        T[] _registeredVMs = Array.Empty<T>();

        public VMBinder(IServiceSource source) => _source = source;

        public TNew GetVM<TNew>(object parentVM) where TNew : class, T
        {
            for (int i = 0; i < _registeredVMs.Length; i++)
                if (_registeredVMs[i].Parent == parentVM && _registeredVMs[i].GetType().IsAssignableTo(typeof(TNew)))
                    return (TNew)(object)_registeredVMs[i];

            var newVM = _source.Get<TNew>();
            AddVM((T)(object)newVM);
            newVM.Parent = parentVM;
            return newVM;
        }

        public void AddVM(T targetVM)
        {
            Array.Resize(ref _registeredVMs, _registeredVMs.Length + 1);
            _registeredVMs[^1] = targetVM;

            targetVM.BindingInfoStore = "";
            targetVM.PropertyChanged += VM_PropertyChanged;

            RefreshVMToModel(targetVM);
        }

        public void RemoveVM(T targetVM)
        {
            targetVM.PropertyChanged -= VM_PropertyChanged;

            // Remove the vm from the array
            int val = Array.IndexOf(_registeredVMs, targetVM);
            if (val == -1) return;

            T[] newArr = new T[_registeredVMs.Length - 1];
            Array.Copy(_registeredVMs, newArr, val);
            Array.Copy(_registeredVMs, val + 1, newArr, val, newArr.Length - val);
            _registeredVMs = newArr;
        }

        public void DisableVM(T targetVM)
        {
            targetVM.BindingInfoStore = null;
        }

        public void EnableVMAndSendToModel(T targetVM, string[] propsToSend)
        {
            targetVM.BindingInfoStore = "";

            for (int i = 0; i < propsToSend.Length; i++)
                OnVMChange(targetVM, propsToSend[i]);

            RefreshVMToModel(targetVM);
        }

        private void VM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var vm = (T)sender!;
            var store = (string?)vm.BindingInfoStore;

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

            OnVMChange((T)sender!, e.PropertyName);
        }

        public void SetVMProp(Action<T> setVMValue, string propName)
        {
            if (propName == "") throw new Exception("Property name must be given.");
            
            for (int i = 0; i < _registeredVMs.Length; i++)
            {
                // If binding is disabled on this VM, don't try.
                if (_registeredVMs[i].BindingInfoStore == null) continue;

                _registeredVMs[i].BindingInfoStore = propName;
                setVMValue(_registeredVMs[i]);
            }
        }

        public abstract void RefreshVMToModel(T vm);
        public abstract void OnVMChange(T vm, string? prop);
    }

    public interface IBindableVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Space used by the binding system to store information about the VM's config.
        /// </summary>
        // null if binding is disabled
        // "" if binding is enabled and no property to suppress
        // "propdata" if binding is enabled and property is present
        object? BindingInfoStore { get; set; }

        /// <summary>
        /// The parent of this view-model.
        /// </summary>
        object Parent { get; set; }
    }
}
