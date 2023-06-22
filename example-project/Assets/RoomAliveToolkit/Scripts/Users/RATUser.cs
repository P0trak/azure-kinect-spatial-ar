using UnityEngine;

namespace RoomAliveToolkit
{
    /// <summary>
    /// Behavior which defines a user in the RoomAlive scene. The user's position in the scene is usually updated from Kinect data. 
    /// Usually, the user will also have an acompanying RATUserViewCamera for rendering user specific views. 
    /// </summary>
    [AddComponentMenu("RoomAliveToolkit/RATUser")]
    public class RATUser : MonoBehaviour
    {
        public RATSkeletonProvider skeletonProvider;
        public int skeletonID = 0; //each Kinect has 6 possible skeleton slots, this number indicates which spot one would request (not quite the same as a User ID


        public bool updateFromKinect = true;
        public GameObject lookAt = null; //always in world coordinates (not local to Kinect data)

        private Vector3 headPos = Vector3.zero;
        public Wall wall;


        public void Start()
        {
            if (skeletonProvider == null)
            {
                Debug.Log("User is missing a skeleton provider!");
                return;
            }
        }

        public RATKinectSkeleton GetSkeleton()
        {
            if (skeletonProvider == null)
                return null;
            else
            {
                return skeletonProvider.GetKinectSkeleton(skeletonID);
            }

        }

        public bool IsReady()
        {
            RATKinectSkeleton skel = GetSkeleton();
            if (skel == null)
            {
                Debug.Log("returned skelton is null!");
            } else if (!skel.valid)
            {
                Debug.Log("returned skelton is not valid!");
            }
            return skel != null && skel.valid;
        }

        public void SetHeadPosition(Vector3 headPos)
        {
            this.headPos = headPos; // new Vector3(headPos.x, headPos.y, headPos.z);
            //Debug.Log("head position at: " + headPos);
            if (wall != null)
            {
                wall.UpdateCalibration(headPos);
            }
        }

        public Vector3 getHeadPosition()
        {
            /* do we need all this
            if (IsReady())
            {
                //Vector3 pos = GetSkeleton().jointPositions3D[(int)JointType.Head]; // this is reported in the coordinate system of the skeleton provider
                return skeletonProvider.transform.localToWorldMatrix.MultiplyPoint(this.headPos); //this moves it to world coordinates
            }
            else
            {
                Debug.Log("not ready!");
                return Vector3.zero;
            }
            */
            return skeletonProvider.transform.localToWorldMatrix.MultiplyPoint(this.headPos); //this moves it to world coordinates


        }

        public void Update()
        {
            if (skeletonProvider != null)
            {
                /*
                if (IsReady())
                {
                */
                if (updateFromKinect)
                {
                    //Debug.Log("updating head position...");
                    transform.position = getHeadPosition();
                    if (lookAt != null)
                    {
                        transform.LookAt(lookAt.transform.position);
                    }
                    //else transform.localRotation = Quaternion.identity;
                }
                
                //}
            }

        }
    }
}
