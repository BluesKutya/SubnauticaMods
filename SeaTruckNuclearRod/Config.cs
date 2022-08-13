using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace SeaTruckNuclearRod.Configuration
{
    [Menu("Seatruck Nuclear Rod")]
    public class Config: ConfigFile
    {
        [Slider("Stored Nuclear Energy", 500, 15000, DefaultValue = 6000, Step = 100, Tooltip = "Stored energy per nuclear rod.")]
        public int StoredNuclearEnergy = 6000;
    }
}
