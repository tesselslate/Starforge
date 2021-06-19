using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Util;
using System;
using System.Collections.Generic;

namespace Starforge.Mod.API.Properties {
    public class NodeProperty : Property {

        public Range<uint> Range;
        static private Range<uint> DefaultRange = new Range<uint>(1, uint.MaxValue);
        new private Vector2 DefaultValue;

        static private readonly System.Numerics.Vector2 SmallButtonSize = new System.Numerics.Vector2(20f, 20f);

        public NodeProperty(Vector2 defaultValue, string description, Range<uint> range = null) : base("node", "", description) {
            if (range != null) {
                Range = range;
            }
            else {
                Range = DefaultRange;
            }
            DefaultValue = defaultValue;
        }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;

            Tuple<Vector2?, int> update = null;
            int counter = 0;
            bool canDelete = mainEntity.Nodes.Count > Range.Minimum;
            bool canAdd = mainEntity.Nodes.Count < Range.Maximum;

            foreach (var node in mainEntity.Nodes) {
                if (RenderNodeGUI(node, counter, ref update, canDelete)) {
                    changed = true;
                }

                counter++;
            }

            if (canAdd) {
                if (ImGui.Button("+", SmallButtonSize)) {
                    changed = true;
                    update = new Tuple<Vector2?, int>(DefaultValue, mainEntity.Nodes.Count);
                }
            }

            if (changed) {
                foreach (var entity in entities) {
                    if (entity.Nodes.Count < update.Item2) {
                        continue;
                    }

                    if (entity.Nodes.Count == update.Item2) {
                        // new node
                        entity.Nodes.Add((Vector2)update.Item1);
                    }
                    else if (update.Item1.HasValue) {
                        // updated node values
                        entity.Nodes[update.Item2] = (Vector2)update.Item1;
                    }
                    else {
                        // delete node
                        entity.Nodes.RemoveAt(update.Item2);
                    }
                }
            }

            return changed;
        }

        private bool RenderNodeGUI(Vector2 node, int count, ref Tuple<Vector2?, int> update, bool canDelete) {
            bool changed = false;

            string name = "Node " + count;
            ImGui.Text(name);
            ImGui.SameLine();
            ImGui.PushItemWidth(50f);
            ImGui.Text("X:");
            ImGui.SameLine();
            {
                float outX = node.X;
                if (ImGui.InputFloat("##X" + count, ref outX, 0f, 0f, "%.0f")) {
                    changed = true;
                    update = new Tuple<Vector2?, int>(new Vector2(outX, node.Y), count);
                }
                UIHelper.Tooltip(Description);
            }
            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            {
                float outY = node.Y;
                if (ImGui.InputFloat("##Y" + count, ref outY, 0f, 0f, "%.0f")) {
                    changed = true;
                    update = new Tuple<Vector2?, int>(new Vector2(node.X, outY), count);
                }
                UIHelper.Tooltip(Description);
            }
            ImGui.PopItemWidth();

            if (canDelete) {
                ImGui.SameLine();
                if (ImGui.Button("X##" + count, SmallButtonSize)) {
                    update = new Tuple<Vector2?, int>(null, count);
                    changed = true;
                }
                UIHelper.Tooltip("Delete this Node");
            }

            return changed;
        }
    }
}
