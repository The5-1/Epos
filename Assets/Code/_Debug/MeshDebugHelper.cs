using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// https://www.youtube.com/watch?v=F2TfRaZ6COQ procedrural mesh generation
/// 
/// https://youtu.be/8LTDFwWMlqQ?t=1404 debug with gizmos
/// https://youtu.be/8LTDFwWMlqQ?t=1482 mesh.doptimize does nothing
/// https://youtu.be/8LTDFwWMlqQ?t=1604 mesh.MarkDynamic for frequently changing meshes
/// https://youtu.be/8LTDFwWMlqQ?t=1618 !!!Double buffering meshes!!!
/// https://youtu.be/8LTDFwWMlqQ?t=1763 move a mesh as skinned mesh
/// </summary>

public class MeshDebugHelper : MonoBehaviour{

    public List<Vector3> vertices;
    public List<int> indices;
    public List<Vector2> uvs;

    private void OnDrawGizmos()
    {
        foreach (Vector3 v in vertices)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(v, 0.5f);

        }
    }
}
