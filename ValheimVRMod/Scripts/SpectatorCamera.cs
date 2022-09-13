using static ValheimVRMod.Utilities.LogUtils;

using UnityEngine;
using ValheimVRMod.Utilities;

namespace ValheimVRMod.Scripts
{
    public class SpectatorCamera : MonoBehaviour
    {
        private Camera _followCam;
        private Camera _specSkyboxCamera;
        private Camera _mainCamera;
        Transform vrCameraTransform;

        private Vector3 offset = new Vector3(0, 2, -2);
        private float initial_MaincameraFOV;

        private enum cameraFollowType
        {
            SuperHot,
            VanityFollow,
            StabilizedFPV
        }



        void Start()
        {
            InitalizeCamera();
            initial_MaincameraFOV = _mainCamera.fieldOfView;

        }




        void Update()
        {
            Camera specSkyboxCam = CameraUtils.getCamera(CameraUtils.SPECTATOR_SKYBOX_CAMERA);

            _followCam.enabled = VHVRConfig.UseSpectatorCamera();
            if (_followCam.enabled == true)
            {
                _mainCamera.fieldOfView = _followCam.fieldOfView;
            }

            else
            {
                _mainCamera.fieldOfView = initial_MaincameraFOV;
            }

            if (specSkyboxCam != null)
            {
                specSkyboxCam.enabled = _followCam.enabled;
            }

        }

        void LateUpdate()
        {

            if (_followCam.enabled && Player.m_localPlayer != null)
            {
                Transform HeadtargetTransform = Player.m_localPlayer.m_animator.GetBoneTransform(HumanBodyBones.Head);
                Transform localPlayerTransform = Player.m_localPlayer.GetTransform();

                if (vrCameraTransform == null)
                {
                    vrCameraTransform = CameraUtils.getCamera(CameraUtils.VR_CAMERA).transform;
                }

                float fpvCamFOV = VHVRConfig.GetfpvCamFOV();
                float fpvNearClipPlane = VHVRConfig.GetfpvCamNearClipPlane();
                float fpvCamPosDamp = VHVRConfig.GetfpvCamPositionDampening();
                float fpvCamRotDamp = VHVRConfig.GetfpvCamRotationDampening();
                float fpvCamZOffset = VHVRConfig.GetfpvCamZPositionOffset();

                CameraStabilizedFPV(vrCameraTransform, fpvCamFOV, fpvNearClipPlane, false, fpvCamZOffset, fpvCamPosDamp, fpvCamRotDamp);

            }

        }

        private void CalculateAndSetFOV(float fieldOfView)
        {
            float oldfov = _followCam.fieldOfView;
            _followCam.fieldOfView = fieldOfView;
            _specSkyboxCamera.fieldOfView += (fieldOfView - oldfov);
        }




        private void CameraStabilizedFPV(Transform targetTransform, float fieldOfView, float nearClipPlane, bool lockRotation, float zOffset, float positionDampening, float rotationDampening)
        {
            _followCam.nearClipPlane = nearClipPlane;

            CalculateAndSetFOV(fieldOfView);

            var velocity = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position + targetTransform.forward * zOffset, ref velocity, positionDampening);



            float angularVelocity = 0f;
            float delta = Quaternion.Angle(transform.rotation, targetTransform.rotation);
            if (delta > 0f)
            {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, rotationDampening);
                t = 1.0f - (t / delta);



                if (lockRotation) // Broken, still working on it.
                {
                    /*
                    float rollCorrectionSpeed = 1f;
                   
                    float roll = Vector3.Dot(transform.right, Vector3.up);  
                    transform.Rotate(0, 0, -roll * rollCorrectionSpeed);
                    Vector3 directionVector = targetTransform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(directionVector);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetTransform.rotation, (rotationDampening * 1000) * Time.deltaTime);
                    */

                    transform.rotation = Quaternion.LookRotation(targetTransform.forward);
                    // broken when looking up and down
                    //float lockZ = 0f;
                    // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockZ);


                }

                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, t);
                }
            }



        }

        private void InitalizeCamera()
        {
            _followCam = GetComponent<Camera>();
            _specSkyboxCamera = CameraUtils.getCamera(CameraUtils.SPECTATOR_SKYBOX_CAMERA);
            _mainCamera = CameraUtils.getCamera(CameraUtils.MAIN_CAMERA);
        }

        public void resetCamera()
        {
            if (Player.m_localPlayer != null)
            {
                Vector3 playerPosition = Player.m_localPlayer.transform.position;
                transform.position = new Vector3(playerPosition.x, playerPosition.y + offset.y, playerPosition.z + offset.z);

            }
        }
    }
}