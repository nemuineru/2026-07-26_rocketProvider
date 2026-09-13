using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    [SerializeField]
    Mesh mesh_Grab, mesh_Release;
    MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isClicked = InputInstance.self.isClicked;
        meshFilter.mesh = isClicked ? mesh_Grab : mesh_Release;
        Plane plane = new Plane(Vector3.up, GameSystem.self.PartPlane.transform.position);
       
        if (plane.Raycast(GameSystem.self.MainRay, out float enter))
        {
            Vector3 pos = GameSystem.self.MainRay.GetPoint(enter) 
            + Vector3.up * (isClicked ? 2 : 3);
            transform.position = Vector3.Lerp(transform.position, pos, .1f);
        }
        transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
    }
}
