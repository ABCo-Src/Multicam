﻿using ABCo.Multicam.Server.Features.Switchers.Fading;
using ABCo.Multicam.Server.Features.Switchers.Data;

namespace ABCo.Multicam.Server.Features.Switchers
{
	/// <summary>
	/// The raw surface to talk to a switcher. 
	/// 
	/// None of these operations are likely to be cached (there's a buffer sitting atop to do that caching), and all interactions implemented here should only be
	/// implemented if the switcher can natively/intuitively perform them. 
	/// 
	/// For example, the preview API should throw if the specs say preview isn't supported. The layers above the switcher will emulate it.
	/// </summary>
	public interface ISwitcher : IDisposable
    {
		// General:
		void Connect();
        void Disconnect();
        void RefreshConnectionStatus();
        void RefreshSpecs();
        SwitcherCompatibility GetPlatformCompatibility();

        // Program/Preview:
        void RefreshProgram(int mixBlock);
        void RefreshPreview(int mixBlock);
        void SendProgramValue(int mixBlock, int id);
        void SendPreviewValue(int mixBlock, int id);
        void Cut(int mixBlock);

        // Cut Bus:
        CutBusMode GetCutBusMode(int mixBlock);
        void SetCutBusMode(int mixBlock, CutBusMode mode);
        void SetCutBus(int mixBlock, int newVal);

        // Event Handling:
        void SetEventHandler(ISwitcherEventHandler? eventHandler);
    }

    public enum CutBusMode
    {
        Cut,
        Auto
    }

    public record struct SwitcherProgramChangeInfo(int MixBlock, int NewValue, RetrospectiveFadeInfo? FadeInfo);
    public record struct SwitcherPreviewChangeInfo(int MixBlock, int NewValue, RetrospectiveFadeInfo? FadeInfo);
}