using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance { get; set; }

    public TextMeshProUGUI textComponent;

    public string[] lines;
    public float textSpeed;

    int index = 0;

    void Awake()
    {
        Instance = this;
    }

    public async UniTask Play()
    {
        while (HasNextLine())
        {
            await Moroutine.Run(TypeLine(NextLine())).WaitForComplete();
        }
    }

    private string NextLine()
    {
        Assert.IsTrue(HasNextLine());
        return lines[index++];
    }

    private bool HasNextLine()
    {
        return index < lines.Length;
    }

    IEnumerator TypeLine(string text)
    {
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(0.1f);

        SoundManager.Instance.Play("Line");

        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        // TODO: Move this timer to the play loop
        yield return new WaitForSeconds(1f);

        SoundManager.Instance.Stop();
    }

    public Moroutine TypeText(string customText)
    {
        StopAllCoroutines();
        return Moroutine.Run(TypeLine(customText));
    }

    public void Off()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
    }
}
