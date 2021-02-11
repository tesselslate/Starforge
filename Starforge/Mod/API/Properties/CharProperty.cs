using ImGuiNET;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties
{
    public class CharProperty : Property
    {
        public CharProperty(string name, char defaultValue, string description) : base(name, defaultValue, description) { }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            int outInt = mainEntity.GetInt(Name, (int)DefaultValue);
            if (ImGui.InputInt(MiscHelper.CleanCamelCase(Name), ref outInt)) {
                if (outInt > char.MaxValue) 
                    outInt = char.MaxValue;
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
