using System.Collections.Generic;
using UnityEngine;
using ValheimVRMod.Utilities;
using ValheimVRMod.VRCore;
using Valve.VR;

namespace ValheimVRMod.Scripts {
    
    class CrossbowMorphManager : MonoBehaviour
    {
        public static CrossbowMorphManager instance { get; private set; }

        // The max distance allowed between the dominant hand and the nocking point to start pulling the string.
        private const float MaxNockingDistance = 0.2f;

        private bool initialized = false;
        private CrossbowAnatomy anatomy;
        private Transform leftLimbBone;
        private Transform rightLimbBone;
        private Transform stringLeft;
        private Transform stringRight;
        private Transform pullStart;
        private GameObject pullObj;
        private LineRenderer stringRenderer;
        private Vector3 pullDirection;
        private float maxDrawDelta;

        // Draw percentage restriction due to the vanilla reload animation progress.
        // Note: we make draw length proportional to the square root of reload progress.
        private float vanillaDrawPercentageRestriction;
        private float realLifeDrawPercentage;
        private float drawPercentage {
            get {
                return Mathf.Min(vanillaDrawPercentageRestriction, realLifeDrawPercentage);
            }
        }

        public bool isPulling { get; private set; }
        public bool shouldAutoReload { get { return anatomy == null; } }

        void Start()
        {
            instance = this;
        }

        public void UpdateWeaponLoading(Player player, float dt) {
            if (player != Player.m_localPlayer || shouldAutoReload)
            {
                return;
            }

            Player.MinorActionData action = GetReloadAction(player);
            vanillaDrawPercentageRestriction = action == null ? 0 : ReloadPercentageToDrawPercentage(action.m_time / action.m_duration);
            if (action != null)
            {
                action.m_time = Mathf.Min(Mathf.Max(DrawPercentageToReloadPercentage(realLifeDrawPercentage) * action.m_duration, 0), action.m_time);
            }
        }

        void Awake()
        {
            anatomy = CrossbowAnatomy.getAnatomy(Player.m_localPlayer.GetLeftItem().m_shared.m_name);
            if (anatomy == null)
            {
                return;
            }
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = GetBowBendingMaterial(meshRenderer.material);
            createBones();
            createNewString();
            initialized = true;
        }

        void OnRenderObject()
        {
            if (!initialized)
            {
                return;
            }

            updatePullStatus();

            if (Player.m_localPlayer.IsWeaponLoaded())
            {
                realLifeDrawPercentage = 1;
            }
            else if (!isPulling)
            {
                realLifeDrawPercentage = 0;
            }
            else
            {
                pullObj.transform.position = VRPlayer.dominantHand.transform.position;
                Vector3 pullVector = pullObj.transform.localPosition - anatomy.restingNockingPoint;
                realLifeDrawPercentage = Mathf.Max(0, Vector3.Dot(pullVector, pullDirection) / maxDrawDelta);
            }

            MorphBow();
        }

        void OnDisable()
        {
            if (isPulling)
            {
                VrikCreator.ResetHandConnectors();
                isPulling = false;
            }
        }

        void OnDestroy() {
            if (!initialized)
            {
                return;
            }
            Destroy(leftLimbBone.gameObject);
            Destroy(rightLimbBone.gameObject);
            Destroy(stringLeft.gameObject);
            Destroy(stringRight.gameObject);
            Destroy(pullStart.gameObject);
            Destroy(pullObj);
        }

        private Player.MinorActionData GetReloadAction(Player player)
        {
            if (!player.IsReloadActionQueued())
            {
                return null;
            }
            foreach (Player.MinorActionData action in player.m_actionQueue)
            {
                if (action.m_type == Player.MinorActionData.ActionType.Reload)
                {
                    return action;
                }
            }
            return null;
        }

        private static float ReloadPercentageToDrawPercentage(float reloadPercentage)
        {
            return Mathf.Sqrt(Mathf.Max(reloadPercentage, 0));
        }

        private static float DrawPercentageToReloadPercentage(float drawPercentage)
        {
            return Mathf.Max(drawPercentage * drawPercentage, 0);
        }

        private void createBones()
        {
            leftLimbBone = new GameObject().transform;
            leftLimbBone.parent = transform;
            leftLimbBone.localPosition = anatomy.hardLimbLeft;
            leftLimbBone.rotation = Quaternion.identity;

            rightLimbBone = new GameObject().transform;
            rightLimbBone.parent = transform;
            rightLimbBone.localPosition = anatomy.hardLimbRight;
            rightLimbBone.rotation = Quaternion.identity;
        }

        private void createNewString() {
            stringLeft = new GameObject().transform;
            stringLeft.parent = transform;
            stringLeft.localPosition = anatomy.restingStringLeft;

            stringRight = new GameObject().transform;
            stringRight.parent = transform;
            stringRight.localPosition = anatomy.restingStringRight;

            pullStart = new GameObject().transform;
            pullStart.parent = transform;
            pullStart.localPosition = anatomy.restingNockingPoint;

            pullObj = new GameObject();
            pullObj.transform.parent = transform;
            pullObj.transform.localPosition = Vector3.zero;

            pullDirection = (anatomy.anchorPoint - anatomy.restingNockingPoint).normalized;
            maxDrawDelta = (anatomy.anchorPoint - anatomy.restingNockingPoint).magnitude;

            stringRenderer = gameObject.AddComponent<LineRenderer>();
            stringRenderer.useWorldSpace = true;
            stringRenderer.widthMultiplier = 0.006f;
            stringRenderer.positionCount = 3;
            stringRenderer.material.color = new Color(0.4f, 0.33f, 0.31f);
            stringRenderer.material.SetFloat("_Glossiness", 0);
            stringRenderer.material.SetFloat("_Smoothness", 0);
            stringRenderer.material.SetFloat("_Metallic", 0);
            updateStringRenderer();
        }

