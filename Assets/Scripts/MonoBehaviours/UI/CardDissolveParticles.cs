using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class CardDissolveParticles : MonoBehaviour
{
    private GraphicsBuffer _xRangeLineBuffer;

    private VisualEffect _visualEffect;

    private const string GRAPHICS_BUFFER_NAME = "XRangeInLineBuffer";
    private readonly int GRAPHICS_BUFFER_ID = Shader.PropertyToID(GRAPHICS_BUFFER_NAME);
    private const string IMAGE_HEIGHT_NAME = "ImageHeight";
    private readonly int IMAGE_HEIGHT_ID = Shader.PropertyToID(IMAGE_HEIGHT_NAME);

    private float CurrentScreenRatio => (float) Screen.width / Screen.height;
    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>();
        if(!_visualEffect.HasGraphicsBuffer(GRAPHICS_BUFFER_ID))
            Debug.LogError("The VFX doesn't have the '" + GRAPHICS_BUFFER_NAME + "' buffer", _visualEffect);
        _visualEffect.Stop();
    }

    public void Dissolve(Sprite sprite)
    {
        // I should probably cache these buffers that I create instead of creating and disposing them each time
        if(_xRangeLineBuffer != null && _xRangeLineBuffer.IsValid())
            _xRangeLineBuffer.Dispose();
        _xRangeLineBuffer = XRangeLine.SpriteToBuffer(sprite, CurrentScreenRatio);
        _visualEffect.SetGraphicsBuffer(GRAPHICS_BUFFER_ID, _xRangeLineBuffer);
        
        var height = sprite.bounds.extents.y;
        _visualEffect.SetFloat(IMAGE_HEIGHT_ID, CurrentScreenRatio * height);
        
        _visualEffect.Reinit();
    }
}

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct XRangeLine
{
    public float MinX;
    public float MaxX;

    public static XRangeLine EMPTY => new XRangeLine { MinX = -5000, MaxX = -5000 };

    public static GraphicsBuffer SpriteToBuffer(Sprite sprite, float screenRatio)
    {
        var height = (int) sprite.rect.height;
        var stride = Marshal.SizeOf<XRangeLine>();
        var graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, height, stride);

        var lines = new NativeArray<XRangeLine>(height, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var pixels = sprite.texture.GetPixelData<Color32>(0);
        var width = (int) sprite.rect.width;
        var widthInUnits = screenRatio * sprite.bounds.extents.x / width;
        for (var y = 0; y < height; y++)
        {
            float? minX = null;
            float? maxX = null;
            for (var x = 0; x < width; x++)
            {
                var pixelIndex = y * width + x;
                if (minX == null)
                {
                    if (pixels[pixelIndex].a > 0)
                        minX = x;
                }
                else if (pixels[pixelIndex].a == 0)
                {
                    maxX = x;
                    break;
                }
            }

            var line = XRangeLine.EMPTY;
            if (minX.HasValue)
            {
                if(maxX.HasValue)
                    line = new XRangeLine { MinX = minX.Value, MaxX = maxX.Value };
                else
                    line = new XRangeLine { MinX = minX.Value, MaxX = width };
                line = new XRangeLine { MinX = line.MinX * widthInUnits, MaxX = line.MaxX * widthInUnits };
            }

            lines[height - 1 - y] = line;
        }
        graphicsBuffer.SetData(lines);

        return graphicsBuffer;
    }
}