namespace Core.Entities
{
    public class BotColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public BotColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}