﻿using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("player")]
    public class Player : Entity {
        private static DrawableTexture Texture;

        public Player(EntityData data, Room room) : base(data, room) {
            Texture = GFX.Gameplay["characters/player/sitDown00"];
        }

        public override void Render() {
            Vector2 pos = Position;
            pos.Y -= 16f;
            Texture.DrawCentered(pos);
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Player (Spawn Point)")
        };
    }
}
