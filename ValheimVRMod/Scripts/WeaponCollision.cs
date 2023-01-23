using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using ValheimVRMod.Scripts.Block;
using ValheimVRMod.Utilities;
using ValheimVRMod.VRCore;
using Valve.VR;

namespace ValheimVRMod.Scripts {
    public class WeaponCollision : MonoBehaviour {
        private const float MIN_SPEED = 1.5f;
        private const float MIN_STAB_SPEED = 1f;
        private const int MAX_SNAPSHOTS_BASE = 20;
        private const int MAX_SNAPSHOTS_FACTOR = -5;
        private const float MAX_STAB_ANGLE = 30f;
        private const float MAX_STAB_ANGLE_TWOHAND = 40f;

        private bool scriptActive;
        private GameObject colliderParent;
        private List<Vector3> snapshots;
        private List<Vector3> weaponHandleSnapshots;
        private ItemDrop.ItemData item;
        private Attack attack;
        private Attack secondaryAttack;
        private bool isRightHand;
        private Outline outline;
        private float hitTime;
        private bool hasDrunk;
        private float postSecondaryAttackCountdown;

        public PhysicsEstimator physicsEstimator { get; private set; }
        public PhysicsEstimator mainHandPhysicsEstimator { get { return weaponWield.mainHand == VRPlayer.leftHand ? VRPlayer.leftHandPhysicsEstimator : VRPlayer.rightHandPhysicsEstimator; } }
        public bool lastAttackWasStab { get; private set; }
        public float twoHandedMultitargetSwipeCountdown { get; private set; } = 0;
        public bool itemIsTool;
        public static bool isDrinking;
        public WeaponWield weaponWield;

        private int maxSnapshots;
        private float colliderDistance;

        private static readonly int[] ignoreLayers = {
            LayerUtils.WATERVOLUME_LAYER,
            LayerUtils.WATER,
            LayerUtils.UI_PANEL_LAYER,
            LayerUtils.CHARARCTER_TRIGGER
        };

        private void Awake()
        {
            colliderParent = new GameObject();

            // TODO: cleanup unused snapshot lists.
            snapshots = new List<Vector3>();
            weaponHandleSnapshots = new List<Vector3>();

            physicsEstimator = gameObject.AddComponent<PhysicsEstimator>();
            physicsEstimator.refTransform = CameraUtils.getCamera(CameraUtils.VR_CAMERA)?.transform.parent;
        }

        void Destroy()
        {
            Destroy(colliderParent);
        }

        private void OnTriggerStay(Collider collider) {

            if (!isCollisionAllowed()) {
                return;
            }

            if (!isRightHand || EquipScript.getRight() != EquipType.Tankard || collider.name != "MouthCollider" || hasDrunk) {
                return;
            }

            isDrinking = hasDrunk = weaponWield.mainHand.transform.rotation.eulerAngles.x > 0 && weaponWield.mainHand.transform.rotation.eulerAngles.x < 90;

            //bHaptics
            if (isDrinking && !BhapticsTactsuit.suitDisabled)
            {
                BhapticsTactsuit.PlaybackHaptics("Drinking");
            }

        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!isCollisionAllowed()) {
                return;
            }

            if (isRightHand && EquipScript.getRight() == EquipType.Tankard) {
                if (collider.name == "MouthCollider" && hasDrunk) {
                    hasDrunk = false;
                }

                return;
            }

            var maybePlayer = collider.GetComponentInParent<Player>();

            if (maybePlayer != null && maybePlayer == Player.m_localPlayer) {
                return;
            }

            if (item == null && !itemIsTool || !hasMomentum()) {
                return;
            }

            bool isSecondaryAttack = postSecondaryAttackCountdown <= 0 && RoomscaleSecondaryAttackUtils.IsSecondaryAttack(this);
            if (EquipScript.getRight() == EquipType.Polearms)
            {
                // Allow continuing an ongoing atgeir secondary attack (multitarget swipe) until cooldown finishes.
                isSecondaryAttack = postSecondaryAttackCountdown > 0 || isSecondaryAttack;
            }

            if (!tryHitTarget(collider.gameObject, isSecondaryAttack)) {
                return;
            }

            if (isSecondaryAttack && postSecondaryAttackCountdown <= 0)
            {
                postSecondaryAttackCountdown = WeaponUtils.GetAttackDuration(secondaryAttack);
            }

