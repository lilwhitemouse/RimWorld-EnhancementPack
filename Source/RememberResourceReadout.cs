﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace TD_Enhancement_Pack
{
	class RememberResourceReadout : GameComponent
	{
		public RememberResourceReadout(Game g) { }
		public override void ExposeData()
		{
			if(Settings.Get().rememberResourceReadout)
				foreach (var resource in DefDatabase<ThingCategoryDef>.AllDefs.Where(cat => cat.resourceReadoutRoot))
					SaveTree(resource.treeNode, 0, 32);
		}

		public void SaveTree(TreeNode_ThingCategory node, int nestLevel, int openMask)
		{
			if (node.Openable)
			{
				bool open = false;
				if (Scribe.mode == LoadSaveMode.Saving)
					open = node.IsOpen(openMask);
				
				Scribe_Values.Look(ref open, node.catDef.defName);

				if (Scribe.mode == LoadSaveMode.LoadingVars)
					node.SetOpen(openMask, open);
			}
			foreach (TreeNode_ThingCategory current in node.ChildCategoryNodes)
				if (!current.catDef.resourceReadoutRoot)
					SaveTree(current, nestLevel + 1, openMask);
		}

		//public override void LoadedGame()
		//{
		//	//TODO: If nothing open, call StartedNewGame because you assume it is unsaved.
		//}

		public override void StartedNewGame()
		{
			if (Settings.Get().startOpenResourceReadout)
				foreach (var resource in DefDatabase<ThingCategoryDef>.AllDefs.Where(cat => cat.resourceReadoutRoot))
					OpenAll(resource.treeNode, 0, 32);
		}

		public void OpenAll(TreeNode_ThingCategory node, int nestLevel, int openMask)
		{
			node.SetOpen(openMask, true);
			foreach (TreeNode_ThingCategory current in node.ChildCategoryNodes)
				if (!current.catDef.resourceReadoutRoot)
					OpenAll(current, nestLevel + 1, openMask);
		}
	}
}