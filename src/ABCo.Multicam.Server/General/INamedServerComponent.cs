using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.General
{
	public interface INamedServerComponent
	{
		string Name { get; }
		void Rename(string name);
	}
}
