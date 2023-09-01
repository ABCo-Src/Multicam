using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace ABCo.Multicam.UI.Blazor
{
	public abstract class VMBoundComponent<T> : ComponentBase, IHandleEvent where T : INotifyPropertyChanged
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

		void CallState(object? sender, EventArgs e) => StateHasChanged();
		async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => await callback.InvokeAsync(arg);
	}
}
