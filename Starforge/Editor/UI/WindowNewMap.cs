using ImGuiNET;
using Starforge.Core;
using Starforge.Map;
using Starforge.Util;

namespace Starforge.Editor.UI {
    public class WindowNewMap : Window {
        public string MapName = "";

        public override void Render() {
            ImGui.OpenPopup("New Map");
            UIHelper.CenterWindow(200f, 100f);

            if (ImGui.BeginPopupModal("New Map", ref Visible, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)) {
                ImGui.Text("Enter a name for your map.");

                ImGui.SetNextItemWidth(185f);
                ImGui.InputText("", ref MapName, 4096);

                ImGui.SetCursorPos(new System.Numerics.Vector2(170f, 70f));
                if (ImGui.Button("OK", new System.Numerics.Vector2(25f, 20f))) {
                    MapEditor editor = new MapEditor();
                    editor.LoadLevel(new Level(MapName));
                    Engine.SetScene(editor);
                    Visible = false;
                }
            }


            ImGui.EndPopup();
        }
    }
}
