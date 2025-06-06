using NUnit.Framework;
using UnityEngine;

public class CameraFollowTest
{
    private GameObject cameraObject;
    private GameObject targetObject;
    private CameraFollow cameraFollow;

    [SetUp]
    public void Setup()
    {
        targetObject = new GameObject("Target");
        targetObject.transform.position = new Vector3(5f, 3f, 0f);

        cameraObject = new GameObject("Camera");
        cameraFollow = cameraObject.AddComponent<CameraFollow>();

        var targetTransform = targetObject.transform;
        cameraFollow.GetType()
            .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cameraFollow, targetTransform);

        // ��������� ����������
        cameraFollow.GetType().GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cameraFollow, 100f); // ������� �������� ��� ����� ����������� ����������

        cameraFollow.GetType().GetField("yOffset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cameraFollow, 1f);
        cameraFollow.GetType().GetField("xMin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cameraFollow, -10f);
        cameraFollow.GetType().GetField("xMax", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cameraFollow, 10f);
    }

    [Test]
    public void CameraFollowsTargetWithinLimits()
    {
        // �������������� ��������� ������
        cameraObject.transform.position = Vector3.zero;

        // �������� ���������� �������
        cameraFollow.GetType().GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(cameraFollow, null);

        Vector3 cameraPos = cameraObject.transform.position;

        // ������ ������ ��������� �� target �� X � Y + yOffset, Z = -10
        Assert.AreEqual(5f, Mathf.Round(cameraPos.x), 0.1f, "������ ������ ��������� �� X");
        Assert.AreEqual(4f, Mathf.Round(cameraPos.y), 0.1f, "������ ������ ��������� yOffset");
        Assert.AreEqual(-10f, cameraPos.z, "Z-���������� ������ ������ ���� -10");
    }

    [Test]
    public void CameraXClampedByMinAndMax()
    {
        // ��������� target �� ������� ������
        targetObject.transform.position = new Vector3(1000f, 2f, 0f);

        // �������� ���������� �������
        cameraFollow.GetType().GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(cameraFollow, null);

        Vector3 cameraPos = cameraObject.transform.position;

        // ��������, ��� X ��������� xMax
        Assert.LessOrEqual(cameraPos.x, 10f + 0.5f); // � ������� �� ������������
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(cameraObject);
        Object.DestroyImmediate(targetObject);
    }
}
