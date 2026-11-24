using FishSlop;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// sets up toys to float nicely with Avery's ObjectFloat script (doesnt change her script at all)
// select the toys in the scene, then go to Tools > Ryan > Make Selected Toys Float
public class FloatingToySetup
{
    // how much of the toy sits under the water (0.35 = about a third under, the rest pokes out)
    private const float amountUnderwater = 0.35f;

    [MenuItem("Tools/Ryan/Make Selected Toys Float")]
    public static void MakeSelectedToysFloat()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length == 0)
        {
            Debug.LogWarning("Float Setup: select the toys in the scene first");
            return;
        }

        foreach (GameObject toy in selected)
        {
            SetupToy(toy);
        }

        EditorSceneManager.MarkSceneDirty(selected[0].scene);
    }

    private static void SetupToy(GameObject toy)
    {
        Rigidbody rb = toy.GetComponent<Rigidbody>();
        Collider col = toy.GetComponentInChildren<Collider>();
        if (rb == null || col == null)
        {
            Debug.LogWarning("Float Setup: " + toy.name + " needs a Rigidbody and a Collider");
            return;
        }

        // the fish, spring and mouse models come in rotated -90 on X from Blender,
        // so ObjectFloat's "turn upright" torque flips them onto their side and they sink.
        // freezing rotation stops the flip, and they still bob up and down with the waves
        Undo.RecordObject(rb, "Float Setup");
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        ObjectFloat floater = toy.GetComponent<ObjectFloat>();
        if (floater == null)
        {
            floater = Undo.AddComponent<ObjectFloat>(toy);
        }

        // ObjectFloat settles with the bottom of the toy restDepth below the water.
        // the default 0.5 is taller than these toys, so they sat fully underwater
        float toyHeight = col.bounds.size.y;
        float restDepth = toyHeight * amountUnderwater;

        // restDepth and hullCollider are private on ObjectFloat, so set them like the Inspector does
        SerializedObject settings = new SerializedObject(floater);
        settings.FindProperty("restDepth").floatValue = restDepth;
        settings.FindProperty("hullCollider").objectReferenceValue = col;
        settings.ApplyModifiedProperties();

        Debug.Log("Float Setup: " + toy.name + " is " + toyHeight.ToString("0.00") + " tall, restDepth set to " + restDepth.ToString("0.00"));
    }
}
