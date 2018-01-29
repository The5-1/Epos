using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this takes the compue shaders calculations and does things in unity with it
public class grid_MonoBehaviour : MonoBehaviour
{

    public ComputeShader computeShader; //the Compute Shader
    public ComputeBuffer computeBuffer; //stores the compute shaders result
    private const int posCount = 10 * 10 * 10 * 10 * 10 * 10;   //The compute Shader generates 10*10*10 threads which each generate a 10*10*10 block of positions
    private int CSKernelIndex; //the index of the desired kernel in the Compute Shader file


    public Shader drawPointsShader; //shader that actually draws the points this gets from the compute shader
    [SerializeField] private Material drawPointsMaterial; //material with the shader is generated in code
    public bool debugRender = true;

    public void Start()
    {
        CSKernelIndex = computeShader.FindKernel("CSMain"); //find the kernel in the Compute Shader file, returns its index

        if (debugRender)
        {
            drawPointsMaterial = new Material(drawPointsShader);
            drawPointsMaterial.SetVector("_worldPos", this.gameObject.transform.position); //set the shaders variable to this GameObjects transformation
        }

        initBuffers();
    }

    public void OnDisable()
    {
        ReleaseBuffers();
    }

    //This is called after the scene is rendered and is used to render custom geometry, e.g. toggle between high res and low res minimap
    public void OnRenderObject()
    {
        if (debugRender)
        {
            Dispatch();
            drawPointsMaterial.SetPass(0);
            drawPointsMaterial.SetVector("_worldPos", this.gameObject.transform.position); //set the shaders variable to this GameObjects transformation

            Graphics.DrawProcedural(MeshTopology.Points, posCount);
        }
    }


    protected void initBuffers()
    {
        computeBuffer = new ComputeBuffer(posCount, sizeof(float) * 3 + sizeof(int) * 6);

        computeShader.SetBuffer(CSKernelIndex, "outputBuffer", computeBuffer); //the "outputBuffer" in the compute shader code

        if (debugRender) drawPointsMaterial.SetBuffer("inputPoints", computeBuffer);
    }

    protected void Dispatch()
    {
        //check if the platform supports compute shaders
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogWarning("Compute shader is not supported, DX11 is required.");
            return;
        }

        computeShader.Dispatch(CSKernelIndex, 10, 10, 10);
    }

    protected void ReleaseBuffers()
    {
        computeBuffer.Release();
    }




}
