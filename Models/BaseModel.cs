namespace BMHRI.WCS.Server.Models
{
    public class BaseModel
    {
        public int GroupID { get; set; }
        public string ModelName { get; set; }
        public string ModelID { get; set; }
        public string PLCID { get; set; }
        public ModelType ModelType { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        // public string DeviceID { get; set; }
        public double GetCenterX()
        {
            return (MaxX - MinX) / 2;
        }
        public double GetCenterY()
        {
            return (MaxY - MinY) / 2;
        }
        public double GetWidth()
        {
            return MaxX - MinX;
        }
        public double GetHeight()
        {
            return MaxY - MinY;
        }
    }
    public enum ModelType
    {
        VerticalDatumPoint,
        FloorPlane,  //巷道
        PalletChainMachine,
        PalletRolleMachine,
        PalletLiftMachine,
        Shelves,  //货架
        RailGuidedVehicle,    //RGV
        PalletStackMachine,
        PalletHoistMachine,
        PalletRolleNoPower,
        Pallet,
        RGVerticalRailWay,    //RGV垂直轨道
        RGVHorizontalRailWay,   //RGV水平轨道
        TextDescription,
        Led
    }
}
