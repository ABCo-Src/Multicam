namespace ABCo.Multicam.Core.Features.Data
{
	public class FeatureGeneralInfo : FeatureData
	{
		public override int DataId => 0;

		public FeatureTypes Type { get; }
		public string Title { get; }

		public FeatureGeneralInfo(FeatureTypes type, string title)
		{
			Type = type;
			Title = title;
		}
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
