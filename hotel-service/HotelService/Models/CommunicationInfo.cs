using System.Text.Json.Serialization;

namespace HotelService.Models
{
    public class CommunicationInfo
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string InfoType { get; set; }
        public string InfoDetails { get; set; }

        [JsonIgnore]
        public virtual Hotel? Hotel { get; set; }//lazy load
    }

}
