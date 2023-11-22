using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public interface ISwitcherFeatureState : IFeatureState 
	{
		SwitcherPlatformCompatibilityValue PlatformCompatibility { get; internal set; }
		bool IsConnected { get; internal set; }
		string? ErrorMessage { get; internal set; }
		SpecsSpecificInfo SpecsInfo { get; internal set; }
		SwitcherConfig Config { get; internal set; }
	}

	public partial class SwitcherFeatureState : ServerComponentState<IFeatureState, IFeature>, ISwitcherFeatureState
	{
		public FeatureTypes Type => FeatureTypes.Switcher;

		public IFeature Feature => _component;

		[ObservableProperty] string _name = "New Switcher";
		[ObservableProperty] SwitcherPlatformCompatibilityValue _platformCompatibility = SwitcherPlatformCompatibilityValue.Supported;
		[ObservableProperty] bool _isConnected = false;
		[ObservableProperty] string? _errorMessage = null;
		[ObservableProperty] SpecsSpecificInfo _specsInfo = new(new SwitcherSpecs(), Array.Empty<MixBlockState>());
		[ObservableProperty] SwitcherConfig _config = new DummySwitcherConfig(4);

		public SwitcherFeatureState(IFeature component, IServerInfo info) : base(component, info) { }
	}

	public record class SpecsSpecificInfo(SwitcherSpecs Specs, IReadOnlyList<MixBlockState> State);
	public record struct MixBlockState(int Prog, int Prev);

	public enum SwitcherPlatformCompatibilityValue
	{
		Supported,
		UnsupportedPlatform,
		NoSoftware
	}
}
