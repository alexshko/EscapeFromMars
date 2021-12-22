using UnityEngine;

namespace alexshko.defensetower.Core
{
    [ExecuteInEditMode]
    public class FollowCamera : MonoBehaviour
    {
        //reference to the camera that the object has to follow.
        //e.g. Lifebar on the prisoner that follows the camera
        private Camera cam;

        // Start is called before the first frame update
        void Awake()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            //transform.LookAt(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)));
            //Vector3 posToLook = cam.transform.position + cam.transform.forward;
            //posToLook.x = 0;
            transform.LookAt(cam.transform.position + cam.transform.forward);
            Quaternion curRot = transform.rotation;
            curRot.eulerAngles = new Vector3(curRot.eulerAngles.x, 180, curRot.eulerAngles.z);
            transform.rotation = curRot;
        }
    }
}
