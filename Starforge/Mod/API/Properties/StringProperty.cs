using ImGuiNET;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties {
    public class StringProperty : Property {
        public StringProperty(string name, string defaultValue, string description) : base(name, defaultValue, description) { }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;
            string outString = mainEntity.GetString(Name, (string)DefaultValue);

            if (ImGui.InputText(MiscHelper.CleanCamelCase(Name), ref outString, 4098)) {
                changed = true;
                foreach (var entity in entities) {
                    entity.Attributes[Name] = outString;
                }
            }
            UIHelper.Tooltip(Description);

            return changed;
        }
    }
}
