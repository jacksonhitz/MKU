using System.Collections;
using AudioSystem;
using Cysharp.Threading.Tasks;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Dialogue : MonoBehaviour
{
    [ResetOnPlay]
    public static Dialogue Instance { get; set; }

    public TextMeshProUGUI textComponent;

    public string[] lines;
    public float textSpeed;
    private Moroutine dialogue;
    private bool completeNow;
    private SoundEmitter soundEmitter;

    int index = 0;

    void Awake()
    {
        Instance = this;
    }

    public async UniTask Play()
    {
        while (HasNextLine())
        {
            dialogue = Moroutine.Run(TypeLine(NextLine())).SetOwner(this);
            await dialogue.WaitForComplete();
        }
    }

    public void Complete()
    {
        if (dialogue?.IsCompleted ?? true)
            return;

        completeNow = true;
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

        soundEmitter = SoundManager.Instance.CreateSoundBuilder().Play("Line");

        foreach (char c in text)
        {
            if (completeNow)
            {
                textComponent.text = text;
                completeNow = false;
                if (soundEmitter != null && soundEmitter.isActiveAndEnabled)
                    soundEmitter.Stop();
                soundEmitter = null;
                yield return null;
                break;
            }
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(1f);

        if (soundEmitter != null && soundEmitter.isActiveAndEnabled)
            soundEmitter.Stop();
        soundEmitter = null;
    }

    public Moroutine TypeText(string customText)
    {
        StopAllCoroutines();
        dialogue = Moroutine.Run(TypeLine(customText)).SetOwner(this);
        return dialogue;
    }

    public void Off()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
        if (soundEmitter != null && soundEmitter.isActiveAndEnabled)
            soundEmitter.Stop();
        soundEmitter = null;
    }
}
