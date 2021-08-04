using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveBacteria : MonoBehaviour
{
/*
    public GameObject plane;
    public float goTImer = 1f;
    private float timer;
    private float speed = 0.02f;

    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= goTImer)
        //{

            Vector3 newPos = RandomMove();
        if(Vector3.Distance(transform.position, newPos)>4)
            transform.Translate(newPos.x * Time.deltaTime, newPos.y * Time.deltaTime, newPos.z * Time.deltaTime);
          //  timer = 0f;
        //}
       // float step = speed;// * Time.deltaTime;
        

      
    }
    public Vector3 RandomMove()
    {
        Vector3 min = plane.GetComponent<MeshFilter>().mesh.bounds.min;
        Vector3 max = plane.GetComponent<MeshFilter>().mesh.bounds.max;

       float  x = Random.Range(min.x, max.x);
        float y = Random.Range(min.z, max.z);
        // transform.position = plane.transform.position - new Vector3((Random.Range(min.x * 0.1f, max.x * 0.1f)), plane.transform.position.y, (Random.Range(min.z * 0.1f, max.z * 0.1f)));
        // this.transform.Translate(new Vector3((Random.Range(min.x * 0.1f, max.x * 0.1f))* Time.deltaTime*5, this.transform.position.y * Time.deltaTime * 5, (Random.Range(min.z * 0.1f, max.z * 0.1f)) * Time.deltaTime * 5));

        return new Vector3(x, 0, y);/*
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, (new Vector3(x, 0, y)), step);

        if (Vector3.Distance(transform.position, new Vector3(x, 0, y)) < 0.001f)
        {
            // Swap the position of the cylinder.
            target.position *= -1.0f;
        }
        //(new Vector3(x, 0, y));*/
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bacteria")
        {
            //  Vector3 newPos = RandomMove();
            //transform.Translate(newPos.x * Time.deltaTime, newPos.y * Time.deltaTime, newPos.z * Time.deltaTime);
            //BookController.Instance.RandomMove(this.gameObject);
           // BookController.Instance.spawnedObjs.Remove(this.gameObject);
          //  Destroy(this.gameObject);
        }
    }
}
