using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Server.Features
{
	public interface IUnsupportedLiveFeature : IFeature { }
    public partial class UnsupportedLiveFeature : BindableServerComponent<IFeature>, IUnsupportedLiveFeature
	{
		[ObservableProperty] string _name = "New Unsupported Feature";

		public FeatureTypes Type => FeatureTypes.Unsupported;

		public UnsupportedLiveFeature(IServerInfo info) : base(info) { }

		public void Rename(string name) => Name = name;
		public override void DisposeComponent() { }
	}
}
