using ABCo.Multicam.Server.Features.Switchers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
	public interface IServerList<IItem>
	{
		IReadOnlyList<IItem> Items { get; }
		void MoveUp(IItem feature);
		void MoveDown(IItem feature);
		void Delete(IItem feature);
	}

	public class ReorderableList<TItem> : List<TItem> where TItem : IDisposable
	{
		public void MoveUp(TItem feature)
		{
			int indexOfFeature = IndexOf(feature);

			// Don't do anything if it's at the start
			if (indexOfFeature == 0) return;

			(this[indexOfFeature], this[indexOfFeature - 1]) = (this[indexOfFeature - 1], this[indexOfFeature]);
		}

		public void MoveDown(TItem feature)
		{
			int indexOfFeature = IndexOf(feature);

			// Don't do anything if it's at the end
			if (indexOfFeature == Count - 1) return;

			(this[indexOfFeature], this[indexOfFeature + 1]) = (this[indexOfFeature + 1], this[indexOfFeature]);
		}

		public void Delete(TItem feature)
		{
			Remove(feature);
			feature.Dispose();
		}

		public void Dispose()
		{
			for (int i = 0; i < Count; i++)
				this[i].Dispose();
		}
	}
}
