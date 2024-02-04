using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCellularAutomaton : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    ComputeShader _automatonShader;
    [SerializeField]
    ComputeShader _initShader;
    [SerializeField]
    Material _drawMat;

    [Header("Settings")]
    [SerializeField]
    int _resolution = 100;
    [Range(5, 500), SerializeField]
    int _targetFrameRate = 20;

    [SerializeField]
    Color[] _stateColors;
    [SerializeField]
    CellRule[] _rules;

    float _aspectRatio;
    RenderTexture _prevTex = null;
    RenderTexture _currTex = null;
    ComputeBuffer _rulesBuffer;
    ComputeBuffer _colorsBuffer;


    private void Awake()
    {
        _aspectRatio = (float)Screen.width / Screen.height;

        QualitySettings.vSyncCount = 0; // for framerate controlling
    }

    private void Start()
    {
        _rulesBuffer = new ComputeBuffer(_rules.Length, sizeof(int)*6, ComputeBufferType.Structured);
        _rulesBuffer.SetData(_rules);
        _automatonShader.SetBuffer(0, "rules", _rulesBuffer);

        _colorsBuffer = new ComputeBuffer(_stateColors.Length, sizeof(float)*4);

        Color[] linearColors = new Color[_stateColors.Length];
        for (int i = 0; i < _stateColors.Length; i++)
        {
            linearColors[i] = _stateColors[i].linear;
        }
        _colorsBuffer.SetData(linearColors);
        _drawMat.SetBuffer("_stateColors", _colorsBuffer);
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

        _rulesBuffer?.Dispose();
        _colorsBuffer?.Dispose();
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
            _initShader.SetInt("stateAmount", _stateColors.Length);
            _initShader.Dispatch(0, _currTex.width / 8, _currTex.height / 8, 1);
        }

        _prevTex = _currTex;
    }

    public Color getStateColor(int id)
    {
        return (id >= 0 && id < _stateColors.Length)? _stateColors[id] : Color.black;
    }
}
