using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StickOrganizer : MonoBehaviour 
{
    [SerializeField] private float _offset = 20.0f;
    public GameObject[] ControllerModels;

    private List<GameObject> activeControllers = new List<GameObject>();

    void Start () 
    {
        if (ControllerModels == null || ControllerModels.Length == 0)
            ControllerModels = GameObject.FindGameObjectsWithTag("ControllerModel").OrderBy(o => o.transform.parent.GetSiblingIndex()).ToArray();

        foreach (GameObject model in ControllerModels)
        {
            if (model.activeInHierarchy)
                activeControllers.Add(model);
        }


        float center = ((activeControllers.Count - 1) * _offset) / 2;

        for (int i = 0; i < activeControllers.Count; i++)
        {
            activeControllers[i].transform.position = new Vector3((i * _offset) - center, activeControllers[i].transform.position.y, activeControllers[i].transform.position.z);
        }
    }
}