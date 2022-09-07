using static LandScapes;

namespace Assets.Scripts
{
    public interface IPlateau
    {
        public Mountains MountainTiles { get; }
        public Highlands HighlandTiles { get; }
        public Hills HillTiles { get; }

        public LandScapeTile GetRandomTile(Temperature temp, GroundLevel height);
        //{
        //    switch (height)
        //    {
        //        case HeightLevel.Hills:
        //            return HillTiles.GetRandomTile(temp);
        //        case HeightLevel.Highlands:
        //            return HighlandTiles.GetRandomTile(temp);
        //        case HeightLevel.Mountain:
        //            return MountainTiles.GetRandomTile(temp);
        //        default:
        //            return null;
        //    }
        //}

        public void SetPlateaus(Mountains mountains, Highlands highlands, Hills hills);
    }
}
