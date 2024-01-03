using EnvDTE;
using System;
using System.Collections.Generic;
using System.Text;
using TCatSysManagerLib;

namespace TwinpackVsixShared.Extensions
{
	public static class ProjectItemExtensions
	{
		public static bool IsTwinCATTreeItem(this ProjectItem projectItem)
		{
			return projectItem?.Object is ITcSmTreeItem;
		}
	}
}
