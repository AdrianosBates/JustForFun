using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
[CreateAssetMenu(fileName = "ElementalStatusRules", menuName = "ScriptableObjects/ElementalStatusRules", order = 1)]
public class ElementalStatusRules : ScriptableObject {
    public List<ElementalStatusPair> combines;
    public List<ElementalStatusPair> negates;

    public List<ElementalStatus> SettleInteractions(List<ElementalStatus> allActiveEffects) {
        List<ElementalStatus> settledEffects = new List<ElementalStatus>(allActiveEffects);

        for (int i = 0; i < settledEffects.Count; i++) {
            var status = settledEffects[i];
            var specificCombines = GetCombines(status.effect);
            var specificNegates = GetNegates(status.effect);
            var value = status.value;

            foreach (var e in specificCombines) {
                if (allActiveEffects.Exists(x => x.effect == e)) {
                    var opposingValue = allActiveEffects.Find(x => x.effect == e).value;
                    value += opposingValue;
                    value = value < 0 ? 0 : value;
                }
            }

            foreach (var e in specificNegates) {
                if (allActiveEffects.Exists(x => x.effect == e)) {
                    var opposingValue = allActiveEffects.Find(x => x.effect == e).value;
                    value -= opposingValue;
                    value = value < 0 ? 0 : value;
                }
            }

            //effect.value = value;
            var newStatus = settledEffects[i];
            newStatus.value = value;
            settledEffects[i] = newStatus;
        }

        return settledEffects;
    }

    private List<ElementalStatusEffect> GetCombines(ElementalStatusEffect effect) {
        List<ElementalStatusEffect> allCombines = new List<ElementalStatusEffect>();

        foreach (var pair in combines) {
            if (pair.a == effect) {
                allCombines.Add(pair.b);
            } else if (pair.b == effect) {
                allCombines.Add(pair.a);
            }
        }

        // combines shouldn't contain themselves
        Assert.IsFalse(allCombines.Contains(effect));

        return allCombines;
    }

    private List<ElementalStatusEffect> GetNegates(ElementalStatusEffect effect) {
        List<ElementalStatusEffect> allNegates = new List<ElementalStatusEffect>();

        foreach (var pair in negates) {
            if (pair.a.effectName == effect.effectName) {
                allNegates.Add(pair.b);
            } else if (pair.b.effectName == effect.effectName) {
                allNegates.Add(pair.a);
            }
        }

        // negates shouldn't contain themselves
        Assert.IsFalse(allNegates.Contains(effect));

        return allNegates;
    }
}

[System.Serializable]
public struct ElementalStatusPair {
    public ElementalStatusEffect a;
    public ElementalStatusEffect b;
}
