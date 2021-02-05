using ImGuiNET;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Util;
using Starforge.Editor.UI;
using Starforge.Editor;
using Starforge.Vanilla.Actions;
using Starforge.Vanilla.Tools;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Starforge.Vanilla.UI {

    using Attributes = Dictionary<string, object>;

    public class WindowEntityEdit : Window {

        private Entity SelectedEntity;
        private EntitySelectionTool Tool;

        private Attributes InitialAttributes;

        public WindowEntityEdit(EntitySelectionTool tool, Entity Entity) {
            SelectedEntity = Entity;
            InitialAttributes = MiscHelper.CloneDictionary(Entity.Attributes);
            Tool = tool;
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

            if (ImGui.Button("Delete", new System.Numerics.Vector2(60f, 20f))) {
                MapEditor.Instance.State.Apply(new EntityRemovalAction(
                    MapEditor.Instance.State.SelectedRoom,
                    SelectedEntity
                ));
                Tool.Deselect();
                Visible = false;
            }
            UIHelper.Tooltip("Delete this Entity");

            ImGui.SameLine();
            if (ImGui.Button("OK", new System.Numerics.Vector2(25f, 20f))) {
                MapEditor.Instance.State.Apply(new EntityEditAction(
                    MapEditor.Instance.State.SelectedRoom,
                    SelectedEntity,
                    InitialAttributes,
                    MiscHelper.CloneDictionary(SelectedEntity.Attributes)
                ));
                Visible = false;
            }

            ImGui.EndPopup();
        }

        public override void End() {
        }

        // Returns true if the property was changed
        public bool AddEntry(Entity entity, Property property) {
            switch (property.Type) {
            case PropertyType.String:
                return AddEntryString(entity, property);
            case PropertyType.Char:
                return AddEntryChar(entity, property);
            case PropertyType.Integer:
                return AddEntryInt(entity, property);
            case PropertyType.Float:
                return AddEntryFloat(entity, property);
            case PropertyType.Bool:
                return AddEntryBool(entity, property);
            case PropertyType.List:
                return AddEntryList(entity, property);
            default:
                ImGui.Text("Unknown property type of property: " + property.Name);
                return false;
            }
        }

        private bool AddEntryChar(Entity entity, Property property) {
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = 0;
            }
            int outInt = Convert.ToInt32(entity.Attributes[property.Name]);
            ImGui.InputInt(MiscHelper.CleanCamelCase(property.Name), ref outInt);
            UIHelper.Tooltip(property.Description);
            if (outInt > char.MaxValue) outInt = char.MaxValue;
            bool changed = Convert.ToInt32(entity.Attributes[property.Name]) != outInt;
            entity.Attributes[property.Name] = Convert.ToChar(outInt);

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

        private bool AddEntryList(Entity entity, Property property) {
            if (property.Values.Count == 0) {
                return false;
            }
            if (!entity.Attributes.ContainsKey(property.Name)) {
                entity.Attributes[property.Name] = "";
            }

            string SelectedEntryBefore = property.SelectedEntry;
            if (property.SelectedEntry == null) {
                // find selected entry
                foreach (DictionaryEntry pair in property.Values) {
                    property.SelectedEntry ??= pair.Key.ToString();
                    if (property.Values[pair.Key].Equals(entity.Attributes[property.Name])) {
                        property.SelectedEntry = pair.Key.ToString();
                        break;
                    }
                }
            }

            ImGui.SetNextItemWidth(100f);
            if (ImGui.BeginCombo(MiscHelper.CleanCamelCase(property.Name), property.SelectedEntry)) {
                foreach (DictionaryEntry pair in property.Values) {
                    if (ImGui.Selectable(pair.Key.ToString())) {
                        property.SelectedEntry = pair.Key.ToString();
                    }
                    if (pair.Key.ToString() == property.SelectedEntry) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }

            UIHelper.Tooltip(property.Description);
            bool changed = property.SelectedEntry != SelectedEntryBefore;
            entity.Attributes[property.Name] = property.Values[property.SelectedEntry];

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
