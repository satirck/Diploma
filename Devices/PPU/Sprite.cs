namespace Devices.PPU;

public class Sprite
{
    public ushort Width;
    public ushort Height;

    public List<Pixel> PColData;

    public Sprite(ushort w, ushort h)
    {
        Width = w;
        Height = h;
        PColData = Enumerable.Repeat(Pixel.Default, Width * Height).ToList();
    }

    public bool SetPixel(int x, int y, Pixel p)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PColData[y * Width + x] = p;
            return true;
        }

        return false;
    }
}