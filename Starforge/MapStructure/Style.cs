using Starforge.MapStructure.Encoding;

namespace Starforge.MapStructure {
    public class Style : MapElement {
        public float Alpha {
            get => GetFloat("alpha");
            set => SetAttribute("alpha", value);
        }

        public bool Always {
            get => GetBool("always");
            set => SetAttribute("always", value);
        }

        public string Color {
            get => GetString("color");
            set => SetAttribute("color", value);
        }

        public string Exclude {
            get => GetString("exclude");
            set => SetAttribute("exclude", value);
        }

        public string Flag {
            get => GetString("flag");
            set => SetAttribute("flag", value);
        }

        public bool FlipX {
            get => GetBool("flipx");
            set => SetAttribute("flipx", value);
        }

        public bool FlipY {
            get => GetBool("flipy");
            set => SetAttribute("flipy", value);
        }

        public bool InstantIn {
            get => GetBool("instantIn");
            set => SetAttribute("instantIn", value);
        }

        public bool InstantOut {
            get => GetBool("instantOut");
            set => SetAttribute("instantOut", value);
        }

        public bool LoopX {
            get => GetBool("loopx");
            set => SetAttribute("loopx", value);
        }

        public bool LoopY {
            get => GetBool("loopy");
            set => SetAttribute("loopy", value);
        }

        public string NotFlag {
            get => GetString("notflag");
            set => SetAttribute("notflag", value);
        }

        public string Only {
            get => GetString("only");
            set => SetAttribute("only", value);
        }

        public string Tag {
            get => GetString("tag");
            set => SetAttribute("tag", value);
        }

        public float WindMultiplier {
            get => GetFloat("wind");
            set => SetAttribute("wind", value);
        }

        public float X {
            get => GetFloat("x");
            set => SetAttribute("x", value);
        }

        public float Y {
            get => GetFloat("y");
            set => SetAttribute("y", value);
        }

        public float ScrollX {
            get => GetFloat("scrollx");
            set => SetAttribute("scrollx", value);
        }

        public float ScrollY {
            get => GetFloat("scrolly");
            set => SetAttribute("scrolly", value);
        }

        public float SpeedX {
            get => GetFloat("speedx");
            set => SetAttribute("speedx", value);
        }

        public float SpeedY {
            get => GetFloat("speedy");
            set => SetAttribute("speedy", value);
        }

        public static Style FromBinary(BinaryMapElement element) {
            Style style = new Style();
            style.Attributes = element.Attributes;

            return style;
        }

        public override BinaryMapElement ToBinary() {
            throw new System.NotImplementedException();
        }
    }

    public class Parallax : Style {
        public string BlendMode {
            get => GetString("blendmode");
            set => SetAttribute("blendmode", value);
        }

        public bool FadeIn {
            get => GetBool("fadeIn");
            set => SetAttribute("fadeIn", value);
        }

        public string Texture {
            get => GetString("texture");
            set => SetAttribute("texture", value);
        }

        public Parallax() {
            SetAttribute("atlas", "game");
        }

        public static new Parallax FromBinary(BinaryMapElement element) {
            return new Parallax
            {
                Attributes = element.Attributes
            };
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = "parallax"
            };

            bin.Attributes = Attributes;

            return bin;
        }
    }

    public class Effect : Style {
        public string Name;

        public Effect() { }

        public Effect(BinaryMapElement el) {
            Attributes = el.Attributes;
            Name = el.Name;
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = Name
            };

            bin.Attributes = Attributes;
            return bin;
        }
    }
}