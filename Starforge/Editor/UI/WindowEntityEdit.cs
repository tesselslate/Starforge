using ImGuiNET;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Util;
using System.Numerics;

namespace Starforge.Editor.UI {
    public class WindowEntityEdit : Window {
        public string MapName = "";
        private bool Done = false;
        private Entity SelectedEntity;

        public WindowEntityEdit(Entity Entity) {
            SelectedEntity = Entity;
        }

        public override void Render() {
            PropertyList properties = SelectedEntity.Properties;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.OpenPopup("Editing Entity");
            ImGui.BeginPopupModal("Editing Entity", ref Visible, ImGuiWindowFlags.None);

            bool changed = false;
            foreach (Property property in properties) {
                if (AddEntry(SelectedEntity, property)) {
                    changed = true;
                }
            }

            if (changed) {
                MapEditor.Instance.Renderer.GetRoom(SelectedEntity.Room).Dirty = true;
            }

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.PopStyleVar();

            if (Done = ImGui.Button("OK", new System.Numerics.Vector2(25f, 20f))) {
                Visible = false;
                Done = true;
            }

            ImGui.EndPopup();
        }

        public override void End() {
            if (!Done) return;
        }

        // Returns true if the property was changed
        public bool AddEntry(Entity entity, Property property) {
            switch (property.Type) {
            case PropertyType.String:
                return AddEntryString(entity, property);
            case PropertyType.Integer:
                return AddEntryInt(entity, property);
            case PropertyType.Float:
                return AddEntryFloat(entity, property);
            case PropertyType.Bool:
                return AddEntryBool(entity, property);
            default:
                ImGui.Text("Unknown property type of property: " + property.Name);
                return false;
            }
        }

        public bool AddEntryInt(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = 0;
            }
            int outInt = (int)entity.Attributes[property.Name];
            ImGui.InputInt(MiscHelper.CleanCamelCase(property.Name), ref outInt);
            UIHelper.Tooltip(property.Description);
            bool changed = (int)entity.Attributes[property.Name] != outInt;
            entity.Attributes[property.Name] = outInt;

            return changed;
        }
        public bool AddEntryFloat(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = 0;
            }
            float outFloat = (float)entity.Attributes[property.Name];
            ImGui.InputFloat(MiscHelper.CleanCamelCase(property.Name), ref outFloat);
            UIHelper.Tooltip(property.Description);
            bool changed = (float)entity.Attributes[property.Name] != outFloat;
            entity.Attributes[property.Name] = outFloat;

            return changed;
        }

        public bool AddEntryString(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = "";
            }
            string outString = (string)entity.Attributes[property.Name];
            ImGui.SetNextItemWidth(100f);
            ImGui.InputText(MiscHelper.CleanCamelCase(property.Name), ref outString, 4096);
            UIHelper.Tooltip(property.Description);
            bool changed = (string)entity.Attributes[property.Name] != outString;
            entity.Attributes[property.Name] = outString;

            return changed;
        }

        public bool AddEntryBool(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = false;
            }
            bool outBool = (bool)entity.Attributes[property.Name];
            ImGui.Checkbox(MiscHelper.CleanCamelCase(property.Name), ref outBool);
            UIHelper.Tooltip(property.Description);
            bool changed = (bool)entity.Attributes[property.Name] != outBool;
            entity.Attributes[property.Name] = outBool;

            return changed;
        }

    }
}
