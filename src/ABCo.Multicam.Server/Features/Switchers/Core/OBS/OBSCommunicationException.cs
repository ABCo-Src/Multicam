﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS
{
	public class OBSCommunicationException : Exception
	{
		public OBSCommunicationException(string msg) : base(msg) { }
	}
}
