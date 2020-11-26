using System.Collections.Generic;

namespace Starforge.MapStructure.Encoding {
    public class BinaryMapElement : BaseMapElement {
        public List<BinaryMapElement> Children = new List<BinaryMapElement>();
        public string Name;
        public string Package;
    }
}
