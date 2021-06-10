using ImGuiNET;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties {

    public class BoolProperty : Property {

        public BoolProperty(string name, bool defaultValue, string description) : base(name, defaultValue, description) { }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            bool outBool = mainEntity.GetBool(Name, (bool)DefaultValue);
            if (ImGui.Checkbox(MiscHelper.CleanCamelCase(Name), ref outBool)) {
                changed = true;
                foreach (var entity in entities) {
                    entity.Attributes[Name] = outBool;
                }
            }
            UIHelper.Tooltip(Description);

            return changed;
        }
    }
}
