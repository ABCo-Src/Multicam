using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace ABCo.Multicam.Client.Blazor
{
	public abstract class VMBoundComponent<T> : ComponentBase, IDisposable, IHandleEvent where T : INotifyPropertyChanged
	{
		T _vm = default!;

		// TODO: Look into OnParametersSetAsync
		[Parameter]
#pragma warning disable
		public T VM
#pragma warning enable
		{
			get => _vm;
			set
			{
				if (_vm != null)
					_vm.PropertyChanged -= CallState;

				_vm = value;
				_vm.PropertyChanged += CallState;
				StateHasChanged();
			}
		}

		public void Dispose()
		{
			if (_vm != null)
				_vm.PropertyChanged -= CallState;
		}

		void CallState(object? sender, EventArgs e) => StateHasChanged();
		async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => await callback.InvokeAsync(arg);
	}

	public abstract class AnimBasedVMBoundComponent<T> : ComponentBase, IDisposable, IHandleEvent where T : IAnimationHandlingVM
	{
		T _vm = default!;

		// TODO: Look into OnParametersSetAsync
		[Parameter]
#pragma warning disable
		public T VM
#pragma warning enable
		{
			get => _vm;
			set
			{
				if (_vm != null)
					_vm.PropertyChanged -= CallState;

				_vm = value;
				_vm.PropertyChanged += CallState;
				_vm.SetAnimationWaiter(PerformAnimationAndDelay);
				StateHasChanged();
			}
		}

		public void Dispose()
		{
			if (_vm != null)
				_vm.PropertyChanged -= CallState;
		}

		public abstract Task PerformAnimationAndDelay(string propertyName);

		void CallState(object? sender, EventArgs e) => StateHasChanged();
		async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => await callback.InvokeAsync(arg);
	}
}