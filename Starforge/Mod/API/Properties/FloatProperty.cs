using ImGuiNET;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties {

    public class FloatProperty : Property {

        public FloatProperty(string name, float defaultValue, string description) : base(name, defaultValue, description) { }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            float outFloat = mainEntity.GetFloat(Name, (float)DefaultValue);
            if (ImGui.InputFloat(MiscHelper.CleanCamelCase(Name), ref outFloat)) {
                changed = true;
                foreach (var entity in entities) {
                    entity.Attributes[Name] = outFloat;
                }
            }
            UIHelper.Tooltip(Description);

            return changed;
        }
    }
}
