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
	Func<string, Task>? _animationWaiter;

	public ViewModelBase() { }
	public void SetAnimationWaiter(Func<string, Task>? animationWaiter) => _animationWaiter = animationWaiter;
	public async Task WaitForAnimationHandler(string propName)
	{
		if (_animationWaiter != null)
			await _animationWaiter(propName);
	}
}