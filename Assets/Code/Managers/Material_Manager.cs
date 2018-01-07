using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Material_Manager_Material //classes rest on the heap rather than the stack!
{
    public readonly byte ID;
    public readonly string name;
    public Material material;

    public Material_Manager_Material(byte id, string n)
    {
        ID = id;
        name = n;
        updateMaterial();
    }

    public void updateMaterial()
    {
        //!!! Resources.Load() looks inside a folder called Resoruces that you have to create yourself!
        material = (Material)Resources.Load(name, typeof(Material));
        Debug.Log(string.Format("Material {0} loaded as: {1}", name, material));
    }
}



public class Material_Manager : MonoBehaviour {

    static public Material_Manager singleton;

    #region Materials
    public Dictionary<byte,Material_Manager_Material> materials;
    #endregion

    protected void Awake()
    {
        // Check if there is already a singleton instance
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton.gameObject);
            init();
        }
        else { Destroy(this); }
    }

    // This method is called when the script is destroy
    protected void OnDestroy()
    {
        // If this is the currently active singleton, delete the static instance too
        if (singleton == this) { singleton = null; }
    }

    private void init()
    {
        Debug.Log(string.Format("{0}.init()", this));

        materials = new Dictionary<byte, Material_Manager_Material>();

        loadMaterials();
    }

    private void loadMaterials()
    {
        //TODO: load from... somewhere
        loadMaterial(0, "Materials/Default_gray");
        loadMaterial(1, "Materials/VertexColorShaded_Material");
    }

    private void loadMaterial(byte id, string name)
    {
        materials.Add(id, new Material_Manager_Material(id, name));
    }

    public void updateMaterials()
    {
        foreach (Material_Manager_Material mat in materials.Values)
        {
            mat.updateMaterial();
        }
    }


}
