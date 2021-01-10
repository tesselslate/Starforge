<div align="center">
	<img src="./assets/banner_256.png">
</div>

---

Starforge is an experimental map editor for Celeste. **It is still in very early development, and may never reach a usable state.** If you are interested in creating maps, you are better off taking a look at [Ahorn](https://github.com/CelestialCartographers/Ahorn) for now.

---
# Compiling from Source
Prerequisites:
- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)

First, clone the Git repository and its submodules (you can do the latter with `git submodule update --init --recursive`).

### Windows Users (`dotnet` CLI / Visual Studio)
- Open `lib/FNA/FNA.Core.csproj`. Build for Release (x64). 
- Build Starforge. All dependencies should be copied automatically.

### Mac/Linux Users
Starforge does not *currently* support Mac and Linux.

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
