
namespace BlitzBit {

    public partial class BlitHybrid {

        public bool active => connected || hosting;

        public bool connected { get; private set; }
        public bool hosting { get; private set; }
    }
}
