using Starforge.Map;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ImGuiNET;
using Starforge.Util;
using System.Collections;

namespace Starforge.Mod.API.Properties {

    public class ListProperty : Property {

        public ListProperty(string name, OrderedDictionary values, bool allowManualInput, object defaultValue, string description) : base(name, defaultValue, description) {
            AllowManualInput = allowManualInput;
            Values = values;
        }

        public ListProperty(string name, string[] values, bool allowManualInput, string defaultValue, string description) : base(name, defaultValue, description) {
            Values = new OrderedDictionary();
            AllowManualInput = allowManualInput;
            foreach (string v in values) {
                Values[MiscHelper.CleanCamelCase(v)] = v;
            }
        }

        public OrderedDictionary Values { get; private set; }
        public bool AllowManualInput { get; private set; }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            string outString = mainEntity.GetString(Name, DefaultValue.ToString());

            if (ImGui.BeginCombo(MiscHelper.CleanCamelCase(Name), outString)) {
                if (AllowManualInput && ImGui.InputText("", ref outString, 4096)) {
                    ImGui.SetItemDefaultFocus();
                    changed = true;
                    foreach (var entity in entities) {
                        entity.Attributes[Name] = outString;
                    }
                }

                foreach (DictionaryEntry pair in Values) {
                    if (ImGui.Selectable(pair.Key.ToString(), outString == pair.Value.ToString())) {
                        foreach (var entity in entities) {
                            entity.Attributes[Name] = pair.Value;
                        }
                    }
                }
                ImGui.EndCombo();
            }

            return changed;
        }
    }
}
