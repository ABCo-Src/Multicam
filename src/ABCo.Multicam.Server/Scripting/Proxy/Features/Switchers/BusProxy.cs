using ABCo.Multicam.Server.Features.Switchers;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Scripting.Proxy.Features.Switchers
{
	[MoonSharpUserData]
	public class BusProxy
	{
		readonly ISwitcher _switcher;
		readonly int _mixBlock;
		readonly bool _isProgram;

		public BusProxy(ISwitcher switcher, int mixBlock, bool isProgram)
		{
			_switcher = switcher;
			_mixBlock = mixBlock;
			_isProgram = isProgram;
		}

		public int SelectedPos
		{
			get
			{
				// TODO: Improve exception type
				if (_mixBlock >= _switcher.SpecsInfo.State.Count) throw new Exception("Attempt to modify a mix-block that no longer exists.");

				int selectedId = _isProgram ? _switcher.SpecsInfo.State[_mixBlock].Prog : _switcher.SpecsInfo.State[_mixBlock].Prev;
				int selectedPos = GetPos(_isProgram ? _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].ProgramInputs : _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].PreviewInputs, selectedId);

				// Return that, translated
				return selectedPos + 1;
			}
			set
			{
				int translatedVal = value - 1;
				if (translatedVal < 0) throw new Exception("Selected position for a bus can't be less than 0!");
				if (_mixBlock >= _switcher.SpecsInfo.State.Count) throw new Exception("Attempt to modify a mix-block that no longer exists.");

				if (_isProgram)
				{
					if (translatedVal >= _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].ProgramInputs.Count) throw new Exception("Attempt to set a mix-block to an input that doesn't exist.");
					_switcher.SetProgram(_mixBlock, _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].ProgramInputs[translatedVal].Id);
				}
				else
				{
					if (translatedVal >= _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].ProgramInputs.Count) throw new Exception("Attempt to set a mix-block to an input that doesn't exist.");
					_switcher.SetPreview(_mixBlock, _switcher.SpecsInfo.Specs.MixBlocks[_mixBlock].PreviewInputs[translatedVal].Id);
				}
			}
		}

		int GetPos(IReadOnlyList<SwitcherBusInput> arr, int targetId)
		{
			for (int i = 0; i < arr.Count; i++)
				if (arr[i].Id == targetId)
					return i;

			return -1;
		}

		public override string ToString() => _isProgram ?
			$"Switchers[\"{_switcher.Name}\"].MixBlocks[{_mixBlock + 1}].Prog" :
			$"Switchers[\"{_switcher.Name}\"].MixBlocks[{_mixBlock + 1}].Prev";
	}
}