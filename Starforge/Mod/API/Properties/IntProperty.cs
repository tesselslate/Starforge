using ImGuiNET;
using Starforge.Map;
using Starforge.Util;
using System;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties {
    public class IntProperty : Property {
        public IntProperty(string name, int defaultValue, string description) : base(name, defaultValue, description) { }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            int outInt = mainEntity.GetInt(Name, (int)DefaultValue);
            if (ImGui.InputInt(MiscHelper.CleanCamelCase(Name), ref outInt)) {
                changed = true;
                foreach (var entity in entities) {
                    entity.Attributes[Name] = outInt;
                }
            }
            UIHelper.Tooltip(Description);

            return changed;
        }
    }
}
