#if TOOLS
using Godot;
using System;

namespace LBG.GodotTools;

[Tool]
public partial class LBGToolsPlugin : EditorPlugin {
	public override void _EnterTree() {
		// Initialization of the plugin goes here.
	}

	public override void _ExitTree() {
		// Clean-up of the plugin goes here.
	}
}
#endif
