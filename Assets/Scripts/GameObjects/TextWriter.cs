using System.Collections;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{
    private TextMeshPro text = new TextMeshPro();

    [Header("Настройки текста")]
    [SerializeField] private string message;

    [Header("Тайминги")]
    [SerializeField] private float letterDelay = 0.05f; // Задержка между буквами
    [SerializeField] private float displayDuration = 2f; // Время показа полного текста

    private Coroutine currentAnimation;
    void Start()
    {
        if(GetComponent<TextMeshPro>() != null)
            text = GetComponent<TextMeshPro>();
        text.text = "";
    }

    virtual public void ShowText()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(TypewriterAnimation());
    }

    private IEnumerator TypewriterAnimation()
    {
        // Постепенное появление букв
        for (int i = 0; i <= message.Length; i++)
        {
            text.text = message.Substring(0, i);
            yield return new WaitForSeconds(letterDelay);
        }

        // Ожидание перед исчезновением
        yield return new WaitForSeconds(displayDuration);

        // Постепенное исчезновение букв
        for (int i = 0; i < message.Length; i++)
        {
            Debug.Log(i);
            Debug.Log(message[i]);
            text.text = message.Substring(i);
            yield return new WaitForSeconds(letterDelay);
        }
        text.text = "";
    }
}
