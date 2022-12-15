﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCRCore.Command
{
	public interface ICommandPayload
	{
		ICommand Command { get; }
		string Parameters { get; }
		TCRClientUser UserExecutor { get; }
		bool Executed { get; }
		string Execute(object sender);
	}
}
