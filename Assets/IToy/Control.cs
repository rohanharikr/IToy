using UnityEngine;

public class IToyControl : ScriptableObject
{
    /*
     * Why byte[]?
     * We keep the original file around for restoration.
     * We do not want the original file i.e. temporary file to show up in the editor.
     * We could hide the file (using special char/reserved) but then Unity would not see it for us to reference.
     */
    [SerializeField]
    public byte[] Original;

    /*
     * Why Texture2D?
     * Unlike Original (above), this file is meant to be shown to users i.e. Unity can see it as well.
     * 
     * Why not byte[]?
     * Because the Current image can potentially be edited unlike original which is static,
     * easier for Current image to be used directly in the preview without conversions.
     */
    [SerializeReference]
    public Texture2D Current;
}
