using AspCommon.Models;

namespace ResourceServer.Models
{
    public class Planet : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HexColor { get; set; }
        public decimal Radius { get; set; }
    }
}
