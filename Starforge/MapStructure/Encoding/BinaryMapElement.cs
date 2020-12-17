using System.Collections.Generic;

namespace Starforge.MapStructure.Encoding {
    public class BinaryMapElement : BaseMapElement {
        public List<BinaryMapElement> Children = new List<BinaryMapElement>();
        public string Name;
        public string Package;
        public void AddList<T>(List<T> objects, string name) where T : MapElement {
            if (objects.Count <= 0) return;

            BinaryMapElement binaryMapElement = new BinaryMapElement()
            {
                Name = name
            };
            foreach (T obj in objects) {
                binaryMapElement.Children.Add(obj.ToBinary());
            }
            Children.Add(binaryMapElement);
        }
    }
}
