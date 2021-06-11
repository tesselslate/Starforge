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
        private Entity MainEntity;
        private List<Entity> SelectedEntities;
        private EntitySelectionTool Tool;

        private List<Attributes> InitialAttributes;

        public WindowEntityEdit(EntitySelectionTool tool, Entity mainEntity, List<Entity> entities) {
            MainEntity = mainEntity;
            SelectedEntities = new List<Entity>(entities);
            InitialAttributes = new List<Attributes>(SelectedEntities.Count);
            for (int i = 0; i < SelectedEntities.Count; i++) {
                InitialAttributes.Add(MiscHelper.CloneDictionary(SelectedEntities[i].Attributes));
            }
            Tool = tool;
        }

        public override void Render() {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.OpenPopup("Editing Entity");
            ImGui.BeginPopupModal("Editing Entity", ref Visible, ImGuiWindowFlags.AlwaysAutoResize);

            bool changed = false;
            Type firstType = MainEntity.GetType();
            if (SelectedEntities.TrueForAll((e) => e.GetType() == firstType)) {
                PropertyList properties = MainEntity.Properties;
                foreach (Property property in properties) {
                    if (AddEntry(MainEntity, SelectedEntities, property)) {
                        changed = true;
                    }
                }
            }

            if (changed) {
                MapEditor.Instance.Renderer.GetRoom(SelectedEntities[0].Room).Dirty = true;
            }

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.PopStyleVar();

            if (ImGui.Button("Delete", new System.Numerics.Vector2(60f, 20f))) {
                MapEditor.Instance.State.Apply(new EntityRemovalAction(
                    MapEditor.Instance.State.SelectedRoom,
                    SelectedEntities
                ));
                Tool.Deselect();
                Visible = false;
            }
            UIHelper.Tooltip("Delete this Entity");

            ImGui.SameLine();
            if (ImGui.Button("OK", new System.Numerics.Vector2(25f, 20f))) {
                var newAttrs = new List<Attributes>(SelectedEntities.Count);
                for (int i = 0; i < SelectedEntities.Count; i++) {
                    newAttrs.Add(MiscHelper.CloneDictionary(SelectedEntities[i].Attributes));
                }
                MapEditor.Instance.State.Apply(new BulkEntityEditAction(
                    MapEditor.Instance.State.SelectedRoom,
                    SelectedEntities,
                    InitialAttributes,
                    newAttrs
                ));
                Visible = false;
            }

            ImGui.EndPopup();
        }

        public override void End() {
        }

        // Returns true if the property was changed
        public bool AddEntry(Entity mainEntity, List<Entity> entities, Property property) {
            return property.RenderGUI(mainEntity, entities);
        }
    }
}
