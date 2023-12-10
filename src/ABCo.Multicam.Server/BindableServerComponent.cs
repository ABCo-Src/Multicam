using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Server
{
	public interface IBindableServerComponent<T> : IServerService<T>, INotifyPropertyChanged
	{
	}

	public abstract class BindableServerComponent<T> : ObservableObject, IBindableServerComponent<T>
	{
	}
}
