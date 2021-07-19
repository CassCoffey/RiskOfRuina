using System;
using IL.RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace RiskOfRuinaMod.Modules
{
	internal static class Music
	{
		internal static int musicSources = 0;

		internal static void Initialize()
		{
			MusicController.LateUpdate += delegate (ILContext il)
			{
				ILCursor ilcursor = new ILCursor(il);
				ILCursor ilcursor2 = ilcursor;
				Func<Instruction, bool>[] array = new Func<Instruction, bool>[1];
				array[0] = delegate (Instruction i)
				{
					int num;
					return ILPatternMatchingExt.MatchStloc(i, out num);
				};
				ilcursor2.GotoNext(array);
				ilcursor.EmitDelegate<Func<bool, bool>>((bool b) => b || Music.musicSources != 0);
			};
		}
	}
}
