using ImGuiNET;
using Starforge.Core;
using Starforge.Map;
using Starforge.Util;

namespace Starforge.Editor.UI {
    public class WindowNewMap : Window {
        public string MapName = "";
        private bool Clicked = false;

        public override void Render() {
            ImGui.OpenPopup("New Map");
            UIHelper.CenterWindow(200f, 100f);

            if (ImGui.BeginPopupModal("New Map", ref Visible, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)) {
                ImGui.Text("Enter a name for your map.");

                ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
                ImGui.SetNextItemWidth(185f);
                ImGui.InputText("", ref MapName, 4096);
                ImGui.PopStyleVar();

                ImGui.SetCursorPos(new System.Numerics.Vector2(170f, 70f));
                if (Clicked = ImGui.Button("OK", new System.Numerics.Vector2(25f, 20f))) {
                    Visible = false;
                    Clicked = true;
                }
            }

            ImGui.EndPopup();
        }

        public override void End() {
            if (!Clicked) return;
            MapEditor editor = new MapEditor();
            editor.LoadLevel(new Level(MapName));
            Engine.SetScene(editor);
        }
    }
}
