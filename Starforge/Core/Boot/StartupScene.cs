using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core.Interop;
using Starforge.Mod.Content;
using System.Diagnostics;
using System.IO;

namespace Starforge.Core.Boot {
    public class StartupScene : Scene {
        private string[] AutolocatedInstalls;
        private int Current = 0;
        private string TryPath = "";
        private string PopupMsg = "";
        private bool Popup = false;
        private bool ShowInstallWindow = true;

        private float LogoRotation = 0f;
        private float TaskInterval = 0.1f;
        private float Alpha = 1f;

        public override void Begin() {
            AutolocatedInstalls = Engine.Platform.GetCelesteDirectories().ToArray();

            if (Directory.Exists(Settings.CelesteDirectory) && StartupHelper.IsCelesteInstall(Settings.CelesteDirectory)) {
                ShowInstallWindow = false;
                StartupHelper.BeginStartupTasks();
            }
        }

        public override bool End() => true;

        public override void Render(GameTime gt) {
            Engine.Instance.GraphicsDevice.Clear(Settings.BackgroundColor);

            if (ShowInstallWindow) {
                // Show the window for selecting a Celeste installation.

                ImGui.SetNextWindowSize(new System.Numerics.Vector2(500f, 280f));
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2 - 250, Engine.Instance.GraphicsDevice.Viewport.Height / 2 - 140));
                ImGui.Begin("Startup",
                    ImGuiWindowFlags.NoScrollbar |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoTitleBar
                );

                ImGui.TextWrapped("Please select the Celeste installation you would like Starforge to use. Click the Choose button to manually locate it.");

                ImGui.SetCursorPos(new System.Numerics.Vector2(10f, 50f));
                ImGui.SetNextItemWidth(480f);
                if (ImGui.ListBox("", ref Current, AutolocatedInstalls, AutolocatedInstalls.Length, 8)) TryPath = AutolocatedInstalls[Current];

                ImGui.SetCursorPosX(10f);
                ImGui.Text("Selected Location");

                ImGui.SetCursorPosX(10f);
                ImGui.SetNextItemWidth(480f);
                ImGui.InputText("", ref TryPath, 4096, ImGuiInputTextFlags.ReadOnly);

                ImGui.SetCursorPos(new System.Numerics.Vector2(294f, 250f));
                if (ImGui.Button("Quit", new System.Numerics.Vector2(50f, 20f))) Engine.Instance.Exit();

                ImGui.SameLine();
                if (ImGui.Button("Choose", new System.Numerics.Vector2(60f, 20f))) {
                    if (NFD.PickFolder(Engine.RootDirectory, out string celestePath) == NfdResult.OKAY) {
                        TryPath = celestePath;
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button("Continue", new System.Numerics.Vector2(70f, 20f))) {
                    if (string.IsNullOrEmpty(TryPath)) {
                        PopupMsg = "No installation was selected.";
                        Popup = true;
                    } else {
                        if (StartupHelper.IsCelesteInstall(TryPath)) {
                            Settings.CelesteDirectory = TryPath;
                            ShowInstallWindow = false;

                            StartupHelper.BeginStartupTasks();
                        } else {
                            PopupMsg = "The selected folder is not a valid installation.";
                            Popup = true;
                        }
                    }
                }

                ImGui.End();

                if (Popup) {
                    ImGui.OpenPopup("Notification");

                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(200f, 100f));
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2 - 100, Engine.Instance.GraphicsDevice.Viewport.Height / 2 - 50));
                    if (ImGui.BeginPopupModal("Notification", ref Popup, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)) {
                        ImGui.TextWrapped(PopupMsg);
                    }
                    ImGui.EndPopup();
                }
            } else {
                // Show the launch window.
                Engine.Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
                Engine.Batch.Draw(GFX.Logo, new Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2 - 128, Engine.Instance.GraphicsDevice.Viewport.Height / 2), GFX.Logo.Bounds, Color.White * Alpha, LogoRotation, new Vector2(GFX.Logo.Bounds.Center.X, GFX.Logo.Bounds.Center.Y), 1f, SpriteEffects.None, 0f);
                Engine.Batch.End();

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, Alpha);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
                ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0f, 0f, 0f, 0f));
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2, Engine.Instance.GraphicsDevice.Viewport.Height / 2 - ImGui.GetTextLineHeightWithSpacing()));
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(256f, 64f));

                ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

                if (StartupHelper.HasErrored) {
                    ImGui.Text("Starforge encountered an error.");
                    if (ImGui.Button("Open log")) {
                        Logger.Close();
                        Process.Start(Path.Combine(Settings.ConfigDirectory, "log.txt"));
                    }
                } else {
                    ImGui.Text("Launching Starforge...");
                    ImGui.Text($"{StartupHelper.FinishedTasks} / {StartupHelper.TotalTasks} tasks complete");
                }

                ImGui.End();
                ImGui.PopStyleVar(2);
                ImGui.PopStyleColor();
            }
        }

        public override void Update(GameTime gt) {
            if (!StartupHelper.HasErrored) {
                LogoRotation = LogoRotation % (MathHelper.Pi * 2) + (float)gt.ElapsedGameTime.TotalSeconds * 2;
            } else {
                if (LogoRotation % (MathHelper.Pi * 2) > 0.001) {
                    float mod = LogoRotation % (MathHelper.Pi * 2);
                    LogoRotation = mod / 1.05f;
                } else {
                    LogoRotation = 0;
                }
            }

            TaskInterval -= (float)gt.ElapsedGameTime.TotalSeconds;

            // Continuously update the startup helper to check for finished tasks.
            if (TaskInterval < 0 && StartupHelper.CurrentlyStarting) {
                TaskInterval = 0.25f;
                StartupHelper.Update();
            }

            // Check if startup helper finished
            if (StartupHelper.Finished) {
                if (Alpha > 0.01) {
                    Alpha = Alpha /= 1.3f;
                } else {
                    Engine.SetScene(new BlankScene());
                }
            }
        }
    }
}
