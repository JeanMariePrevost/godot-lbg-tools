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

Copy this repository into your Godot project’s `addons/` folder and enable the plugin:

1. Clone or download:

   ```bash
   cd /path/to/your/godot/project
   git clone https://github.com/YourUser/godot-lbg-tools.git addons/lbg_tools
   ```

2. In the Godot editor, open **Project → Project Settings → Plugins**,
   find **LBG Tools** and set **Enabled** to **On**.

_or_ copy the `src/` folder into your project and consume the source files directly.

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
