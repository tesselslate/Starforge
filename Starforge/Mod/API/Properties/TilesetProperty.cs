using ImGuiNET;
using Starforge.Editor;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Starforge.Mod.API.Properties {

    public class TilesetProperty : Property {

        public enum TilesetLayer {
            FG,
            BG
        }

        private static OrderedDictionary GetTilesets(TilesetLayer layer) {
            OrderedDictionary dict = new OrderedDictionary();
            var tilesets = layer == TilesetLayer.BG ? MapEditor.Instance.BGAutotiler.GetTilesetList() : MapEditor.Instance.FGAutotiler.GetTilesetList();
            foreach (var tileset in tilesets) {
                dict.Add(tileset.ID, MiscHelper.CleanCamelCase(tileset.Path));
            }
            return dict;
        }

        TilesetLayer Layer;

        public TilesetProperty(string name, TilesetLayer layer, string description) : base(name, GetTilesets(layer)[0], description) {
            Layer = layer;
        }

        public override bool RenderGUI(Entity mainEntity, List<Entity> entities) {
            bool changed = false;
            string outString = mainEntity.GetString(Name, DefaultValue.ToString());
            var tilesets = Layer == TilesetLayer.BG ? MapEditor.Instance.BGAutotiler.GetTilesetList() : MapEditor.Instance.FGAutotiler.GetTilesetList(); //GetTilesets(Layer);

            if (ImGui.BeginCombo(MiscHelper.CleanCamelCase(Name), MiscHelper.CleanCamelCase(tilesets.Find((t) => t.ID.ToString() == outString).Path))) {
                foreach (Tileset tileset in tilesets) {
                    if (ImGui.Selectable(MiscHelper.CleanCamelCase(tileset.Path), outString == tileset.ID.ToString())) {
                        foreach (var entity in entities)
                        {
                            entity.Attributes[Name] = tileset.ID.ToString();
                        }
                    }
                }
                ImGui.EndCombo();
            }

            return changed;
        }
    }
}
