using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcBall : MonoBehaviour
{
    public Transform initialViewTarget;

    public float angularSpeed; // also angular velocity
    public float moveSpeed = 10.0f;

    protected Plane groundPlane;
    protected Vector3 lookAt;      // a lookat position for an invisible cursor
    protected bool dragging = false;

    public float radius;

    protected Vector2 lastMousePos;

    // Start is called before the first frame update
    void Start()
    {
        if (initialViewTarget)
        {
            groundPlane = new Plane(Vector3.up, initialViewTarget.position);
            lookAt = initialViewTarget.position;
            radius = (lookAt - transform.position).magnitude;
        } else
        {
            Vector3 zero = new Vector3(0.0f, 0.0f, 0.0f);
            groundPlane = new Plane(Vector3.up, zero);
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
            if (groundPlane.Raycast(ray, out float enter))
            {
                lookAt = ray.GetPoint(enter);
            }
            else
                lookAt = zero;
            radius = (lookAt - transform.position).magnitude;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // first check for horizontal and vertical motion
        Vector3 HVMotion = Input.GetAxis("Horizontal") * Vector3.forward * moveSpeed +
                           Input.GetAxis("Vertical") * Vector3.left * moveSpeed;
        //        HVMotion = transform.TransformDirection(HVMotion);      // convert into camera space.

        transform.Translate(transform.InverseTransformDirection(HVMotion * Time.deltaTime));
        // do arcball
        Vector3 dir = transform.position - lookAt;
        dir.Normalize();

        
        // right I can't get this to work (tomorrow maybe)
        /*
        if(Input.GetMouseButton(2))
        {
            if(dragging)
            {
                Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                lastMousePos = mousePos;
                dragging = true;
            } else
            {
                // get mouse location
                Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                Vector2 mouseDiff = lastMousePos - mousePos;
                Vector2 mouseDiffWorld = transform.TransformDirection(mouseDiff); // convert update into world co-ordinates

                Vector3 dirToCamera = lookAt - transform.position;
                float radius = dirToCamera.magnitude;
                //dirToCamera.Normalize();
                dirToCamera += new Vector3(mouseDiffWorld.x, mouseDiffWorld.y, 1.0f);
                dirToCamera.Normalize();

                Vector3 newCameraPosition = lookAt + dirToCamera * radius;
                Vector3 newAngle = -dirToCamera;
                transform.position = newCameraPosition;
                transform.rotation = Quaternion.FromToRotation(newCameraPosition, lookAt);


                
                /*
                Vector3 rotateBy = mouseDiff.x * Vector3.right + mouseDiff.y * Vector3.up;
                Vector3 oldOnSphere = ProjectToSphere(lastMousePos);
                Vector3 newOnSphere = ProjectToSphere(mousePos);

                Debug.DrawRay(transform.position, oldOnSphere, Color.black, 1.0f, false);
                Debug.DrawRay(transform.position, newOnSphere, Color.red, 1.0f, false);

                Debug.Log(rotateBy);
                float angle = Mathf.Acos(Mathf.Min(1.0f, Vector3.Dot(oldOnSphere, newOnSphere)));

                Vector3 axisInCameraCoords = Vector3.Cross(oldOnSphere, newOnSphere);
                Debug.DrawRay(transform.position, axisInCameraCoords, Color.green, 1.0f, false);

                transform.Rotate(rotateBy);

                lastMousePos = mousePos; 
            }
        } */
        
    }

    public static Vector3 ProjectToSphere(Vector2 location)
    {
        Vector3 mloc = new Vector3(location.x, location.y, 0.0f);
        // basicaly find location on a sphere
        if (mloc.x * mloc.x + mloc.y * mloc.y <= 1.0f)
            mloc.z = Mathf.Sqrt(1.0f - mloc.x * mloc.x + mloc.y * mloc.y);
        else
            mloc.Normalize();

        return mloc;
    }
}

