using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ElementalStatusEffect", menuName = "ScriptableObjects/ElementalStatusEffect", order = 1)]
public class ElementalStatusEffect : ScriptableObject {
    [SerializeField]
    private string _effectName;
    public string effectName {
        get { return _effectName; }
    }

    [SerializeField]
    private float _absoluteDecayPerSecond = 0.1f;
    public float absoluteDecayPerSecond {
        get { return _absoluteDecayPerSecond; }
    }

    [SerializeField]
    private Material _material;
    public Material material {
        get { return _material; }
    }
}

[System.Serializable]
public struct ElementalStatus {
    [SerializeField]
    private ElementalStatusEffect _effect;
    public ElementalStatusEffect effect {
        get { return _effect; }
    }

    public float value;
}
