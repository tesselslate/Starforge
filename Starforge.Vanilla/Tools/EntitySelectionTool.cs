using Starforge.Core;
using Starforge.Map;
using Starforge.Util;
using Starforge.Editor;
using Starforge.Vanilla.Actions;
using Starforge.Mod.Content;
using Starforge.Mod.API;
using Starforge.Vanilla.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;

namespace Starforge.Vanilla.Tools {

    using Attributes = Dictionary<string, object>;

    [ToolDefinition("Entity Selection")]
    public class EntitySelectionTool : Tool {
        private Rectangle Hold = new Rectangle(-1, -1, 0, 0);
        private bool Dragging = false;
        private Point Start;
        private List<Entity> SelectedEntities = new List<Entity>();
        private List<Point> ClickOffsets;
        private EntityRegion HeldRegion = EntityRegion.Outside;
        private List<Attributes> InitialAttributes;

        public override string GetName() => "Entity Selection";
        public override string GetSearchGroup() => "Selection";

        public override void Update() {
            // reset stuff if for some reason it didn't
            if (!Input.Mouse.HasAny()) {
                HeldRegion = EntityRegion.Outside;
                Hold = new Rectangle(-1, -1, 0, 0);
                Dragging = false;
                Start = Point.Zero;

                ClickOffsets = null;
                InitialAttributes = null;
            }
            UpdateCursor();

            if (Input.Mouse.RightClick) {
                HandleRightClick();
            }

            if (Input.Mouse.LeftClick) {
                HandleLeftClick();
            }

            if (Input.Mouse.LeftHold) {
                HandleLeftDrag();
            }

            if (Input.Mouse.LeftUnclick) {
                HandleLeftUnclick();
            }

            if (Input.Keyboard.Pressed(Keys.Delete)) {
                MapEditor.Instance.State.Apply(new EntityRemovalAction(
                    MapEditor.Instance.State.SelectedRoom,
                    SelectedEntities
                ));
                Deselect();
            }
        }

        public override void Render() {
            if (ImGuiNET.ImGui.IsMouseDoubleClicked(ImGuiNET.ImGuiMouseButton.Left)) {
                Entity clicked;
                if ((clicked = SelectedEntities.Find((e) => e.ContainsPosition(MapEditor.Instance.State.PixelPointer))) != null) {
                    Type clickedType = clicked.GetType();
                    Select(MapEditor.Instance.State.SelectedRoom.Entities.Where((e) => e.GetType() == clickedType).ToList());
                }
            }

            foreach (var entity in SelectedEntities) {
                GFX.Draw.BorderedRectangle(entity.Hitbox, Settings.ToolColor * 0.2f, Settings.ToolColor * 0.5f);
            }

            GFX.Draw.BorderedRectangle(new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8), Settings.ToolColor * 0.5f, Settings.ToolColor);
        }

        public void Select(List<Entity> entities) {
            SelectedEntities = entities;
            ClickOffsets = new List<Point>();
            InitialAttributes = new List<Attributes>(SelectedEntities.Count);
            // remember where in the entity we started clicking
            for (int i = 0; i < SelectedEntities.Count; i++) {
                ClickOffsets.Add(new Point(MiscHelper.GetMousePosition().X - (int)SelectedEntities[i].Position.X, MiscHelper.GetMousePosition().Y - (int)SelectedEntities[i].Position.Y));
                InitialAttributes.Add(MiscHelper.CloneDictionary(SelectedEntities[i].Attributes));
            }
        }

        public override void RenderGUI() {
            // No GUI to render
        }

        public override ToolLayer[] GetSelectableLayers() => null;

        public void Deselect() {
            SelectedEntities.Clear();
            HeldRegion = EntityRegion.Outside;
            ClickOffsets = new List<Point>();
        }

