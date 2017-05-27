/// <summary>
/// !!!DX11 doesn't support point mode rendering anymore (old fixed-function pipeline)!!! Today use geometry shader instead.
/// For OpenGL you can enable GL_VERTEX_PROGRAM_POINT_SIZE to use pointsize
/// 
/// https://blogs.msdn.microsoft.com/ivanne/2012/01/04/multiple-ways-to-render-point-sprites-in-dx11/ //performance comparison of new methods
/// http://answers.unity3d.com/questions/861891/how-do-you-change-point-size-in-direct3d-11-shader.html
/// https://forum.unity3d.com/threads/dx11-psize-bug-please-help.383220/
/// http://answers.unity3d.com/questions/519670/shader-inconsistency-between-opengl-and-directx.html
/// 
/// Usage in shader: via "PSIZE"
/// 
/// struct v2f
/// {
///     float4 vertex : SV_POSITION;
///     float4 color : COLOR;
///     float size : PSIZE;
/// };
/// 
/// </summary>


#if UNITY_STANDALONE
#define IMPORT_GLENABLE
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GL_Enable_PointSize : MonoBehaviour {

    const UInt32 GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642;
    const UInt32 GL_POINT_SMOOTH = 0x0B10;

    const string LibGLPath =
#if UNITY_STANDALONE_WIN
        "opengl32.dll";
#elif UNITY_STANDALONE_OSX
    "/System/Library/Frameworks/OpenGL.framework/OpenGL";
#elif UNITY_STANDALONE_LINUX
    "libGL";  // Untested on Linux, this may not be correct
#else
    null;   // OpenGL ES platforms don't require this feature
#endif

#if IMPORT_GLENABLE
    [DllImport(LibGLPath)]
    public static extern void glEnable(UInt32 cap);

    private bool mIsOpenGL;

    void Start()
    {
        mIsOpenGL = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
    }

    void OnPreRender()
    {
        if (mIsOpenGL)
            glEnable(GL_VERTEX_PROGRAM_POINT_SIZE);
        Debug.Log("glEnable(GL_VERTEX_PROGRAM_POINT_SIZE)");
        glEnable(GL_POINT_SMOOTH);
    }
#endif
}
