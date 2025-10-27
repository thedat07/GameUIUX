using UnityEngine;
using TMPro;
using Lean.Pool;
using BrunoMikoski.AnimationSequencer;

public class ViewInfoWarring : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_txt;
    [SerializeField] AnimationSequencerController m_Anim;
    [SerializeField] CanvasGroup m_Canvas;

    void Start()
    {
        m_Anim.OnFinishedEvent.AddListener(Despawn);
    }

    public void Init(string log)
    {
        Setting();
        m_txt.text = string.Format("{0}", log);
        m_Anim.Play();
    }

    void Setting()
    {
        m_Canvas.alpha = 1;
        transform.localScale = Vector3.one;
    }

    public void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }
}
