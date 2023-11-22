using ABCo.Multicam.Server.Features.Switchers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	public interface IMainFeatureCollectionState : IServerComponentState<IMainFeatureCollectionState, IMainFeatureCollection> 
	{ 
		IReadOnlyList<IFeatureState> Features { get; internal set; }
	}

	public partial class MainFeatureCollectionState : ServerComponentState<IMainFeatureCollectionState, IMainFeatureCollection>, IMainFeatureCollectionState
	{
		[ObservableProperty] IReadOnlyList<IFeatureState> _features = Array.Empty<IFeatureState>();

		public MainFeatureCollectionState(IMainFeatureCollection component, IServerInfo info) : base(component, info) { }
	}
}
