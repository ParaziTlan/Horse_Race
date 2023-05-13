namespace HorseRace
{
    public static class Extensions
    {
        public static float Remap(this float value, float min1, float max1, float min2, float max2)
            => (value - min1) / (max1 - min1) * (max2 - min2) + min2;
    }
}