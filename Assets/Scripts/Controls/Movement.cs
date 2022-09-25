using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageSide
{
    Left,
    Right,
    All
}

public class Movement : MonoBehaviour
{
    private LayerMask stageLayerMask => LayerMask.GetMask("Stage");

    private float maxCastDist => 20;

    public StageSide StageSide;

    private Vector3 lastSnapInput = Vector2.zero;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 snapInput = GetSnapInput();

        if(snapInput != lastSnapInput)
        {
            Move(snapInput);
            lastSnapInput = snapInput;
        }


    }

    private Vector3 GetSnapInput()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        return new Vector3(Mathf.Sign(hInput) * Mathf.Ceil(Mathf.Abs(hInput)), 0, Mathf.Sign(vInput) * Mathf.Ceil(Mathf.Abs(vInput)));
    }

    private void Move(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, maxCastDist, stageLayerMask))
            if (hit.collider.GetComponent<FloorPanel>().StageSide == this.StageSide)
            {
                transform.position = new Vector3(hit.collider.gameObject.transform.position.x, transform.position.y, hit.collider.gameObject.transform.position.z);
            }


    }
}
