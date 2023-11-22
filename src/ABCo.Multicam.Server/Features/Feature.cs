using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features
{
	/// <summary>
	/// Represents a feature currently loaded.
	/// </summary>
	public interface IFeature : IBindableServerComponent<IFeature>, IDisposable
	{
		string Name { get; }
		FeatureTypes Type { get; }
		void Rename(string name);
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