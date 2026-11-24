using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

// one click setup for the cat player
// in Unity go to Tools > Ryan > Setup Cat Player
// it can be run again safely, it replaces the old cat model each time
public class CatSetup
{
    private const string catModelPath = "Assets/Team/Josh/Cat + ani/cat.fbx";
    private const string catAnimsPath = "Assets/Team/Josh/Cat + ani/cat anims.fbx";
    private const string controllerPath = "Assets/Team/Ryan/Animations/CatAnimator.controller";

    // how much bigger the cat is than the model Josh exported (1 = normal size)
    // change this and run the setup again to resize the cat
    private const float catScale = 1.5f;

    [MenuItem("Tools/Ryan/Setup Cat Player")]
    public static void SetupCat()
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Cat Setup: couldn't find a PlayerController in the scene");
            return;
        }

        GameObject catPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(catModelPath);
        if (catPrefab == null)
        {
            Debug.LogError("Cat Setup: couldn't find the cat model at " + catModelPath);
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(player.gameObject, "Setup Cat Player");

        // hide the capsule but keep the player object and its scripts
        MeshRenderer capsuleRenderer = player.GetComponent<MeshRenderer>();
        if (capsuleRenderer != null)
        {
            capsuleRenderer.enabled = false;
        }

        // the CharacterController already handles collisions, so the old capsule collider isnt needed
        CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }

        // remove an old cat model if this has been run before
        Transform oldModel = player.transform.Find("CatModel");
        if (oldModel != null)
        {
            Undo.DestroyObjectImmediate(oldModel.gameObject);
        }

        // add the cat model as a child of the player
        GameObject cat = (GameObject)PrefabUtility.InstantiatePrefab(catPrefab, player.transform);
        Undo.RegisterCreatedObjectUndo(cat, "Add Cat Model");
        cat.name = "CatModel";
        cat.transform.localPosition = Vector3.zero;
        cat.transform.localScale = catPrefab.transform.localScale * catScale;

        // the fbx has a spare sphere in it, hide it
        Transform sphere = cat.transform.Find("Icosphere");
        if (sphere != null)
        {
            sphere.gameObject.SetActive(false);
        }

        TurnCatToFaceForward(player.transform, cat);
        Bounds catBounds = FitControllerToCat(player, cat);
        PlaceMouth(player, catBounds);
        SetupAnimator(player, cat);

        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        EditorSceneManager.SaveScene(player.gameObject.scene);
        Selection.activeGameObject = player.gameObject;

        Debug.Log("Cat Setup: done! cat size is " + catBounds.size);
    }

    // the player moves along its blue (forward) axis, so the cats head needs to point that way
    private static void TurnCatToFaceForward(Transform player, GameObject cat)
    {
        Transform head = FindChild(cat.transform, "head");
        Transform pelvis = FindChild(cat.transform, "pelvis");
        if (head == null || pelvis == null)
        {
            Debug.LogWarning("Cat Setup: couldn't find head/pelvis bones, rotate the cat by hand");
            return;
        }

        Vector3 catDirection = player.InverseTransformDirection(head.position - pelvis.position);
        catDirection.y = 0f;

        float angle = Vector3.SignedAngle(catDirection, Vector3.forward, Vector3.up);
        cat.transform.localRotation = Quaternion.Euler(0f, angle, 0f) * cat.transform.localRotation;
    }

    // shrink the CharacterController so it wraps the cat instead of a person sized capsule
    private static Bounds FitControllerToCat(PlayerController player, GameObject cat)
    {
        Bounds bounds = GetLocalBounds(player.transform, cat);

        // put the cats feet at the bottom of the player object
        cat.transform.localPosition = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z);
        bounds = GetLocalBounds(player.transform, cat);

        CharacterController controller = player.GetComponent<CharacterController>();
        Undo.RecordObject(controller, "Fit Controller");

        float radius = Mathf.Min(bounds.size.x, bounds.size.z) * 0.5f;
        float height = Mathf.Max(bounds.size.y, radius * 2f);

        controller.radius = radius;
        controller.height = height;
        controller.center = new Vector3(0f, height * 0.5f, 0f);

        // the default skin width (0.08) is bigger than the cat, which makes it hover
        // unity recommends about 10% of the radius
        controller.skinWidth = radius * 0.1f;
        controller.stepOffset = height * 0.3f;

        // the controller always floats by its skin width, so drop the model down to touch the floor
        cat.transform.localPosition -= new Vector3(0f, controller.skinWidth, 0f);

        return bounds;
    }

    // put the Mouth object at the front of the cats head, where ObjectHold holds the toys
    private static void PlaceMouth(PlayerController player, Bounds catBounds)
    {
        ObjectHold hold = player.GetComponent<ObjectHold>();
        if (hold == null || hold.mouth == null)
        {
            Debug.LogWarning("Cat Setup: no ObjectHold mouth to move");
            return;
        }

        Undo.RecordObject(hold.mouth, "Move Mouth");
        hold.mouth.SetParent(player.transform);
        hold.mouth.localPosition = new Vector3(0f, catBounds.size.y * 0.75f, catBounds.max.z);
        hold.mouth.localRotation = Quaternion.identity;

        // pick up range is measured from the mouth, so keep it about cat sized
        Undo.RecordObject(hold, "Pickup Range");
        hold.pickupRange = Mathf.Max(catBounds.size.z * 0.5f, 0.5f);
    }

    private static void SetupAnimator(PlayerController player, GameObject cat)
    {
        AnimatorController controller = CreateAnimatorController();

        Animator animator = cat.GetComponent<Animator>();
        if (animator == null)
        {
            animator = Undo.AddComponent<Animator>(cat);
        }
        animator.runtimeAnimatorController = controller;
        // the PlayerController moves the cat, not the animations
        animator.applyRootMotion = false;

        CatAnimation catAnimation = player.GetComponent<CatAnimation>();
        if (catAnimation == null)
        {
            catAnimation = Undo.AddComponent<CatAnimation>(player.gameObject);
        }
        catAnimation.animator = animator;
    }

    private static AnimatorController CreateAnimatorController()
    {
        AssetDatabase.DeleteAsset(controllerPath);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("VerticalSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;

        // there is no idle animation yet, so idle is just the cat standing still
        AnimatorState idle = machine.AddState("Idle");
        AnimatorState walk = machine.AddState("Walk");
        AnimatorState jump = machine.AddState("Jump");
        AnimatorState fall = machine.AddState("Fall");
        machine.defaultState = idle;

        walk.motion = FindClip("Walk");
        jump.motion = FindClip("Jump");
        fall.motion = FindClip("Fall");

        // idle <-> walk
        AnimatorStateTransition toWalk = idle.AddTransition(walk);
        toWalk.hasExitTime = false;
        toWalk.duration = 0.1f;
        toWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");

        AnimatorStateTransition toIdle = walk.AddTransition(idle);
        toIdle.hasExitTime = false;
        toIdle.duration = 0.1f;
        toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

        // jump when leaving the ground going up
        AnimatorStateTransition toJump = machine.AddAnyStateTransition(jump);
        toJump.hasExitTime = false;
        toJump.duration = 0.05f;
        toJump.canTransitionToSelf = false;
        toJump.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsGrounded");
        toJump.AddCondition(AnimatorConditionMode.Greater, 0.5f, "VerticalSpeed");

        // fall once the jump starts coming down
        AnimatorStateTransition jumpToFall = jump.AddTransition(fall);
        jumpToFall.hasExitTime = false;
        jumpToFall.duration = 0.15f;
        jumpToFall.AddCondition(AnimatorConditionMode.Less, 0f, "VerticalSpeed");

        // or fall after walking off a ledge (-3 so tiny bumps dont count as falling)
        AnimatorStateTransition toFall = machine.AddAnyStateTransition(fall);
        toFall.hasExitTime = false;
        toFall.duration = 0.15f;
        toFall.canTransitionToSelf = false;
        toFall.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsGrounded");
        toFall.AddCondition(AnimatorConditionMode.Less, -3f, "VerticalSpeed");

        // land
        AnimatorStateTransition land = fall.AddTransition(idle);
        land.hasExitTime = false;
        land.duration = 0.1f;
        land.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");

        AssetDatabase.SaveAssets();
        return controller;
    }

    private static AnimationClip FindClip(string clipName)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(catAnimsPath);
        foreach (Object asset in assets)
        {
            AnimationClip clip = asset as AnimationClip;
            if (clip != null && clip.name == clipName)
            {
                return clip;
            }
        }

        Debug.LogWarning("Cat Setup: couldn't find the " + clipName + " animation");
        return null;
    }

    // works out how big the cat is, measured from the player
    private static Bounds GetLocalBounds(Transform player, GameObject cat)
    {
        Renderer[] renderers = cat.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds();
        bool first = true;

        foreach (Renderer renderer in renderers)
        {
            if (!renderer.gameObject.activeInHierarchy)
            {
                continue;
            }

            Bounds world = renderer.bounds;
            // check all 8 corners so rotation is handled
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = world.center + Vector3.Scale(world.extents, new Vector3(
                    (i & 1) == 0 ? -1 : 1,
                    (i & 2) == 0 ? -1 : 1,
                    (i & 4) == 0 ? -1 : 1));
                Vector3 local = player.InverseTransformPoint(corner);

                if (first)
                {
                    bounds = new Bounds(local, Vector3.zero);
                    first = false;
                }
                else
                {
                    bounds.Encapsulate(local);
                }
            }
        }

        return bounds;
    }

    private static Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                return child;
            }
        }
        return null;
    }
}
