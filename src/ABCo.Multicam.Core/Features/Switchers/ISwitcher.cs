using ABCo.Multicam.Core.Features.Switchers.Fading;

namespace ABCo.Multicam.Core.Features.Switchers
{
    /// <summary>
    /// The raw surface to talk to the switcher. 
    /// None of these operations are likely to be cached (the runner provides caching), and all interactions must be valid 
    /// (no changing the preview bus if there is none, for instance, it's the runner's job to emulate this if needed).
    /// </summary>
    public interface ISwitcher : IDisposable
    {
        bool IsConnected { get; }
        SwitcherConfig ConnectionConfig { get; }

        /// <summary>
        /// Establishes a connection with the physical switcher
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the physical switcher
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Contacts the switcher and receives its current specifications.
        /// </summary>
        SwitcherSpecs RefreshSpecs();

        /// <summary>
        /// Receives the current value stored in the given mix block.
        /// </summary>
        int RefreshProgram(int mixBlock);

        /// <summary>
        /// Receives the current value stored in the given mix block.
        /// </summary>
        void RefreshPreview(int mixBlock);

        /// <summary>
        /// Contacts the switcher and sends a new value to the program bus of the given mix-block.
        /// </summary>
        void SendProgramValue(int mixBlock, int id);

        /// <summary>
        /// Contacts the switcher and sends a new value to the preview bus of the given mix-block.
        /// </summary>
        void SendPreviewValue(int mixBlock, int id);

        /// <summary>
        /// Contacts the swither and asks it to perform an "immediate cut".
        /// </summary>
        void Cut(int mixBlock);

        /// <summary>
        /// Contacts the switcher and asks it perform a cut bus switch.
        /// </summary>
        void SetCutBus(int mixBlock, int newVal);

        CutBusMode GetCutBusMode(int mixBlock);

        /// <summary>
        /// Contacts the switcher and asks it perform a cut bus switch.
        /// </summary>
        void SetCutBusMode(int mixBlock, CutBusMode mode);

        void SetEventHandler(ISwitcherEventHandler? eventHandler);
    }

    public enum CutBusMode
    {
        Cut,
        Auto
    }

    public record struct SwitcherProgramChangeInfo(int MixBlock, byte Bus, int NewValue, RetrospectiveFadeInfo? FadeInfo);
    public record struct SwitcherPreviewChangeInfo(int MixBlock, int NewValue, RetrospectiveFadeInfo? FadeInfo);
}