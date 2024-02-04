using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DrawCellularAutomaton : MonoBehaviour
{
    [SerializeField]
    int _resolution = 100;
    [SerializeField]
    ComputeShader _automatonShader;
    [SerializeField]
    ComputeShader _initShader;
    [SerializeField]
    Material _drawMat;
    [Range(5, 500), SerializeField]
    int _targetFrameRate = 20;

    float _aspectRatio;
    RenderTexture _prevTex = null;
    RenderTexture _currTex = null;

    private void Awake()
    {
        _aspectRatio = (float)Screen.width / Screen.height;

        QualitySettings.vSyncCount = 0; // for framerate controlling
    }

    private void Update()
    {
        // update framerate
        Application.targetFrameRate = _targetFrameRate;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateTextures();

        // Render to screen
        Graphics.Blit(_prevTex, destination, _drawMat);
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
        // create new temporary texture
        _currTex = RenderTexture.GetTemporary((int)(_resolution * _aspectRatio), _resolution, 0, RenderTextureFormat.RInt);
        _currTex.enableRandomWrite = true;

        if (_prevTex) // update the board
        {
            _automatonShader.SetTexture(0, "Board", _prevTex);
            _automatonShader.SetTexture(0, "Result", _currTex);
            _automatonShader.SetInt("resolution", _resolution);
            _automatonShader.Dispatch(0, _currTex.width / 8, _currTex.height / 8, 1);

            RenderTexture.ReleaseTemporary(_prevTex); // release last frame's texture
        }
        else // first frame, initialize board
        {
            _initShader.SetTexture(0, "Result", _currTex);
            _initShader.SetInt("resolution", _resolution);
            _initShader.Dispatch(0, _currTex.width / 8, _currTex.height / 8, 1);
        }

        _prevTex = _currTex;
    }
}
