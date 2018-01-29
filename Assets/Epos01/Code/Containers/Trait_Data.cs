using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Trait_description
{
    string _traitname; //e.g. work_lazy
    List<KeyValuePair<string, short>> _relations; //(work_lazy +5), (strong_weak +5)

    /*load a text file with
    work_lazy: work_lazy +5, strong_weak +3, ... //simple properties are symmetrical
    good: good_evil +5, strong_weak -2, ... //but a good guy does not simply hate everything that a bad guy likes
    evil: work_lazy -2, honest_liar -5, ...
    */
}


public class Trait_Data {

    public Trait_Data[] _positiveTraits = new Trait_Data[3];
    public Trait_Data[] _negativeTraits = new Trait_Data[3]; //the weights for those are inverted


}
