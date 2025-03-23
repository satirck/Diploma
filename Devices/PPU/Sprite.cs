using System.Drawing;
using System.Numerics;

namespace Devices.PPU;

public class Sprite
{
    private int _width, _height;
    
    public List<Vector4> PColData; 
    
    public Sprite(int w = 0, int h = 0)
    {
        _width = w;
        _height = h;
        
        PColData = new List<Vector4>(_width * _height);

        for (var i = 0; i < _width * _height; i++)
        {
            PColData.Add(Vector4.Zero);
        }
    }

    public bool SetPixel(int x, int y, Vector4 p)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height) return false;
        
        PColData[y * _width + x] = p;
        return true;
    }
}