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
    StateColor[] states;
    [SerializeField]
    CellRule[] rules;

    float _aspectRatio;
    RenderTexture _prevTex = null;
    RenderTexture _currTex = null;
    ComputeBuffer _computeBuffer;


    private void Awake()
    {
        _aspectRatio = (float)Screen.width / Screen.height;

        QualitySettings.vSyncCount = 0; // for framerate controlling
    }

    private void Start()
    {
        _computeBuffer = new ComputeBuffer(rules.Length, sizeof(int)*6, ComputeBufferType.Constant | ComputeBufferType.Structured);

        _computeBuffer.SetData(rules);
        _automatonShader.SetBuffer(0, "rules", _computeBuffer);
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

        _computeBuffer?.Dispose();
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

    public Color getStateColor(int id)
    {
        foreach (StateColor state in states)
        {
            if (state.id == id)
                return state.color;
        }
        return Color.black;
    }
}
