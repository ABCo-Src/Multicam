using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Represents a feature currently loaded.
	/// </summary>
	public interface IFeature : IServerService<FeatureTypes>, IServerComponent, IDisposable
	{
		IFeatureState State { get; }
		void Rename(string name);
	}

	public interface IFeatureState : IServerComponentState<IFeatureState, IFeature>
	{
		string Name { get; internal set; }
		FeatureTypes Type { get; }

		// ======
		// TODO: This is the worst thing ever
		// ======
		IFeature Feature { get; }
	}

	public enum FeatureTypes
	{
		Unsupported,

		// v1 features
		Switcher,
		Tally,
		Logger,
		Generator
	}
}