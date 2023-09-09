using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.General;

namespace ABCo.Multicam.Core.Features
{
	public interface IGeneralFeaturePresenter
    {
		void OnFragmentUpdate<T>(int code, T structure);
    }

    public interface ISpecificFeaturePresenter : INeedsInitialization<IFeature>
    {
		void OnFragmentUpdate<T>(int code, T structure);
	}

    /// <summary>
    /// Represents a feature currently loaded, either on this system or another system.
    /// Introduces properties shared across all features, such as titles or machine switching.
    /// </summary>
    public interface IFeature : IParameteredService<FeatureTypes>, IDisposable
    {
        IGeneralFeaturePresenter GeneralUIPresenter { get; }
        ILiveFeature LiveFeature { get; }

        void PerformAction(int code, object param);
    }

	public class Feature : IFeature
    {
        readonly IServiceSource _servSource;
        readonly IFeatureManager _manager;

        FeatureDataStore _dataStore = null!;

        public IGeneralFeaturePresenter GeneralUIPresenter { get; private set; } = null!;
		public ISpecificFeaturePresenter SpecificUIPresenter { get; private set; } = null!;
		public ILiveFeature LiveFeature { get; private set; } = null!;

        public static IFeature New(FeatureTypes featureType, IServiceSource servSource) => new Feature(featureType, servSource);
		public Feature(FeatureTypes featureType, IServiceSource servSource)
        {
            _servSource = servSource;
            _manager = servSource.Get<IFeatureManager>();

			// Get the data specification for the given type
			var specs = featureType switch
			{
				FeatureTypes.Switcher => new SwitcherFeatureDataSpecification(),
				_ => throw new Exception("Unsupported feature type")
			};

			_dataStore = new(specs);

			LiveFeature = featureType switch
			{
				FeatureTypes.Switcher => _servSource.Get<ISwitcherRunningFeature>(),
				_ => _servSource.Get<IUnsupportedRunningFeature>(),
			};

			GeneralUIPresenter = _servSource.Get<IGeneralFeaturePresenter, IFeature>(this);
			SpecificUIPresenter = _servSource.Get<ISpecificFeaturePresenter, IFeature>(this);
		}

        public void Dispose() => LiveFeature.Dispose();

		public void PerformAction(int code, object param)
		{
			var act = _dataStore.GetApplyToLiveParamed(code);

			// If we have a live feature, apply it to that
			if (LiveFeature != null)
				act(LiveFeature, param);
		}
	}

	public enum FeatureFragmentID
	{
		Title
	}

	public enum FeatureActionID
    {
		SetTitle,
		MoveUp,
		MoveDown,
		Delete
	}

    public abstract class FeatureDataSpecification
    {
        public abstract FeatureDataValue[] Fragments { get; }
        public abstract FeatureAction[] ParameterlessActions { get; }
        public abstract FeatureActionParam[] ParamedActions { get; }
    }

    public struct FeatureDataValue
    {
        public int Id;
        public Type Type;

        public FeatureDataValue(int id, Type type) => (Id, Type) = (id, type);
    }

	public record struct FeatureAction(int Id, Action<object> ApplyToLive);
	public record struct FeatureActionParam(int Id, Action<object, object> ApplyToLive);
}