            Attack currentAttack = isSecondaryAttack ? secondaryAttack : attack;

            if (WeaponUtils.IsTwoHandedMultitargetSwipe(currentAttack) && twoHandedMultitargetSwipeCountdown <= 0)
            {
                twoHandedMultitargetSwipeCountdown = WeaponUtils.GetAttackDuration(currentAttack);
            }     

            StaticObjects.lastHitPoint = transform.position;
            StaticObjects.lastHitDir = physicsEstimator.GetVelocity().normalized;
            StaticObjects.lastHitCollider = collider;

            if (currentAttack.Start(Player.m_localPlayer, null, null,
                        Player.m_localPlayer.m_animEvent,
                        null, item, null, 0.0f, 0.0f))
            {
                if (isRightHand) {
                    VRPlayer.rightHand.hapticAction.Execute(0, 0.2f, 100, 0.5f, SteamVR_Input_Sources.RightHand);
                }
                else {
                    VRPlayer.leftHand.hapticAction.Execute(0, 0.2f, 100, 0.5f, SteamVR_Input_Sources.LeftHand);
                }
                //bHaptics
                if (!BhapticsTactsuit.suitDisabled)
                {
                    BhapticsTactsuit.SwordRecoil(!VHVRConfig.LeftHanded());
                }
            }
        }

        private bool tryHitTarget(GameObject target, bool isSecondaryAttack) {

            // ignore certain Layers
            if (ignoreLayers.Contains(target.layer)) {
                return false;
            }

            if (Player.m_localPlayer.m_blocking && !weaponWield.allowBlocking() && VHVRConfig.BlockingType() == "GrabButton")
            {
                return false;
            }

            if (Player.m_localPlayer.IsStaggering() || Player.m_localPlayer.InDodge())
            {
                return false;
            }

            // if attack is vertical, we can only hit one target at a time
            if (attack.m_attackType != Attack.AttackType.Horizontal && AttackTargetMeshCooldown.isPrimaryTargetInCooldown()) {
                return false;
            }

            if (target.GetComponentInParent<MineRock5>() != null) {
                target = target.transform.parent.gameObject;
            }

            var character = target.GetComponentInParent<Character>();
            if (character != null) {
                target = character.gameObject;
            }

            var attackTargetMeshCooldown = target.GetComponent<AttackTargetMeshCooldown>();
            if (attackTargetMeshCooldown == null) {
                attackTargetMeshCooldown = target.AddComponent<AttackTargetMeshCooldown>();
            }

            if (isSecondaryAttack)
            {
                return attackTargetMeshCooldown.tryTriggerSecondaryAttack(WeaponUtils.GetAttackDuration(secondaryAttack));
            }
            else
            {
                return attackTargetMeshCooldown.tryTriggerPrimaryAttack(WeaponUtils.GetAttackDuration(attack));
            }
        }

        private void OnRenderObject() {
            if (!isCollisionAllowed()) {
                return;
            }
            transform.SetParent(colliderParent.transform);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.SetParent(Player.m_localPlayer.transform, true);
        }

        public void setColliderParent(Transform obj, string name, bool rightHand) {
            outline = obj.parent.gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;

            isRightHand = rightHand;
            if (isRightHand) {
                item = Player.m_localPlayer.GetRightItem();
            }
            else {
                item = Player.m_localPlayer.GetLeftItem();
            }

            attack = item.m_shared.m_attack.Clone();
            secondaryAttack = item.m_shared.m_secondaryAttack.Clone();

            // Cleanup unused hitTime
            switch (attack.m_attackAnimation) {
                case "atgeir_attack":
                    hitTime = 0.81f;
                    break;
                case "battleaxe_attack":
                    hitTime = 0.87f;
                    break;
                case "knife_stab":
                    hitTime = 0.49f;
                    break;
                case "swing_longsword":
                case "spear_poke":
                    hitTime = 0.63f;
                    break;
                case "swing_pickaxe":
                    hitTime = 1.3f;
                    break;
                case "swing_sledge":
                    hitTime = 2.15f;
                    break;
                case "swing_axe":
                    hitTime = 0.64f;
                    break;
                default:
                    hitTime = 0.63f;
                    break;
            }

            itemIsTool = name == "Hammer";

            if (colliderParent == null) {
                colliderParent = new GameObject();
            }

            try {
                WeaponColData colliderData = WeaponUtils.getForName(name, item);
                colliderParent.transform.parent = obj;
                colliderParent.transform.localPosition = colliderData.pos;
                colliderParent.transform.localRotation = Quaternion.Euler(colliderData.euler);
                colliderParent.transform.localScale = colliderData.scale;
                colliderDistance = Vector3.Distance(colliderParent.transform.position, obj.parent.position);
                maxSnapshots = (int)(MAX_SNAPSHOTS_BASE + MAX_SNAPSHOTS_FACTOR * colliderDistance);
                setScriptActive(true);
            }
            catch (InvalidEnumArgumentException)
            {
                LogUtils.LogWarning($"Collider not found for: {name}");
                setScriptActive(false);
            }
        }

