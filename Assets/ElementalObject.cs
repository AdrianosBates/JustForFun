using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ElementalObject : MonoBehaviour {

    [SerializeField, ShowListValue]
    private List<ElementalStatus> activeEffects;
    private List<ElementalStatus> newActiveEffects;

    private List<GameObject> objectsInteractedWithThisFixedUpdate;
    private List<GameObject> elementalObjectsTouching;

    [SerializeField]
    private int groupNum;

    public ElementalStatusRules elementalStatusRuleset;

    public bool touchingOtherElementalObject;
    public bool settleElementInteractions;

    public MeshRenderer meshRenderer;
    public Collider2D myCollider2D;

    // Awake is called when the script instance is being loaded
    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        objectsInteractedWithThisFixedUpdate = new List<GameObject>();
        myCollider2D = GetComponent<Collider2D>();

        var groupingObj = FindObjectOfType<ElementalObjectGrouping>();
        groupingObj.allElements.Add(this);
    }

    // Start is called before the first frame update
    void Start() {
        UpdateActiveEffects();
    }

    // Update is called once per frame
    void Update() {
        DecayActiveEffects();
    }

    private void FixedUpdate() {
        if (touchingOtherElementalObject) {
            objectsInteractedWithThisFixedUpdate.Clear();
            touchingOtherElementalObject = false;
        }

        if (settleElementInteractions) {
            settleElementInteractions = false;

            newActiveEffects = elementalStatusRuleset.SettleInteractions(activeEffects);
            SyncNewEffects();
        }
    }

    // OnCollisionStay2D happens BEFORE FixedUpdate
    private void OnCollisionStay2D(Collision2D collision) {
        var otherElementalObject = collision.gameObject.GetComponent<ElementalObject>();
        if (otherElementalObject != null) {
            if (!objectsInteractedWithThisFixedUpdate.Contains(collision.gameObject)) {
                var otherEffects = otherElementalObject.GetActiveEffects();

                // Combine other effects with my own
                CombineEffects(otherEffects);
                // and force the other object to combine my effects with its own
                otherElementalObject.CombineEffects(activeEffects);

                //sync effects for both objects
                SyncNewEffects();
                otherElementalObject.SyncNewEffects();

                // flag this object as already interacted with for the other object
                otherElementalObject.AddAlreadyInteracted(this.gameObject);
            }

            if (activeEffects.Count > 1) {
                settleElementInteractions = true;
            }

            touchingOtherElementalObject = true;
        }
    }

    public void CombineEffects(List<ElementalStatus> toCombine) {
        newActiveEffects = new List<ElementalStatus>(activeEffects);

        /* First, ensure that my list contains all the types of effects neccessary */
        for (int i = 0; i < toCombine.Count; i++) {
            // if an effect in toCombine does not exist in my list
            if (!newActiveEffects.Exists(x => x.effect == toCombine[i].effect)) {
                // add it to my new active effects, but set its value to zero
                var newStatus = toCombine[i];
                newStatus.value = 0;

                newActiveEffects.Add(newStatus);
            }
        }

        /* Then, add and average all of the toCombine values for myself */
        for (int i = 0; i < newActiveEffects.Count; i++) {
            // if toCombine has my effect, then add and average it
            if (toCombine.Exists(x => x.effect == newActiveEffects[i].effect)) {
                var toCombineValue = toCombine.Find(x => x.effect == newActiveEffects[i].effect).value;

                var newStatus = newActiveEffects[i];
                newStatus.value = (newStatus.value + toCombineValue) / 2f;
                newActiveEffects[i] = newStatus;
            } else {
                //if toCombine does not have my effect, it will take half of it, so divide by two.
                var newStatus = newActiveEffects[i];
                newStatus.value /= 2f;
                newActiveEffects[i] = newStatus;
            }
        }

        // flag 
    }

    private void DecayActiveEffects() {
        bool effectsChanged = false;

        for (int i = 0; i < activeEffects.Count; i++) {
            // if this effect has any decay
            if (activeEffects[i].effect.absoluteDecayPerSecond > 0) {
                var newStatus = activeEffects[i];
                newStatus.value -= (activeEffects[i].effect.absoluteDecayPerSecond / groupNum) * Time.deltaTime;
                activeEffects[i] = newStatus;

                effectsChanged = true;
            }
        }

        if (effectsChanged) {
            SyncCurrentEffects();
        }
    }

    private void CheckRemoveActiveEffects() {
        List<int> itemsToRemoveIndexes = new List<int>();

        for (int i = 0; i < activeEffects.Count; i++) {
            if (activeEffects[i].value <= 0.01f) {
                itemsToRemoveIndexes.Add(i);
            }
        }

        for (int i = 0; i < itemsToRemoveIndexes.Count; i++) {
            RemoveEffect(itemsToRemoveIndexes[i] - i);
        }
    }

    public List<ElementalStatus> GetActiveEffects() {
        return activeEffects;
    }

    public void UpdateActiveEffects() {
        List<Material> existingMaterials = new List<Material>();
        meshRenderer.GetSharedMaterials(existingMaterials);

        for (int i = 0; i < activeEffects.Count; i++) {
            int materialIndex;
            if (!existingMaterials.Contains(activeEffects[i].effect.material)) {
                existingMaterials.Add(activeEffects[i].effect.material);
                materialIndex = existingMaterials.Count - 1;
            } else {
                materialIndex = existingMaterials.FindIndex(x => x == activeEffects[i].effect.material);
            }
        }

        meshRenderer.sharedMaterials = existingMaterials.ToArray();

        for (int i = 0; i < activeEffects.Count; i++) {
            int materialIndex;
            materialIndex = existingMaterials.FindIndex(x => x == activeEffects[i].effect.material);

            var props = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(props, materialIndex);
            props.SetFloat("_Thickness", activeEffects[i].value);
            meshRenderer.SetPropertyBlock(props, materialIndex);
        }
    }

    public void RemoveEffect(int index) {
        List<Material> existingMaterials = new List<Material>();
        meshRenderer.GetSharedMaterials(existingMaterials);

        existingMaterials.Remove(activeEffects[index].effect.material);
        meshRenderer.materials = existingMaterials.ToArray();

        activeEffects.RemoveAt(index);
    }

    public void AddAlreadyInteracted(GameObject g) {
        objectsInteractedWithThisFixedUpdate.Add(g);
    }

    public void SyncNewEffects() {
        activeEffects = newActiveEffects;
        SyncCurrentEffects();
    }

    public void SyncCurrentEffects() {
        CheckRemoveActiveEffects();
        UpdateActiveEffects();
    }

    public List<ElementalObject> GetCollidingElementalObjects() {
        List<ElementalObject> collidingObjects = new List<ElementalObject>();

        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int numContacts = myCollider2D.GetContacts(contacts);

        for (int i = 0; i < numContacts; i++) {
            var elementalObject = contacts[i].collider.GetComponent<ElementalObject>();
            if (elementalObject != null && !collidingObjects.Contains(elementalObject)) {
                collidingObjects.Add(elementalObject);
            }
        }

        return collidingObjects;
    }

    public void SetGroupNum(int n) {
        groupNum = n;
    }

    private void OnDestroy() {
        var groupingObj = FindObjectOfType<ElementalObjectGrouping>();
        if (groupingObj != null) {
            groupingObj.allElements.Remove(this);
        }
    }
}