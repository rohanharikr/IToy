using IToy;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Processor
{
    private Texture2D _texture;
    public Texture2D Texture
    {
        get { return _texture; }
        set { _texture = value; }  
    }

    #region Materials
    readonly Material _backgroundMat;
    readonly Material _flipHorizontalMat;
    readonly Material _flipVerticalMat;
    readonly Material _cropMat;
    readonly Material _brightnessMat;
    readonly Material _contrastMat;
    readonly Material _hueMat;
    readonly Material _saturationMat;
    #endregion

    public Processor()
    {
        #region Init shaders
        Shader backgroundShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Background.shader");
        Shader flipHorizontalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/FlipHorizontal.shader");
        Shader flipVerticalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/FlipVertical.shader");
        Shader cropShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/Crop.shader");
        Shader brightnessShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Brightness.shader");
        Shader contrastShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Contrast.shader");
        Shader hueShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Hue.shader");
        Shader saturationShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Saturation.shader");
        _backgroundMat = new Material(backgroundShader);
        _flipHorizontalMat = new Material(flipHorizontalShader);
        _flipVerticalMat = new Material(flipVerticalShader);
        _cropMat = new Material(cropShader);
        _brightnessMat = new Material(brightnessShader);
        _contrastMat = new Material(contrastShader);
        _hueMat = new Material(hueShader);
        _saturationMat = new Material(saturationShader);
        #endregion
    }

    public void RemoveBackground(Color removeColor)
    {
        _backgroundMat.SetColor("_RemoveColor", removeColor);
        _texture = Utility.ApplyShader(_texture, _backgroundMat);
        _backgroundMat.RevertAllPropertyOverrides();
    }

    public void Crop(RectInt crop)
    {
        _cropMat.SetInteger("_Top", crop.x);
        _cropMat.SetInteger("_Right", crop.y);
        _cropMat.SetInteger("_Bottom", crop.width);
        _cropMat.SetInteger("_Left", crop.height);
        _texture = Utility.ApplyShader(_texture, _cropMat);
        _cropMat.RevertAllPropertyOverrides();
    }

    public void FlipHorizontal() => _texture = Utility.ApplyShader(_texture, _flipHorizontalMat);

    public void FlipVertical() => _texture = Utility.ApplyShader(_texture, _flipVerticalMat);

    public void Brightness(int value = 0) {
        _brightnessMat.SetInteger("_Brightness", value);
        _texture = Utility.ApplyShader(_texture, _brightnessMat);
        _brightnessMat.RevertAllPropertyOverrides();
    }

    public void Contrast(int value = 0)
    {
        _contrastMat.SetInteger("_Contrast", value);
        _texture = Utility.ApplyShader(_texture, _contrastMat);
        _contrastMat.RevertAllPropertyOverrides();
    }

    public void Hue(int value = 0)
    {
        _hueMat.SetInteger("_Hue", value);
        _texture = Utility.ApplyShader(_texture, _hueMat);
        _hueMat.RevertAllPropertyOverrides();
    }

    public void Saturation(int value = 0)
    {
        _saturationMat.SetInteger("_Saturation", value);
        _texture = Utility.ApplyShader(_texture, _saturationMat);
        _saturationMat.RevertAllPropertyOverrides();
    }

    public void WriteToDisk(string path)
    {
        byte[] file = _texture.EncodeToPNG();
        File.WriteAllBytes(path, file);
        AssetDatabase.Refresh();
    }
}