        private void Update() {

            if (!outline) {
                return;
            }

            bool inCooldown = AttackTargetMeshCooldown.isPrimaryTargetInCooldown();
            bool canDoPrimaryAttack =
                Player.m_localPlayer.HaveStamina(getStaminaUsage() + 0.1f) && (attack.m_attackType == Attack.AttackType.Horizontal || !inCooldown);
            if (!canDoPrimaryAttack)
            {
                outline.enabled = true;
                outline.OutlineColor = Color.red;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineWidth = 5;
            }
            else if (postSecondaryAttackCountdown > 0.5f) {
                outline.enabled = true;
                outline.OutlineColor = Color.Lerp(new Color(1, 1, 0, 0), new Color(1, 1, 0, 0.5f), postSecondaryAttackCountdown - 0.5f);
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineWidth = 10;
            }
            else
            {
                outline.enabled = false;
            }
        }

        private float getStaminaUsage() {

            if (attack.m_attackStamina <= 0.0) {
                return 0.0f;
            }
            double attackStamina = attack.m_attackStamina;
            return (float)(attackStamina - attackStamina * 0.330000013113022 * Player.m_localPlayer.GetSkillFactor(item.m_shared.m_skillType));
        }

        private bool isCollisionAllowed() {
            return scriptActive && VRPlayer.inFirstPerson && colliderParent != null;
        }

        private void setScriptActive(bool active) {
            scriptActive = active;

            if (!active) {
                snapshots.Clear();
                weaponHandleSnapshots.Clear();
            }
        }
        
        private void FixedUpdate() {
            if (postSecondaryAttackCountdown > 0)
            {
                postSecondaryAttackCountdown -= Time.fixedDeltaTime;
            }
            if (twoHandedMultitargetSwipeCountdown > 0)
            {
                postSecondaryAttackCountdown -= Time.fixedDeltaTime;
            }

            if (!isCollisionAllowed()) {
                return;
            }
            
            snapshots.Add(transform.localPosition);
            weaponHandleSnapshots.Add(weaponWield.mainHand.transform.position);
            if (snapshots.Count > maxSnapshots) {
                snapshots.RemoveAt(0);
            }
            if (weaponHandleSnapshots.Count > maxSnapshots) {
                weaponHandleSnapshots.RemoveAt(0);
            }
        }

        public bool hasMomentum() {
            lastAttackWasStab = isStab();

            if (lastAttackWasStab)
            {
                return true;
            }

            if (!VHVRConfig.WeaponNeedsSpeed()) {
                return true;
            }

            if (physicsEstimator.GetAverageVelocityInSnapshots().magnitude > MIN_SPEED + colliderDistance * 2)
            {
                return true;
            }

            return false;
        }

        private bool isStab()
        {
            Vector3 attackVelocity = mainHandPhysicsEstimator == null ? Vector3.zero : mainHandPhysicsEstimator.GetAverageVelocityInSnapshots();

            if (Vector3.Angle(WeaponWield.weaponForward, attackVelocity) > (WeaponWield.isCurrentlyTwoHanded() ? MAX_STAB_ANGLE_TWOHAND : MAX_STAB_ANGLE))
            {
                return false;
            }

            if (Vector3.Dot(attackVelocity, WeaponWield.weaponForward) > MIN_STAB_SPEED)
            {
                LogUtils.LogDebug("VHVR: stab detected on weapon direction: " + WeaponWield.weaponForward);
                return true;
            }
            return false;
        }
    }
}
