using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy.Core
{
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
            _backgroundMat = new Material(Shader.Find("IToy/Background"));
            _flipHorizontalMat = new Material(Shader.Find("IToy/FlipHorizontal"));
            _flipVerticalMat = new Material(Shader.Find("IToy/FlipVertical"));
            _cropMat = new Material(Shader.Find("IToy/Crop"));
            _brightnessMat = new Material(Shader.Find("IToy/Brightness"));
            _contrastMat = new Material(Shader.Find("IToy/Contrast"));
            _hueMat = new Material(Shader.Find("IToy/Hue"));
            _saturationMat = new Material(Shader.Find("IToy/Saturation"));
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
            _cropMat.SetInteger("_CropTop", crop.x);
            _cropMat.SetInteger("_CropRight", crop.y);
            _cropMat.SetInteger("_CropBottom", crop.width);
            _cropMat.SetInteger("_CropLeft", crop.height);
            _texture = Utility.ApplyShader(_texture, _cropMat);
            _cropMat.RevertAllPropertyOverrides();
        }

        public void FlipHorizontal() => _texture = Utility.ApplyShader(_texture, _flipHorizontalMat);

        public void FlipVertical() => _texture = Utility.ApplyShader(_texture, _flipVerticalMat);

        public void Brightness(int value = 0) {
            _brightnessMat.SetInteger("_Correction", value);
            _texture = Utility.ApplyShader(_texture, _brightnessMat);
            _brightnessMat.RevertAllPropertyOverrides();
        }

        public void Contrast(int value = 0)
        {
            _contrastMat.SetInteger("_Correction", value);
            _texture = Utility.ApplyShader(_texture, _contrastMat);
            _contrastMat.RevertAllPropertyOverrides();
        }

        public void Hue(int value = 0)
        {
            _hueMat.SetInteger("_Correction", value);
            _texture = Utility.ApplyShader(_texture, _hueMat);
            _hueMat.RevertAllPropertyOverrides();
        }

        public void Saturation(int value = 0)
        {
            _saturationMat.SetInteger("_Correction", value);
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
}
