using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class mysteriousman : MonoBehaviour, IInteraction
{
    private SpriteRenderer _render;
    private BoxCollider2D _col;
    public Image Artist;
    public Image FadeImg;
    public Image EndImg;
    private float _fadeTime = 1f;

    private void Awake()
    {
        _render = GetComponentInChildren<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!SaveManager.Instance.UserData.QuestInfo.ContainsKey(21))
        {
            DialogManager.Instance.ChangeField += apearNPC;

            _render.enabled = false;
            _col.enabled = false;
            return;
        }
        if (SaveManager.Instance.UserData.QuestInfo[21] == Condition.QuestCompleted) return;

        DialogManager.Instance.ChangeField += apearNPC;

        _render.enabled = false;
        _col.enabled = false;
    }

    private void apearNPC()
    {
        _render.enabled = true;
        _col.enabled = true;
    }

    public void Interact()
    {
        // 엔딩씬 여기서 구현 예정
        StartCoroutine(EndingStart());
    }

    private IEnumerator EndingStart()
    {
        SoundManager.Instance.SilenceMixer(-80f);
        yield return StartCoroutine(Fade(FadeImg,0.0f, 1.0f));

        GameManager.Instance.Player._actionStateMachine.InteractOn();
        // 들어와야 되는거 엔딩 컷씬
        Show_Tutorial.Show_CutScene("Video/Ending", 38f);

        yield return new WaitForSeconds(38f);


        yield return StartCoroutine(Fade(EndImg,0.0f, 1.0f));

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(Fade(EndImg,1.0f, 0.0f));

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(Fade(Artist, 0.0f, 1.0f));

        yield return new WaitForSeconds(1f);

        DialogManager.Instance.PlayDialogNode(DataManager.Instance.DialogNodes[2542]);
    }


    private IEnumerator Fade(Image img, float start, float End)
    {
        float CurrentTime = 0.0f;

        Image Mat = img;
        Color Color = Mat.color;

        // 페이드 인/아웃 
        while (CurrentTime < _fadeTime)
        {
            CurrentTime += Time.deltaTime;
            float t = CurrentTime / _fadeTime;

            Color.a = Mathf.Lerp(start, End, t);

            Mat.color = Color;
            yield return null;
        }

        // 원본 색상으로 되돌리기 
        Color.a = End;
        Mat.color = Color;
    }

    private IEnumerator FadeEnd(float start, float End)
    {
        float CurrentTime = 0.0f;

        Image Mat = EndImg;
        Color Color = Mat.color;

        // 페이드 인/아웃 
        while (CurrentTime < _fadeTime)
        {
            CurrentTime += Time.deltaTime;
            float t = CurrentTime / _fadeTime;

            Color.a = Mathf.Lerp(start, End, t);

            Mat.color = Color;
            yield return null;
        }

        // 원본 색상으로 되돌리기 
        Color.a = End;
        Mat.color = Color;
    }
}
