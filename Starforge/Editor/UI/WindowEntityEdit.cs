using ImGuiNET;
using Starforge.Editor.Actions;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Editor.UI {

    using Attributes = Dictionary<string, object>;

    public class WindowEntityEdit : Window {
        public string MapName = "";
        private bool Done = false;
        private Entity SelectedEntity;

        private Attributes InitialAttributes;

        public WindowEntityEdit(Entity Entity) {
            SelectedEntity = Entity;
            InitialAttributes = MiscHelper.CloneDictionary(Entity.Attributes);
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
            MapEditor.Instance.State.Apply(new EntityEditAction(
                MapEditor.Instance.State.SelectedRoom,
                SelectedEntity,
                InitialAttributes,
                MiscHelper.CloneDictionary(SelectedEntity.Attributes)
            ));
            if (!Done) return;
        }

        // Returns true if the property was changed
        public bool AddEntry(Entity entity, Property property) {
            switch (property.Type) {
            case PropertyType.String:
                if (property.List) {
                    return AddEntryStringList(entity, property);
                }
                else {
                    return AddEntryString(entity, property);
                }
            case PropertyType.Char:
                return AddEntryChar(entity, property);
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

        private bool AddEntryChar(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = 0;
            }
            int outInt = (int)entity.Attributes[property.Name];
            ImGui.InputInt(MiscHelper.CleanCamelCase(property.Name), ref outInt);
            UIHelper.Tooltip(property.Description);
            if (outInt > char.MaxValue) outInt = char.MaxValue;
            bool changed = (int)entity.Attributes[property.Name] != outInt;
            entity.Attributes[property.Name] = (char)outInt;

            return changed;
        }

        private bool AddEntryInt(Entity entity, Property property) {
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

        private bool AddEntryFloat(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = 0f;
            }
            float outFloat = (float)entity.Attributes[property.Name];
            ImGui.InputFloat(MiscHelper.CleanCamelCase(property.Name), ref outFloat);
            UIHelper.Tooltip(property.Description);
            bool changed = (float)entity.Attributes[property.Name] != outFloat;
            entity.Attributes[property.Name] = outFloat;

            return changed;
        }

        private bool AddEntryString(Entity entity, Property property) {
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

        private bool AddEntryStringList(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = "";
            }
            int Index = 0;
            for (int i = 0; i < property.Values.Length; ++i) {
                if (property.Values[i].ToLower() == (string)entity.Attributes[property.Name]) {
                    Index = i;
                    break;
                }
            }
            int IndexBefore = Index;
            ImGui.SetNextItemWidth(100f);
            ImGui.ListBox(MiscHelper.CleanCamelCase(property.Name), ref Index, property.DisplayValues, property.DisplayValues.Length);
            UIHelper.Tooltip(property.Description);
            bool changed = IndexBefore != Index;
            entity.Attributes[property.Name] = property.Values[Index];

            return changed;
        }

        private bool AddEntryBool(Entity entity, Property property) {
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
