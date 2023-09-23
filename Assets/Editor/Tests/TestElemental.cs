using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestElemental {
    // A Test behaves as an ordinary method
    [Test]
    public void TestElementalSimplePasses() {
        // Use the Assert class to test conditions
        ElementalStatusEffect fire = (ElementalStatusEffect)Resources.Load("ScriptableObjects/ElementalStatusEffect/Fire");
        ElementalStatusEffect water = (ElementalStatusEffect)Resources.Load("ScriptableObjects/ElementalStatusEffect/Water");

        ElementalStatusRules rules = (ElementalStatusRules)Resources.Load("ScriptableObjects/ElementalStatusEffect/ElementalStatusRuleset");

        //ElementalStatusEffectDictionary activeEffects = new ElementalStatusEffectDictionary();
        //activeEffects.Add(fire,1);
        //activeEffects.Add(water,1);

        //ElementalStatusEffectDictionary result = rules.SettleInteractions(activeEffects);
        //foreach (var kvp in result) {
        //    Debug.Log(kvp.Key.effectName + ", " + kvp.Value);
        //}
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestElementalWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