        private void updatePullStatus()
        {
            bool wasPulling = isPulling;
            bool loaded = Player.m_localPlayer.IsWeaponLoaded();
            isPulling =
                !loaded &&
                SteamVR_Actions.valheim_Grab.GetState(VRPlayer.dominantHandInputSource) &&
                (wasPulling || Vector3.Distance(VRPlayer.dominantHand.transform.position, pullStart.position) <= MaxNockingDistance);
            if (isPulling)
            {
                if (!Player.m_localPlayer.IsReloadActionQueued())
                {
                    LogUtils.LogWarning("Crossbow pull start");
                    Player.m_localPlayer.ResetLoadedWeapon();
                    Player.m_localPlayer.QueueReloadAction();
                }
                VrikCreator.GetDominantHandConnector().position = stringRenderer.GetPosition(1);
            }
            else if (wasPulling)
            {
                LogUtils.LogWarning("Crossbow pull stop");
                VrikCreator.ResetHandConnectors();
                if (!Player.m_localPlayer.IsWeaponLoaded())
                {
                    Player.m_localPlayer.CancelReloadAction();
                }
            }
        }

        private void updateStringRenderer()
        {
            stringRenderer.SetPosition(0, stringLeft.position);
            stringRenderer.SetPosition(1, transform.TransformPoint(Vector3.Lerp(anatomy.restingNockingPoint, anatomy.anchorPoint, drawPercentage)));
            stringRenderer.SetPosition(2, stringRight.position);
        }
         
         private void MorphBow()
         {
            // Just a heuristic and simplified approximation for the bend angle.
            float bendAngleDegrees = Mathf.Asin(drawPercentage * Mathf.Sin(anatomy.maxBendAngleRadians)) * 180 / Mathf.PI;

            if (bendAngleDegrees > 0)
            {
                if (stringLeft.parent != leftLimbBone)
                {
                    stringLeft.SetParent(leftLimbBone, true);
                }
                if (stringRight.parent != rightLimbBone)
                {
                    stringRight.SetParent(rightLimbBone, true);
                }
            }

            leftLimbBone.localRotation = Quaternion.Euler(0, 0, bendAngleDegrees);
            rightLimbBone.localRotation = Quaternion.Euler(0, 0, -bendAngleDegrees);
            Quaternion leftLimbRotation = Quaternion.AngleAxis(bendAngleDegrees, Vector3.forward);
            Quaternion rightLimbRotation = Quaternion.AngleAxis(-bendAngleDegrees, Vector3.forward);
            Matrix4x4 leftLimbTransform = Matrix4x4.TRS(anatomy.hardLimbLeft - leftLimbRotation * anatomy.hardLimbLeft, leftLimbRotation, Vector3.one);            
            Matrix4x4 rightLimbTransform = Matrix4x4.TRS(anatomy.hardLimbRight - rightLimbRotation * anatomy.hardLimbRight, rightLimbRotation, Vector3.one);
            gameObject.GetComponent<MeshRenderer>().material.SetMatrix("_UpperLimbTransform", rightLimbTransform);
            gameObject.GetComponent<MeshRenderer>().material.SetMatrix("_LowerLimbTransform", leftLimbTransform);

            updateStringRenderer();
        }

        private Material GetBowBendingMaterial(Material vanillaMaterial)
        {
            // TODO: Consider share this method with BowManager.
            Material bowBendingMaterial = Instantiate(VRAssetManager.GetAsset<Material>("BowBendingMaterial")); ;
            bowBendingMaterial.color = vanillaMaterial.color;
            bowBendingMaterial.mainTexture = vanillaMaterial.mainTexture;
            bowBendingMaterial.SetTexture("_BumpMap", vanillaMaterial.GetTexture("_BumpMap"));
            bowBendingMaterial.SetTexture("_MetallicGlossMap", vanillaMaterial.GetTexture("_MetallicGlossMap"));
            bowBendingMaterial.SetTexture("_EmissionMap", vanillaMaterial.GetTexture("_EmissionMap"));

            // A higher render queue is needed to to prevent certain shadow artifacts when using vertex and fragment shaders. It is not necesssary for surface shaders.
            // meshRenderer.material.renderQueue = 3000;

            bowBendingMaterial.SetVector("_HandleVector", Vector3.right);
            bowBendingMaterial.SetFloat("_HandleTopHeight", anatomy.hardLimbRight.x);
            bowBendingMaterial.SetFloat("_HandleBottomHeight", anatomy.hardLimbLeft.x);
            bowBendingMaterial.SetFloat("_SoftLimbHeight", 0.01f);

            bowBendingMaterial.SetVector("_StringTop", new Vector4(anatomy.restingStringRight.x, anatomy.restingStringRight.y, anatomy.restingStringRight.z, 1));
            bowBendingMaterial.SetVector("_StringTopToBottomDirection", new Vector4(-1, 0, 0, 0));
            bowBendingMaterial.SetFloat("_StringLength", Vector3.Distance(anatomy.restingStringLeft, anatomy.restingStringRight));
            bowBendingMaterial.SetFloat("_StringRadius", 0.005f);

            return bowBendingMaterial;
        }
    }
}
