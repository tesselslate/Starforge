using ImGuiNET;
using Starforge.Editor.Actions;
using Starforge.Map;
using Starforge.Util;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Starforge.Editor.UI {
    public class WindowRoomConfig : Window {
        public bool Apply = false;
        public Room Room;
        public RoomMeta Meta;

        private string InvalidMessage = string.Empty;

        public WindowRoomConfig(Room room = null) {
            if (room != null) {
                Room = room;
                Meta = room.Meta;
            } else {
                Meta = new RoomMeta($"lvl_{MapEditor.Instance.State.LoadedLevel.Rooms.Count + 1}");
            }

            // Apply meta fixes
            Meta.Bounds.X /= 8;
            Meta.Bounds.Y /= 8;
            Meta.Bounds.Width /= 8;
            Meta.Bounds.Height /= 8;
        }

        public override void Render() {
            if (InvalidMessage != string.Empty) {
                ImGui.OpenPopup("Invalid Room");
                UIHelper.CenterWindow(200f, 100f);

                if (ImGui.BeginPopupModal("Invalid Room", ref Visible, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)) {
                    ImGui.TextWrapped(InvalidMessage);
                    ImGui.SetCursorPos(new Vector2(170f, 70f));
                    if (ImGui.Button("OK", new Vector2(25f, 20f))) InvalidMessage = "";
                }
            }

            ImGui.SetNextWindowSize(new Vector2(600f, 200f));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.Begin("Room Settings", ref Visible, ImGuiWindowFlags.NoResize);

            ImGui.Columns(2, "RoomSettingsColumns", false);
            ImGui.SetColumnWidth(0, 200f);
            ImGui.Text("General Settings");

            ImGui.SetNextItemWidth(150f);
            ImGui.InputText("Name", ref Meta.Name, 64);
            UIHelper.Tooltip("The name of the room.");

            ImGui.SetNextItemWidth(50f);
            ImGui.InputInt("X", ref Meta.Bounds.X, 0, 0);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(50f);
            ImGui.InputInt("Y", ref Meta.Bounds.Y, 0, 0);

            ImGui.SetNextItemWidth(50f);
            ImGui.InputInt("W", ref Meta.Bounds.Width, 0, 0);
            UIHelper.Tooltip("The width of the room, in tiles.");

            ImGui.SameLine();
            ImGui.SetNextItemWidth(50f);
            ImGui.InputInt("H", ref Meta.Bounds.Height, 0, 0);
            UIHelper.Tooltip("The height of the room, in tiles.");

            ImGui.SetNextItemWidth(50f);
            ImGui.PushStyleColor(ImGuiCol.Text, RoomColors[Meta.Color]);
            ImGui.SliderInt("Color", ref Meta.Color, 0, 6);
            ImGui.PopStyleColor();
            UIHelper.Tooltip("The color of the room on the debug map.");

            ImGui.SameLine();
            ImGui.Checkbox("Dark", ref Meta.Dark);
            UIHelper.Tooltip("Whether or not the room should be dark.");

            ImGui.Checkbox("Underwater", ref Meta.Underwater);
            UIHelper.Tooltip("Fills the room with water.");

            ImGui.SameLine();
            ImGui.Checkbox("Space", ref Meta.Space);
            UIHelper.Tooltip("Enables low gravity and screen wrap.");

            if (ImGui.BeginCombo("Wind", WindPatterns[Meta.WindPattern], ImGuiComboFlags.NoArrowButton)) {
                foreach (KeyValuePair<string, string> pair in WindPatterns) {
                    if (ImGui.Selectable(pair.Value, pair.Key == Meta.WindPattern)) Meta.WindPattern = pair.Key;
                }

                ImGui.EndCombo();
            }
            UIHelper.Tooltip("The direction(s) of the wind in the room.");

            ImGui.NextColumn();
            ImGui.Text("Music Settings");

            ImGui.SetNextItemWidth(200f);
            ImGui.InputText("Alt Music", ref Meta.AltMusic, 256);

            ImGui.SameLine();
            ImGui.SetCursorPosX(485f);
            ImGui.Checkbox("Delay", ref Meta.DelayAltMusicFade);

            ImGui.SetNextItemWidth(200f);
            ImGui.InputText("Ambience", ref Meta.Ambience, 256);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(25f);
            ImGui.SetCursorPosX(485f);
            ImGui.InputInt("Progress", ref Meta.AmbienceProgress, 0, 0);

            ImGui.SetNextItemWidth(200f);
            ImGui.InputText("Music", ref Meta.Music, 256);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(25f);
            ImGui.SetCursorPosX(485f);
            ImGui.InputInt("Progress", ref Meta.MusicProgress, 0, 0);

            ImGui.NewLine();
            ImGui.NewLine();
            ImGui.Text("Music Layers");

            ImGui.Checkbox("1", ref Meta.MusicLayer1);
            ImGui.SameLine();
            ImGui.Checkbox("2", ref Meta.MusicLayer2);
            ImGui.SameLine();
            ImGui.Checkbox("3", ref Meta.MusicLayer3);
            ImGui.SameLine();
            ImGui.Checkbox("4", ref Meta.MusicLayer4);
            ImGui.SameLine();
            ImGui.Checkbox("Whisper", ref Meta.Whisper);
            UIHelper.Tooltip("Enables the whispering noises from Mirror Temple.");

            ImGui.SameLine();
            ImGui.SetCursorPosX(540f);
            if (Apply = ImGui.Button("Apply")) {
                // Confirm metadata is valid
                if (Meta.Bounds.Width > 4096 || Meta.Bounds.Height > 4096) {
                    InvalidMessage = "The room size is too large (must be within 4096x4096.)";
                } else if (Room == null && MapEditor.Instance.RoomListWindow.RoomNames.Contains(Meta.Name)) {
                    InvalidMessage = "Another room already has the same name.";
                } else {
                    Visible = false;
                }
            }
            UIHelper.Tooltip("Your changes are not made until you click Apply.");

            ImGui.End();
            ImGui.PopStyleVar();
        }

        public override void End() {
            if (!Apply) return;

            Meta.Bounds.X *= 8;
            Meta.Bounds.Y *= 8;
            Meta.Bounds.Width *= 8;
            Meta.Bounds.Height *= 8;

            if (Room != null) {
                if (Room.Meta.Equals(Meta)) {
                    Room = null;
                    Meta = default;
                    return;
                }
                MapEditor.Instance.State.Apply(new RoomModificationAction(Room, Meta));
            } else {
                Room = new Room();
                Room.Meta = Meta;
                MapEditor.Instance.State.Apply(new RoomAdditionAction(Room));
            }
            Room = null;
            Meta = default;
        }

        private static Vector4[] RoomColors = new Vector4[]
        {
            new Vector4(0,          0,          0,          1),
            new Vector4(246 / 255f, 115 / 255f, 94  / 255f, 1),
            new Vector4(133 / 255f, 246 / 255f, 94  / 255f, 1),
            new Vector4(55  / 255f, 215 / 255f, 227 / 255f, 1),
            new Vector4(55  / 255f, 107 / 255f, 227 / 255f, 1),
            new Vector4(195 / 255f, 55  / 255f, 227 / 255f, 1),
            new Vector4(227 / 255f, 55  / 255f, 115 / 225f, 1)
        };

        private static Dictionary<string, string> WindPatterns = new Dictionary<string, string>()
        {
            ["None"] = "None",
            ["Left"] = "Left",
            ["LeftStrong"] = "Left (Strong)",
            ["LeftOnOff"] = "Left (On/Off)",
            ["LeftOnOffFast"] = "Left (On/Off, Fast)",
            ["Right"] = "Right",
            ["RightStrong"] = "Right (Strong)",
            ["RightOnOff"] = "Right (On/Off)",
            ["RightOnOffFast"] = "Right (On/Off, Fast)",
            ["RightCrazy"] = "Right (Crazy)",
            ["Alternating"] = "Alternating",
            ["Down"] = "Down",
            ["Up"] = "Up",
            ["Space"] = "Space"
        };
    }
}
