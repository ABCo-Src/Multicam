using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace ABCo.Multicam.UI;

// GVG Protocol
public abstract class ViewModelBase : ObservableObject
{
    public ViewModelBase()
    {
        PropertyChanged += (s, e) =>
        {
            Debug.WriteLine("Property changed: " + e.PropertyName);
        };
    }
}