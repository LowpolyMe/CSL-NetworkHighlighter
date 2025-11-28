using System.Xml.Serialization;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    [XmlRoot("NetworkNetworkHighlightSettings")]
    public class Config
    {
        public int Version { get; set; } = 1;
        public float PedestrianPathsHue { get; set; } = 0.85f; 
        public float RoadsHue { get; set; } = 0.85f; 
        public float HighwaysHue { get; set; } = 0.85f; 
        public float TrainTracksHue  { get; set; } = 0.85f;
        public float MetroTracksHue  { get; set; } = 0.85f;
        public float TramTracksHue   { get; set; } = 0.85f;
        public float MonorailHue     { get; set; } = 0.85f;
        public float CableCarsHue  { get; set; } = 0.85f;
        
        public bool HighlightPedestrianPaths { get; set; } = true; // ai is PedestrianPathAI, PedestrianWayAI, if HighlightBridges is true: PedestrianBridgeAI, if HighlightTunnels is true: PedestrianTunnelAI
        public bool HighlightRoads { get; set; } = true; // ai is RoadAI, RoadBridgeAI, RoadTunnelAI
        public bool HighlightHighways { get; set; } = true; // will need custom code probably
        public bool HighlightTrainTracks { get; set; } = true; //TrainTrackAI, TrainTrackBridgeAI, TrainTrackTunnelAI
        public bool HighlightMetroTracks { get; set; } = true; //MetroTrackAI
        public bool HighlightTramTracks { get; set; } = true; // couldn't find
        public bool HighlightMonorailTracks { get; set; } = true; // MonorailTrackAI
        public bool HighlightCableCars { get; set; } = true; // CableCarPathAI
        
        public bool HighlightBridges { get; set; } = true;
        public bool HighlightTunnels { get; set; } = true;
    }
}