        private void HandleLeftClick() {
            Start = MapEditor.Instance.State.TilePointer;
            Entity clicked = MapEditor.Instance.State.SelectedRoom.Entities.Find(e => e.ContainsPosition(MapEditor.Instance.State.PixelPointer));

            if (!SelectedEntities.Contains(clicked)) {
                // if clicked outside of selected entities
                Deselect();
                return;
            }

            // else start dragging
            if (SelectedEntities.Count == 1) {
                HeldRegion = SelectedEntities[0].GetEntityRegion(MapEditor.Instance.State.PixelPointer);
            }
            else if (SelectedEntities.Count > 1) {
                HeldRegion = EntityRegion.Middle;
            }

            InitialAttributes = new List<Attributes>(SelectedEntities.Count);
            for (int i = 0; i < SelectedEntities.Count; i++) {
                InitialAttributes.Add(MiscHelper.CloneDictionary(SelectedEntities[i].Attributes));
            }

            // remember where in the entity we started clicking
            ClickOffsets = new List<Point>();
            for (int i = 0; i < SelectedEntities.Count; i++) {
                ClickOffsets.Add(new Point(MiscHelper.GetMousePosition().X - (int)SelectedEntities[i].Position.X, MiscHelper.GetMousePosition().Y - (int)SelectedEntities[i].Position.Y));
            }
        }

        private void HandleLeftDrag() {
            Point tp = MapEditor.Instance.State.TilePointer;

            if (Math.Abs(tp.X - Start.X) >= 4 || Math.Abs(tp.Y - Start.Y) >= 4) {
                Dragging = true;
            }

            if (HeldRegion == EntityRegion.Outside || SelectedEntities.Count == 0) {
                Point tl = new Point(
                    (int)MathHelper.Min(Start.X, tp.X),
                    (int)MathHelper.Min(Start.Y, tp.Y)
                );

                Point br = new Point(
                    (int)MathHelper.Max(Start.X, tp.X),
                    (int)MathHelper.Max(Start.Y, tp.Y)
                );

                Hold = new Rectangle(
                    tl.X, tl.Y,
                    br.X - tl.X + 1,
                    br.Y - tl.Y + 1
                );
                return;
            }

            if (SelectedEntities.Count == 1) {
                var SelectedEntity = SelectedEntities[0];
                Vector2 UpdatedPosition;
                UpdatedPosition.X = SelectedEntity.Position.X;
                UpdatedPosition.Y = SelectedEntity.Position.Y;

                int XEnd = (int)SelectedEntity.Position.X + SelectedEntity.Width;
                int YEnd = (int)SelectedEntity.Position.Y + SelectedEntity.Height;

                int MinSize;
                if (EditorState.PixelPerfect()) {
                    MinSize = 1;
                }
                else {
                    MinSize = 8;
                }

                if ((HeldRegion & EntityRegion.Left) != 0) {
                    // Update left side of the entity to mouse position
                    UpdatedPosition.X = MiscHelper.GetMousePosition().X;
                    if (UpdatedPosition.X >= XEnd) {
                        UpdatedPosition.X = XEnd - MinSize;
                    }
                    SelectedEntity.Width = XEnd - (int)UpdatedPosition.X;
                }
                if ((HeldRegion & EntityRegion.Right) != 0) {
                    // Update left side of the entity to mouse position
                    SelectedEntity.Width = MiscHelper.GetMousePositionCeil().X - (int)SelectedEntity.Position.X;
                }
                if ((HeldRegion & EntityRegion.Top) != 0) {
                    // Update top side of the entity to mouse position
                    UpdatedPosition.Y = MiscHelper.GetMousePosition().Y;
                    if (UpdatedPosition.Y >= YEnd) {
                        UpdatedPosition.Y = YEnd - MinSize;
                    }
                    SelectedEntity.Height = YEnd - (int)UpdatedPosition.Y;
                }
                if ((HeldRegion & EntityRegion.Bottom) != 0) {
                    // Update bottom side of the entity to mouse position
                    SelectedEntity.Height = MiscHelper.GetMousePositionCeil().Y - (int)SelectedEntity.Position.Y;
                }
                if (HeldRegion == EntityRegion.Middle) {
                    // Update location
                    UpdatedPosition.X = MiscHelper.GetMousePosition().X - ClickOffsets[0].X;
                    UpdatedPosition.Y = MiscHelper.GetMousePosition().Y - ClickOffsets[0].Y;
                }

                if (HeldRegion != EntityRegion.Middle) {
                    //don't resize to minimum size when only moving, no matter what
                    SelectedEntity.Width = (int)MathHelper.Max(SelectedEntity.Width, MinSize);
                    SelectedEntity.Height = (int)MathHelper.Max(SelectedEntity.Height, MinSize);
                }

                SelectedEntity.Position = UpdatedPosition;
            } else {
                for (int i = 0; i < SelectedEntities.Count; i++) {
                    Vector2 pos = new Vector2(MiscHelper.GetMousePosition().X - ClickOffsets[i].X, MiscHelper.GetMousePosition().Y - ClickOffsets[i].Y);
                    SelectedEntities[i].Position = pos;
                }
            }

            MapEditor.Instance.Renderer.GetRoom(SelectedEntities[0].Room).Dirty = true;
        }

