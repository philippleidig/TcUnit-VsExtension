using TCatSysManagerLib;

namespace TwinpackVsixShared.Extensions
{
    public static class TcTreeItemExtensions
    {
		public static bool IsPlcProjectFolder (this ITcSmTreeItem treeItem)
		{
			return treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCFOLDER;
		}

		public static bool IsPlcProject(this ITcSmTreeItem treeItem)
		{
			return treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCAPP;
		}

		public static bool IsPlcFunctionBlock(this ITcSmTreeItem treeItem)
		{
			return treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCPOUFB;
		}

		public static bool IsPlcTask(this ITcSmTreeItem treeItem)
		{
			return treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCTASK;
		}

	}
}
