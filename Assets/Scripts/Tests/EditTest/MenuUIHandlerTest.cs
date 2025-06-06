using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.TestTools;
using System.Collections;

public class MenuUIHandlerTest
{
    private GameObject handlerObject;
    private MenuUIHandler handler;

    [SetUp]
    public void Setup()
    {
        handlerObject = new GameObject();
        handler = handlerObject.AddComponent<MenuUIHandler>();

        // Приватные поля через сериализацию
        SetPrivateField("nameInpput", CreateInputField("TestUser"));
        SetPrivateField("passwordInpput", CreateInputField("Test123!"));
        SetPrivateField("messegeText", CreateTextMesh(""));

        // Обход вызова Unity Services
        SetPrivateField("messegePage", new GameObject());
        SetPrivateField("writeNamePage", new GameObject());
        SetPrivateField("levelPage", new GameObject());
    }

    private TMP_InputField CreateInputField(string text)
    {
        var obj = new GameObject();
        var input = obj.AddComponent<TMP_InputField>();
        var textComponent = obj.AddComponent<TextMeshProUGUI>();
        input.textComponent = textComponent;
        input.text = text;
        return input;
    }

    private TextMeshProUGUI CreateTextMesh(string initial)
    {
        var obj = new GameObject();
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = initial;
        return tmp;
    }

    private void SetPrivateField(string fieldName, object value)
    {
        var field = typeof(MenuUIHandler).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(handler, value);
    }

    [Test]
    public void isPasswordValid_WithValidInput_StartsCoroutine()
    {
        SetPrivateField("isContinue", false);
        GameManager.Instance = new GameManager(); // Внедрение заглушки

        Assert.IsTrue(handler.isPasswordValid("Test123!"));
    }

    [Test]
    public void CheckNameAndPassword_WithInvalidPassword_ShowsMessage()
    {
        SetPrivateField("isContinue", false);
        var nameInput = CreateInputField("ValidName");
        var passwordInput = CreateInputField("short");
        var msg = CreateTextMesh("");

        SetPrivateField("nameInpput", nameInput);
        SetPrivateField("passwordInpput", passwordInput);
        SetPrivateField("messegeText", msg);

        handler.CheckNameAndPassword();

        Assert.IsTrue(msg.text.Contains("Имя пользователя"));
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(handlerObject);
    }
}
