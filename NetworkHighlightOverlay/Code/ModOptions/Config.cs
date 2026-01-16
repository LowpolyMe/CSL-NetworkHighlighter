using System.Xml.Serialization;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    [XmlRoot("NetworkNetworkHighlightSettings")]
    public class Config
    {
        public float HighlightStrength  { get; set; }  = 1f;
        public float HighlightWidth  { get; set; }     = 1.0f;
        public float PedestrianPathsHue { get; set; }  = 0.25f; 
        public float PinkPathsHue { get; set; }        = 0.95f;
        public float TerraformingNetworksHue { get; set; } = 0.3f;
        public float RoadsHue { get; set; }            = 0.5f; 
        public float HighwaysHue { get; set; } = 0.65f; 
        public float TrainTracksHue  { get; set; } = 0.1f;
        public float MetroTracksHue  { get; set; } = 0.01f;
        public float TramTracksHue   { get; set; } = 0.85f;
        public float MonorailHue     { get; set; } = 0.85f;
        public float CableCarsHue  { get; set; } = 0.85f;        

        
        public bool HighlightPedestrianPaths { get; set; } = true; 
        public bool HighlightPinkPaths { get; set; } = true; 
        public bool HighlightTerraformingNetworks { get; set; } = true;
        public bool HighlightRoads { get; set; } = true; 
        public bool HighlightHighways { get; set; } = true; 
        public bool HighlightTrainTracks { get; set; } = true;
        public bool HighlightMetroTracks { get; set; } = true; 
        public bool HighlightTramTracks { get; set; } = true;
        public bool HighlightMonorailTracks { get; set; } = true; 
        public bool HighlightCableCars { get; set; } = true; 
        
        public bool HighlightBridges { get; set; } = true;
        public bool HighlightTunnels { get; set; } = true;
    }
}
