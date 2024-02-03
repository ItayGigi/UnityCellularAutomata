using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCellularAutomaton : MonoBehaviour
{
    [SerializeField]
    int _resolution = 100;
    [SerializeField]
    ComputeShader _automatonShader;
    [SerializeField]
    ComputeShader _initShader;
    [SerializeField]
    ComputeShader _drawShader;
    [SerializeField]
    Material _drawMat;

    float _aspectRatio;
    RenderTexture _prevTex = null;
    RenderTexture _currTex = null;

    private void Awake()
    {
        _aspectRatio = (float)Screen.width / Screen.height;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateTextures();
        _currTex.filterMode = FilterMode.Point;

        //RenderTexture texture = new RenderTexture(source);
        //texture.enableRandomWrite = true;
        //_drawShader.SetTexture(0, "Result", texture);
        //_drawShader.SetTexture(0, "Board", _currTex);
        //_drawShader.Dispatch(0, texture.width / 8, texture.height / 8, 1);

        // Render to screen
        Graphics.Blit(_currTex, destination, _drawMat);

        //texture.Release();
    }

    private void OnDisable()
    {
        // Release the allocated render texture in case the script is disabled or destroyed
        if (_prevTex != null)
            RenderTexture.ReleaseTemporary(_prevTex);
        _prevTex = null;

        if (_currTex != null)
            RenderTexture.ReleaseTemporary(_currTex);
        _currTex = null;
    }

    void UpdateTextures()
    {
        _currTex = RenderTexture.GetTemporary((int)(_resolution * _aspectRatio), _resolution, 0, RenderTextureFormat.RInt);
        _currTex.enableRandomWrite = true;

        if (_prevTex)
        {
            Graphics.Blit(_prevTex, _currTex);

            RenderTexture.ReleaseTemporary(_prevTex);
        }
        else
        {
            _initShader.SetTexture(0, "Result", _currTex);
            _initShader.SetInt("resolution", _resolution);
            _initShader.Dispatch(0, _currTex.width / 8, _currTex.height / 8, 1);
        }

        _prevTex = _currTex;
    }
}
