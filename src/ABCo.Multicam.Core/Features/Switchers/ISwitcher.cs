﻿using ABCo.Multicam.Core.Features.Switchers.Fading;

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
        SwitcherSpecs ReceiveSpecs();

        /// <summary>
        /// Contacts the switcher and receives the current value (with no cache) stored in the given mix block. Blocking.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        int ReceiveValue(int mixBlock, int bus);

        /// <summary>
        /// Contacts the switcher and sends a new value. Non-blocking.
        /// </summary>
        /// <param name="bus">The bus within the block. 0 is always program, and 1 may be preview IF the switcher supports it natively.</param>
        void PostValue(int mixBlock, int bus, int id);

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

        void SetOnBusChangeFinishCall(Action<SwitcherBusChangeInfo>? callback);
    }

    public enum CutBusMode
    {
        Cut,
        Auto
    }

    public record struct SwitcherBusChangeInfo(bool IsBusKnown, int MixBlock, int Bus, int NewValue, RetrospectiveFadeInfo? FadeInfo);
}