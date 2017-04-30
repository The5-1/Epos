using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorGenderColor : MonoBehaviour {

    public Actor _parentActor;
    public GameObject _parentGO;
    public Material _parentMaterial;
    public Actor_Gender _parentGender;
    public Color _color;


    //FIXME: this should not be called every frame but just when the actor changes! Event system...
    void FixedUpdate() {
        _parentGO = this.transform.parent.gameObject;
        _parentActor = _parentGO.GetComponent<Actor>();
        if (_parentActor)
        {
            _parentGender = _parentActor._actorData.breedData.gender;
            _parentMaterial = this.GetComponent<MeshRenderer>().material;
        
            switch (_parentGender)
            {
                case Actor_Gender.male:
                    _color = Color.blue; break;
                case Actor_Gender.female:
                    _color = Color.red; break;
                case Actor_Gender.solo:
                    _color = Color.green; break;
                case Actor_Gender.both:
                    _color = Color.yellow; break;
                case Actor_Gender.none:
                    _color = Color.grey; break;
            }
            _parentMaterial.color = _color;
        }
    }
}
