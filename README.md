# godot-lbg-tools

**Personal C# toolkit of utilities and helper classes for the Godot Engine**

⚠️ **Warning:** API and features are not yet settled and may change without notice.

---

## Features & Examples

> **Coming soon:**
>
> - Detailed usage examples for signals (`LBGSignal`, `LBGSignal<T1,…>`), 3D utilities, and more.

---

## Installation & Usage

1. Copy this repository or download the /src/ folder

2. Move the contents of the /src/ folder into your Godot project’s `addons/lbg_tools` folder  
   You can also create a symlink of the same name for easier development or reuse across projects.  
   Note: **the root of the plugin folder _must_ be "/lbg_tools/" for Godot to be able to load it as a plugin.**

3. At this point, you should have:  
   _res://addons/lbg_tools/LBGToolsPlugin.cs_  
   _res://addons/lbg_tools/plugin.cfg_  
   _res://addons/lbg_tools/..._

4. In the Godot editor, open **Project → Project Settings → Plugins**.  
   If everything went as expected, you should now see the **LBG Tools** plugin.  
   To use it, set **Enabled** to **On**.

---

## Roadmap

This toolkit is under active development.

New utilities and enhancements will be added as I identify and resolve friction points in my Godot/C# workflow.

---

## Testing

An xUnit test suite is included.

From the `addons/lbg_tools` folder, run:

```bash
dotnet test
```

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

Feel free to use, modify, and redistribute.
