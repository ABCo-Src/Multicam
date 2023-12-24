using ABCo.Multicam.Server.Features.Switchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
	public interface IServerList<IItem>
	{
		void MoveUp(IItem feature);
		void MoveDown(IItem feature);
		void Delete(IItem feature);
	}
}
