using UnityEngine;

using System.Collections.Generic;

using System; //to use [Serializable], else [System.Serializable]
using System.IO; //reading and writing to files
using System.Runtime.Serialization.Formatters.Binary; //to use binary serializer
using System.Xml.Serialization; //to use xml serializer




/// <summary>
/// Persistent Object
/// Defines base stats that the actor class uses for calculating active stats
/// Actor got a race property
/// </summary>
/// 
//[Serializable] by default, dont set the Attribute again or unity crashes!
//[CreateAssetMenu(fileName = "Race_", menuName = "PF Data/Race", order = 100)]
public class Race //: ScriptableObject
{
    public string _name = "A race";
    public string _lore = "lore";

    public Town _capital;
    public List<Color> _haircolors;
    public List<Color> _skincolors;
    public Color _bloodcolor;
    public Color _fleshcolor;
    public Color _bonecolor;

    public int _HP_base = 100;
    public int _MP_base = 100;
    public int _EP_base = 100;

    public int _AttackSpeed_base = 100;
    public int _CastSpeed_base = 100;
    public int _Movespeed_base = 100;

    public float _ground_SpeedMult_base; //e.g. water creatures move slower when walking
    public float _air_SpeedMult_base; //e.g. winged creatures can move faster in the air
    public float _water_SpeedMult_base; //e.g. water creatures move faster in water
    public float _climb_SpeedMult_base; //e.g. creatures with claws can climb faster

    public int _JumpPower_base = 100;
    public int _NumJumps_base = 1;
    public int _MultijumpPower_base = 100;
    public float _MultijumpFalloff_base = 0.5f;
    public float _GlideFalloff_base = 0.5f;
    public float _GravityMultiplier_base = 1.0f;
    public float _FallDamageThresholdMultiplier_base = 1.0f;

    public float _WalkSpeedMult_base;
    public float _SprintSpeedMult_base;
    public float _SneakSpeedMult_base;

    public float _ground_SpeedFactor_base = 1.0f;
    public float _air_SpeedFactor_base = 1.0f;
    public float _water_SpeedFactor_base = 1.0f;

    public float _viewDistance_base = 100.0f;
    public float _viewDistanceDark_base = 50.0f;

    public int _bodyBrainHP_base = 100;
    public int _bodyTorsoHP_base = 100;
    public int _bodyLungsHP_base = 100;
    public int _bodyOrgansHP_base = 100;
    public int _bodyArmRHP_base = 100;
    public int _bodyArmLHP_base = 100;
    public int _bodyLegsHP_base = 100;
}



/*
//Serialize not needed, scriptable object instead
public void Save()
{
    XmlSerializer serializer = new XmlSerializer(typeof(Race));
    FileStream fs = File.Create(Application.persistentDataPath + "/Race.xml");
    serializer.Serialize(fs, this);
}

void OnDestroy()
{
    //Save();
}
*/
