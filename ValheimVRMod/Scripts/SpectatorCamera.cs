using static ValheimVRMod.Utilities.LogUtils;
using static ValheimVRMod.Patches.MeshHider;

using UnityEngine;
using System.Collections.Generic;
using ValheimVRMod.Utilities;

namespace ValheimVRMod.Scripts
{
    public class SpectatorCamera : MonoBehaviour
    {
        public bool isFade;
        private bool isfollowCamEnabled;
        public HashSet<Camera> specatorCameraSet;

        private Camera _mainCamera;
        private Camera _followCam;
        private Camera _specSkyboxCamera;
        private Camera _specUICamera;
        private Camera _worldSpaceUiSpectatorCamera;

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
            
            isfollowCamEnabled = VHVRConfig.UseSpectatorCamera();

            if (isfollowCamEnabled && !isFade)
            {
                foreach(Camera camera in specatorCameraSet)
                {
                    camera.enabled = true;
                }

                if (VHVRConfig.HideSpecUI() == true)
                {
                    setSpecUICameraEnabled(false);
                }

                _mainCamera.fieldOfView = _followCam.fieldOfView;
            }

            else
            {
                foreach (Camera camera in specatorCameraSet)
                {
                    camera.enabled = false;
                }
                _mainCamera.fieldOfView = initial_MaincameraFOV;
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
                Vector3 fpvCamOffset = VHVRConfig.GetfpvCamZPositionOffset();

                CameraStabilizedFPV(vrCameraTransform, fpvCamFOV, fpvNearClipPlane, fpvCamOffset, fpvCamPosDamp, fpvCamRotDamp);

            }

        }

        private void CalculateAndSetFOV(float fieldOfView)
        {
            float oldfov = _followCam.fieldOfView;
            _followCam.fieldOfView = fieldOfView;
            _specSkyboxCamera.fieldOfView += (fieldOfView - oldfov);
        }




        private void CameraStabilizedFPV(Transform targetTransform, float fieldOfView, float nearClipPlane, Vector3 offset, float positionDampening, float rotationDampening)
        {
            _followCam.nearClipPlane = nearClipPlane;

            CalculateAndSetFOV(fieldOfView);

            var velocity = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position
                                                                        + (targetTransform.right * offset.x)
                                                                        + (targetTransform.up * offset.y)
                                                                        + (targetTransform.forward * offset.z),
                                                                        ref velocity, positionDampening);



            float angularVelocity = 0f;
            float delta = Quaternion.Angle(transform.rotation, targetTransform.rotation);
            if (delta > 0f)
            {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, rotationDampening);
                t = 1.0f - (t / delta);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, t);
                
            }



        }

        private void InitalizeCamera()
        {
            specatorCameraSet = new HashSet<Camera>();
            _followCam = GetComponent<Camera>();
            _specSkyboxCamera = CameraUtils.getCamera(CameraUtils.SPECTATOR_SKYBOX_CAMERA);
            _mainCamera = CameraUtils.getCamera(CameraUtils.MAIN_CAMERA);

            createUiPanelCamera();
            CreateWorldSpaceSpecUICamera();

            specatorCameraSet.Add(_followCam);
            specatorCameraSet.Add(_specSkyboxCamera);

        }

        private void createUiPanelCamera()
        {
            GameObject uiPanelCameraObj = new GameObject(CameraUtils.SPECTATOR_UI_CAMERA);
            _specUICamera = uiPanelCameraObj.AddComponent<Camera>();
            _specUICamera.CopyFrom(_followCam);
            _specUICamera.depth = 501;
            _specUICamera.clearFlags = CameraClearFlags.Depth;
            _specUICamera.renderingPath = RenderingPath.Forward;
            _specUICamera.cullingMask = LayerUtils.UI_PANEL_LAYER_MASK;
            _specUICamera.transform.SetParent(_followCam.transform);
            _specUICamera.enabled = true;

            specatorCameraSet.Add(_specUICamera);
        }

        private void CreateWorldSpaceSpecUICamera()
        {
            Camera worldSpaceUiCamera = CameraUtils.getCamera(CameraUtils.WORLD_SPACE_UI_CAMERA);

            if (worldSpaceUiCamera != null)
            {
                GameObject worldSpaceUiSpectatorCamParent = new GameObject(CameraUtils.WORLD_SPACE_UI_SPECTATOR_CAMERA);
                _worldSpaceUiSpectatorCamera = worldSpaceUiSpectatorCamParent.AddComponent<Camera>();
                _worldSpaceUiSpectatorCamera.CopyFrom(worldSpaceUiCamera);
                _worldSpaceUiSpectatorCamera.depth = 502;
                _worldSpaceUiSpectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;
                worldSpaceUiSpectatorCamParent.transform.SetParent(_followCam.transform);

                specatorCameraSet.Add(_worldSpaceUiSpectatorCamera);

            }
        }

        public void resetCameraToTransform()
        {
            if (Player.m_localPlayer != null)
            {
                Vector3 playerPosition = Player.m_localPlayer.transform.position;
                transform.position = new Vector3(playerPosition.x, playerPosition.y + offset.y, playerPosition.z + offset.z);

            }
        }

        public void resetSpectatorCameraToVRCamera()
        {
            Camera vrCamera = CameraUtils.getCamera(CameraUtils.VR_CAMERA);
            if (vrCamera != null)
            {
                transform.position = vrCamera.gameObject.transform.position;
                transform.rotation = vrCamera.gameObject.transform.rotation;
            }
        }

        private void setSpecCameraEnabled(bool isEnabled)
        {
            _followCam.enabled = isEnabled;
            _specSkyboxCamera.enabled = isEnabled;
        }

        private void setSpecUICameraEnabled(bool isEnabled)
        {
            _specUICamera.enabled = isEnabled;
            _worldSpaceUiSpectatorCamera.enabled = isEnabled;
        }

    }
}