        private void HandleLeftUnclick() {
            // can only select new entities when none are selected
            if (SelectedEntities.Count == 0) {
                if (Dragging) {
                    var collisionRect = new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8);
                    Select(MapEditor.Instance.State.SelectedRoom.Entities.Where(e => e.Hitbox.Intersects(collisionRect)).ToList());
                }
                else {
                    Entity clicked = MapEditor.Instance.State.SelectedRoom.Entities.Find(e => e.ContainsPosition(MapEditor.Instance.State.PixelPointer) && !SelectedEntities.Contains(e));
                    if (clicked != null) {
                        Select(new List<Entity> { clicked });
                    }
                }

                Hold = new Rectangle(MapEditor.Instance.State.TilePointer.X, MapEditor.Instance.State.TilePointer.Y, 0, 0);
            }
            else {
                // make action of movement thus far
                // Only create action if anything actually changed
                if (SelectedEntities.Count > 0 && InitialAttributes != null) {
                    List<Attributes> attrs = new List<Attributes>();
                    foreach (var item in SelectedEntities) {
                        attrs.Add(MiscHelper.CloneDictionary(item.Attributes));
                    }
                    MapEditor.Instance.State.Apply(new BulkEntityEditAction(
                        MapEditor.Instance.State.SelectedRoom,
                        SelectedEntities,
                        InitialAttributes,
                        attrs
                    ));
                }
            }

            HeldRegion = EntityRegion.Outside;
            UpdateCursor();
            Dragging = false;

        }

        private void HandleRightClick() {
            if (SelectedEntities.Count == 0) {
                return;
            }
            Entity clicked;
            if ((clicked = SelectedEntities.Find((e) => e.ContainsPosition(MapEditor.Instance.State.PixelPointer))) == null) {
                return;
            }
            HandleSelectedEntity(clicked);
        }

        public void HandleSelectedEntity(Entity entity) {
            Engine.CreateWindow(new WindowEntityEdit(this, entity, SelectedEntities));
        }

        public void UpdateCursor() {
            SDL2.SDL.SDL_SystemCursor updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW;
            if (SelectedEntities.Count == 1) {
                EntityRegion Region;
                if (HeldRegion != EntityRegion.Outside) {
                    Region = HeldRegion;
                }
                else {
                    Region = SelectedEntities[0].GetEntityRegion(MapEditor.Instance.State.PixelPointer);
                }
                switch (Region) {
                case EntityRegion.TopLeft:
                case EntityRegion.BottomRight:
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE;
                    break;
                case EntityRegion.TopRight:
                case EntityRegion.BottomLeft:
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW;
                    break;
                case EntityRegion.Left:
                case EntityRegion.Right:
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
                    break;
                case EntityRegion.Top:
                case EntityRegion.Bottom:
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
                    break;
                case EntityRegion.Middle:
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                    break;
                default:
                    break;
                }
            }
            else if (SelectedEntities.Count > 1) {
                Entity held;
                if ((held = SelectedEntities.Find((e) => e.ContainsPosition(MapEditor.Instance.State.PixelPointer))) != null) {
                    updatedCursor = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                }
            }
            UIHelper.SetCursor(updatedCursor);
        }
    }
}
