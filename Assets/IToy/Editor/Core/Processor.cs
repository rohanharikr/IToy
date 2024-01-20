using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy.Core
{
    public class Processor
    {
        public Texture2D Texture;

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
            _flipHorizontalMat = new Material(Shader.Find("IToy/Transform/FlipHorizontal"));
            _flipVerticalMat = new Material(Shader.Find("IToy/Transform/FlipVertical"));
            _cropMat = new Material(Shader.Find("IToy/Transform/Crop"));
            _brightnessMat = new Material(Shader.Find("IToy/Correction/Brightness"));
            _contrastMat = new Material(Shader.Find("IToy/Correction/Contrast"));
            _hueMat = new Material(Shader.Find("IToy/Correction/Hue"));
            _saturationMat = new Material(Shader.Find("IToy/Correction/Saturation"));
            #endregion
        }

        public void RemoveBackground(Color removeColor)
        {
            _backgroundMat.SetColor("_RemoveColor", removeColor);
            Texture = Utility.ApplyShader(Texture, _backgroundMat);
            _backgroundMat.RevertAllPropertyOverrides();
        }

        public void Crop(RectInt crop)
        {
            _cropMat.SetInteger("_CropTop", crop.x);
            _cropMat.SetInteger("_CropRight", crop.y);
            _cropMat.SetInteger("_CropBottom", crop.width);
            _cropMat.SetInteger("_CropLeft", crop.height);
            Texture = Utility.ApplyShader(Texture, _cropMat);
            _cropMat.RevertAllPropertyOverrides();
        }

        public void FlipHorizontal() => Texture = Utility.ApplyShader(Texture, _flipHorizontalMat);

        public void FlipVertical() => Texture = Utility.ApplyShader(Texture, _flipVerticalMat);

        public void Brightness(int value = 0) {
            _brightnessMat.SetInteger("_Correction", value);
            Texture = Utility.ApplyShader(Texture, _brightnessMat);
            _brightnessMat.RevertAllPropertyOverrides();
        }

        public void Contrast(int value = 0)
        {
            _contrastMat.SetInteger("_Correction", value);
            Texture = Utility.ApplyShader(Texture, _contrastMat);
            _contrastMat.RevertAllPropertyOverrides();
        }

        public void Hue(int value = 0)
        {
            _hueMat.SetInteger("_Correction", value);
            Texture = Utility.ApplyShader(Texture, _hueMat);
            _hueMat.RevertAllPropertyOverrides();
        }

        public void Saturation(int value = 0)
        {
            _saturationMat.SetInteger("_Correction", value);
            Texture = Utility.ApplyShader(Texture, _saturationMat);
            _saturationMat.RevertAllPropertyOverrides();
        }

        public void WriteToDisk(string path)
        {
            byte[] file = Texture.EncodeToPNG();
            //TBD Handle exception
            File.WriteAllBytes(path, file);
            AssetDatabase.Refresh();
        }
    }
}
