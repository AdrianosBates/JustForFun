using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalObjectGrouping : MonoBehaviour {
    public List<ElementalObject> allElements;
    public List<List<ElementalObject>> elementsGroupingList;

    // Awake is called when the script instance is being loaded
    void Awake() {
        allElements = new List<ElementalObject>();
        elementsGroupingList = new List<List<ElementalObject>>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        TraverseGroups();
        SetGroupValues();
    }

    private void TraverseGroups() {
        //reset grouping list
        elementsGroupingList = new List<List<ElementalObject>>();

        // for traversal
        List<ElementalObject> visited = new List<ElementalObject>();
        List<ElementalObject> notVisited = new List<ElementalObject>(allElements);
        Stack<ElementalObject> searchStack = new Stack<ElementalObject>();

        while (notVisited.Count > 0) {
            List<ElementalObject> currentGroup = new List<ElementalObject>();

            // add the first node the top of the stack
            searchStack.Push(notVisited[0]);

            // while there are nodes in the stack
            while (searchStack.Count > 0) {
                //mark the top item visited, add it to the current group list
                var topItem = searchStack.Pop();
                notVisited.Remove(topItem);
                visited.Add(topItem);
                currentGroup.Add(topItem);

                // add nodes that aren't yet visited to the top of the stack
                var adjacentNodes = topItem.GetCollidingElementalObjects();
                for (int i = 0; i < adjacentNodes.Count; i++) {
                    if (!visited.Contains(adjacentNodes[i])) {
                        searchStack.Push(adjacentNodes[i]);
                    }
                }
            }

            elementsGroupingList.Add(currentGroup);
        }
    }

    private void SetGroupValues() {
        //for every group
        for (int i = 0; i < elementsGroupingList.Count; i++) {
            //cycle through each elementalObject
            for (int k = 0; k < elementsGroupingList[i].Count; k++) {
                elementsGroupingList[i][k].SetGroupNum(elementsGroupingList[i].Count);
            }
        }
    }
}
