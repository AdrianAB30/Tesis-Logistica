using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

public class TestScripts
{
    private System.Type GetTypeByName(string name)
    {
        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(name);
            if (t != null) return t;
        }
        return null;
    }

    private Component CreateInstanceAndAddComponent(string typeFullName, string goName)
    {
        var t = GetTypeByName(typeFullName);
        Assert.IsNotNull(t, $"Type {typeFullName} not found in assemblies");
        var go = new GameObject(goName);
        return go.AddComponent(t);
    }

    [Test]
    public void Player_SetCanMove_Reflection()
    {
        var pType = GetTypeByName("PlayerController");
        Assert.IsNotNull(pType, "PlayerController not found");

        var go = new GameObject("player");
        var player = go.AddComponent(pType);

        var setCanMove = pType.GetMethod("SetCanMove", BindingFlags.Public | BindingFlags.Instance);
        var canMoveField = pType.GetField("canMove", BindingFlags.Public | BindingFlags.Instance);

        Assert.IsNotNull(setCanMove);
        Assert.IsNotNull(canMoveField);

        setCanMove.Invoke(player, new object[] { true });
        Assert.IsTrue((bool)canMoveField.GetValue(player));

        setCanMove.Invoke(player, new object[] { false });
        Assert.IsFalse((bool)canMoveField.GetValue(player));

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Player_OnMovement_Reflection()
    {
        var pType = GetTypeByName("PlayerController");
        Assert.IsNotNull(pType);

        var go = new GameObject("player");
        var player = go.AddComponent(pType);

        var setCanMove = pType.GetMethod("SetCanMove", BindingFlags.Public | BindingFlags.Instance);
        var onMovement = pType.GetMethod("OnMovement", BindingFlags.Public | BindingFlags.Instance);
        var movementField = pType.GetField("movement", BindingFlags.NonPublic | BindingFlags.Instance);

        setCanMove.Invoke(player, new object[] { true });
        onMovement.Invoke(player, new object[] { new Vector2(1, 0) });

        var movement = (Vector2)movementField.GetValue(player);
        Assert.AreEqual(new Vector2(1, 0), movement);

        setCanMove.Invoke(player, new object[] { false });
        onMovement.Invoke(player, new object[] { new Vector2(1, 1) });

        movement = (Vector2)movementField.GetValue(player);
        Assert.AreEqual(Vector2.zero, movement);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Player_OnLook_Reflection()
    {
        var pType = GetTypeByName("PlayerController");
        Assert.IsNotNull(pType);

        var go = new GameObject("player");
        var player = go.AddComponent(pType);

        var onLook = pType.GetMethod("OnLook", BindingFlags.Public | BindingFlags.Instance);
        var lookInputField = pType.GetField("lookInput", BindingFlags.NonPublic | BindingFlags.Instance);

        onLook.Invoke(player, new object[] { new Vector2(5, -3) });
        var lookInput = (Vector2)lookInputField.GetValue(player);

        Assert.AreEqual(new Vector2(5, -3), lookInput);

        Object.DestroyImmediate(go);
    }

    [Test]
    public IEnumerator Player_OnInteract_Reflection()
    {
        var pType = GetTypeByName("PlayerController");
        Assert.IsNotNull(pType);

        var go = new GameObject("player");
        var player = go.AddComponent(pType);

        var eyesField = pType.GetField("eyes", BindingFlags.NonPublic | BindingFlags.Instance);
        var holdPosField = pType.GetField("holdPosition", BindingFlags.NonPublic | BindingFlags.Instance);
        var objectHeldField = pType.GetField("objectHeld", BindingFlags.NonPublic | BindingFlags.Instance);
        var onInteract = pType.GetMethod("OnInteract", BindingFlags.Public | BindingFlags.Instance);

        eyesField.SetValue(player, go.transform);
        holdPosField.SetValue(player, new GameObject("Hold").transform);

        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = go.transform.position + Vector3.forward * 1.5f;
        obj.AddComponent<Rigidbody>();

        onInteract.Invoke(player, new object[] { true });
        yield return null;

        var heldObj = objectHeldField.GetValue(player) as GameObject;
        Assert.IsNotNull(heldObj);

        onInteract.Invoke(player, new object[] { true });
        yield return null;

        heldObj = objectHeldField.GetValue(player) as GameObject;
        Assert.IsNull(heldObj);

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(go);
    }
    [Test]
    public void Player_HandleLookPlayer_Reflection()
    {
        var pType = GetTypeByName("PlayerController");
        Assert.IsNotNull(pType, "PlayerController not found");

        var go = new GameObject("player");
        var player = go.AddComponent(pType);

        var cameraGO = new GameObject("Camera");
        var cameraTransform = cameraGO.transform;

        var cameraField = pType.GetField("cameraTransform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        cameraField.SetValue(player, cameraTransform);

        var lookInputField = pType.GetField("lookInput", BindingFlags.NonPublic | BindingFlags.Instance);
        var sensitivityField = pType.GetField("mouseSensitivity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var xRotationField = pType.GetField("xRotation", BindingFlags.NonPublic | BindingFlags.Instance);

        lookInputField.SetValue(player, new Vector2(10f, 5f));
        sensitivityField.SetValue(player, 2f);
        xRotationField.SetValue(player, 0f);

        var method = pType.GetMethod("HandleLookPlayer", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(method, "HandleLookPlayer method not found");

        float initialPlayerY = go.transform.rotation.eulerAngles.y;
        float initialCameraX = cameraTransform.localRotation.eulerAngles.x;

        method.Invoke(player, null);

        float finalPlayerY = go.transform.rotation.eulerAngles.y;
        float finalCameraX = cameraTransform.localRotation.eulerAngles.x;

        Assert.AreNotEqual(initialPlayerY, finalPlayerY, "El jugador no rotó en Y");
        Assert.AreNotEqual(initialCameraX, finalCameraX, "La cámara no rotó en X");

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(cameraGO);
    }
}
