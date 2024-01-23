using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy.Core
{
    public class Toy : ScriptableObject
    {
        //Store original image in SO since we do not want to show in inspector
        [HideInInspector]
        public byte[] Original;

        public string Current; //Asset GUID

        public RemoveBackgroundOpts RemoveBackground;

        public Transform Transform;

        public Correction Correction;

        public Color RemoveBackgroundColor;
    }

    public enum RemoveBackgroundOpts
    {
        None,
        White,
        Black,
        Custom,
    }

    [Serializable]
    public struct Transform
    {
        public bool FlipHorizontal = false;

        public bool FlipVertical = false;

        public RectInt Crop;
    }

    [Serializable]
    public struct Correction
    {
        [Range(-100, 100)]
        public int Brightness = 0;

        [Range(-100, 100)]
        public int Contrast = 0;

        [Range(-100, 100)]
        public int Hue = 0;

        [Range(-100, 100)]
        public int Saturation = 0;
    }
}
