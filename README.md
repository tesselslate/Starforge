<div align="center">
	<img src="./assets/banner_256.png">
</div>

---

Starforge is an experimental map editor for Celeste. **It is still in very early development, and may never reach a usable state.** If you are interested in creating maps, you are better off taking a look at [Ahorn](https://github.com/CelestialCartographers/Ahorn) for now.

---
# Compiling from Source
First, clone the Git repository and its submodules (you can do the latter with `git submodule update --init --recursive`).

### Visual Studio:
- Compile FNA from source (make sure you are **creating a release build**) within the included submodule. The Starforge project is set up to automatically pull it from the `bin` folder.
- Download the FNA [native libraries](http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2) and unpack them. You will need to place **the x86 versions** of `FNA3D.dll` and `SDL2.dll` within the Starforge output directory (where the final `exe` is located.)

### If you are not using Visual Studio:
- Restore NuGet packages. Starforge requires `ImGui.NET`. Both `ImGui.NET.dll` and `cimgui.dll` should be present in the output directory containing the final executable.
- Compile the included nativefiledialog fork, and FNA submodule. Place the resulting DLLs into the output directory.
- Acquire the appropriate [native libraries](http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2) for FNA and place them in the output directory.

These steps may differ and libraries may not work depending on the platform you are using. If you have problems attempting to build the project, please open an issue so we can attempt to help and update these instructions to better reflect how to do so.

---
# Contributing
Public contributions are currently not accepted. You are, however, welcome to create a fork of your own. While it's preferred that in doing so you change the name and logo of your version of the project, I can't force you to do so.

---
# License
Starforge is licensed under the [MIT License.](https://github.com/WoofWoofDoggo/Starforge/blob/main/LICENSE) It also makes use of various open source projects, whose licenses can be found below.

- [FNA](https://github.com/FNA-XNA/FNA), which is under the [Microsoft Public License.](https://github.com/FNA-XNA/FNA/blob/master/licenses/LICENSE)
- [ImGui.NET](https://github.com/mellinoe/ImGui.NET), which is under the [MIT License.](https://github.com/mellinoe/ImGui.NET/blob/master/LICENSE)

#### Additionally, Starforge makes use of a [fork](https://github.com/WoofWoofDoggo/nativefiledialog) of [nativefiledialog](https://github.com/mlabbe/nativefiledialog), as well as a C# wrapper for it based off of [nfd-sharp.](https://github.com/benklett/nfd-sharp)
---
