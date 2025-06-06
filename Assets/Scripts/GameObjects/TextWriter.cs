using System.Collections;
using TMPro;
using UnityEngine;

public class TextWriter : MonoBehaviour
{
    private TextMeshPro text = new TextMeshPro();

    [Header("��������� ������")]
    [SerializeField] private string message;

    [Header("��������")]
    [SerializeField] private float letterDelay = 0.05f; // �������� ����� �������
    [SerializeField] private float displayDuration = 2f; // ����� ������ ������� ������

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
        // ����������� ��������� ����
        for (int i = 0; i <= message.Length; i++)
        {
            text.text = message.Substring(0, i);
            yield return new WaitForSeconds(letterDelay);
        }

        // �������� ����� �������������
        yield return new WaitForSeconds(displayDuration);

        // ����������� ������������ ����
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
