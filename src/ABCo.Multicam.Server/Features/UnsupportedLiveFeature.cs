using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	public interface IUnsupportedLiveFeature : IFeature { }
    public class UnsupportedLiveFeature : IUnsupportedLiveFeature
    {
		readonly UnsupportedLiveFeatureState _state;

		public IFeatureState State => _state;

		public UnsupportedLiveFeature(IServerInfo info) => _state = new UnsupportedLiveFeatureState(this, info);

		public void Rename(string name) => _state.Name = name;
		public void Dispose() { }
	}

	public interface IUnsupportedLiveFeatureState : IFeatureState { }
	public partial class UnsupportedLiveFeatureState : ServerComponentState<IFeatureState, IFeature>, IUnsupportedLiveFeatureState
	{
		[ObservableProperty] string _name = "New Unsupported Feature";

		public UnsupportedLiveFeatureState(IUnsupportedLiveFeature component, IServerInfo info) : base(component, info) { }

		public FeatureTypes Type => FeatureTypes.Switcher;
		public IFeature Feature => _component;
	}
}
