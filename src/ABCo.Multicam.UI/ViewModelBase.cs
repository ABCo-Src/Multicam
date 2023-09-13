using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.UI;

// GVG Protocol
public interface IAnimationHandlingVM : INotifyPropertyChanged
{
	void SetAnimationWaiter(Func<string, Task>? animationWaiter);
}

public abstract class ViewModelBase : ObservableObject, IAnimationHandlingVM
{
	Task? _currentHandler;
	Func<string, Task>? _animationWaiter;

	public ViewModelBase() { }
	public void SetAnimationWaiter(Func<string, Task>? animationWaiter) => _animationWaiter = animationWaiter;
	public async Task WaitForAnimationHandler(string propName)
	{
		while (_currentHandler != null) await _currentHandler;

		if (_animationWaiter != null)
		{
			_currentHandler = _animationWaiter(propName);
			await _currentHandler;
			_currentHandler = null;
		}
	}
}