#if UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY_5_OR_NEWER
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class CrossLink : MonoBehaviour
{
    public static CrossLink instance;

    public GameObject Border;
    public GameObject BannerTemplate;

    private new GameObject animation;

    public static bool needShow = false;
    public static bool confirmationActive = false;

    public GameObject PrefabConfirmationWindow;
    public bool ShowConfirmation = true;
    public LocalizeParams localizeParams;

    public static void Init(int gameID, int providerID)
    {
        if (!CrossLinkInstance.instance)
        {
            GameObject obj = new GameObject();
            obj.name = "~CrossLinkInstance";
            obj.AddComponent<CrossLinkInstance>();
            obj.GetComponent<CrossLinkInstance>().gameID = gameID.ToString();
            obj.GetComponent<CrossLinkInstance>().providerID = providerID.ToString();
            obj.GetComponent<CrossLinkInstance>().Initialize();
        }
    }

    void Awake()
    {
        instance = this;
        Border.SetActive(false);
#if UNITY_5_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }

#if UNITY_5_OR_NEWER
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (animation)
        {
            Hide();
        }
    }
#endif

    void Update()
    {
        if (CrossLinkInstance.instance)
            if (!animation && CrossLinkInstance.instance.animationParams.Count > 0 && CrossLinkInstance.instance.bannersFrames.Count > 0)
            {
                animation = Instantiate(BannerTemplate, BannerTemplate.transform.position, BannerTemplate.transform.rotation) as GameObject;
                Destroy(BannerTemplate);
                animation.GetComponent<Image>().enabled = true;
                animation.gameObject.name = "Banner";

                animation.GetComponent<RectTransform>().SetParent(Border.gameObject.transform);
                animation.GetComponent<RectTransform>().sizeDelta = new Vector2(CrossLinkInstance.instance.animationParams[0].width, CrossLinkInstance.instance.animationParams[0].height);
                animation.GetComponent<RectTransform>().sizeDelta -= 0.15f * animation.GetComponent<RectTransform>().sizeDelta;
                animation.GetComponent<Image>().sprite = CrossLinkInstance.instance.bannersFrames[0][0];
                animation.GetComponent<RectTransform>().SetParent(gameObject.transform);
                animation.GetComponent<RectTransform>().localScale = Border.GetComponent<RectTransform>().localScale;
                animation.SetActive(false);

                Border.GetComponent<RectTransform>().sizeDelta = new Vector2(CrossLinkInstance.instance.animationParams[0].width, CrossLinkInstance.instance.animationParams[0].height + CrossLinkInstance.instance.animationParams[0].height * 0.15f);
                Border.gameObject.transform.SetSiblingIndex(animation.transform.GetSiblingIndex() + 1);
            }

        if (needShow)
            Show();
    }

    private IEnumerator StartAnimation(int item)
    {
        int frame = 0;
        int loop = -1;
        int currentBunner = CrossLinkInstance.instance.bannerNumСurrent;

        while (loop < CrossLinkInstance.instance.animationParams[item].loops && currentBunner == CrossLinkInstance.instance.bannerNumСurrent)
        {
            yield return new WaitForSeconds(CrossLinkInstance.instance.animationParams[item].speed / 100);

            animation.GetComponent<Image>().sprite = CrossLinkInstance.instance.bannersFrames[item][frame];
            frame++;
            if (frame >= CrossLinkInstance.instance.bannersFrames[item].Count)
            {
                frame = 0;
                if (CrossLinkInstance.instance.animationParams[item].loops == 0)
                    loop++;
            }
        }
    }

    public static void Show()
    {
        needShow = true;

        if (CrossLinkInstance.instance && instance.animation)
        {
            needShow = false;

            if (CrossLinkInstance.instance.bannerNum >= CrossLinkInstance.instance.promoList.Count)
                CrossLinkInstance.instance.bannerNum = 0;

            CrossLinkInstance.instance.impressions[CrossLinkInstance.instance.bannerNum]++;
            PlayerPrefs.SetInt("impressions_" + CrossLinkInstance.instance.bannerIDs[CrossLinkInstance.instance.bannerNum], CrossLinkInstance.instance.impressions[CrossLinkInstance.instance.bannerNum]);
            if (CrossLinkInstance.instance.impressions[CrossLinkInstance.instance.bannerNum] >= 5)
                instance.StartCoroutine(CrossLinkInstance.instance.SendStatistic(CrossLinkInstance.instance.bannerNum, "impressions", CrossLinkInstance.instance.impressions[CrossLinkInstance.instance.bannerNum]));

            instance.animation.SetActive(true);
            instance.Border.SetActive(true);
            instance.GetComponent<EventSystem>().enabled = true;
            CrossLinkInstance.instance.bannerNumСurrent = CrossLinkInstance.instance.bannerNum;
            instance.StartCoroutine(instance.StartAnimation(CrossLinkInstance.instance.bannerNum));
            CrossLinkInstance.instance.bannerNum++;
        }
    }

    public static void Hide()
    {
        if (CrossLinkInstance.instance && instance)
        {
            instance.Border.SetActive(false);
            instance.GetComponent<EventSystem>().enabled = false;
            if (CrossLinkInstance.instance.bannerNum >= CrossLinkInstance.instance.bannersFrames.Count)
                CrossLinkInstance.instance.bannerNum = 0;
            CrossLinkInstance.instance.bannerNumСurrent = -1;
            if (instance.animation)
            {
                instance.animation.SetActive(false);
                if (CrossLinkInstance.instance.bannersFrames.Count > 0 &&
                    CrossLinkInstance.instance.bannersFrames[CrossLinkInstance.instance.bannerNum].Count > 0 &&
                    CrossLinkInstance.instance.bannersFrames[CrossLinkInstance.instance.bannerNum] != null &&
                    CrossLinkInstance.instance.bannersFrames[CrossLinkInstance.instance.bannerNum][0] != null)
                {
                    instance.animation.GetComponent<Image>().sprite = CrossLinkInstance.instance.bannersFrames[CrossLinkInstance.instance.bannerNum][0];
                }
            }
        }
    }

    public void OnBannerClick()
    {
        if (ShowConfirmation)
        {
            MessageBox mb = MessageBox.Show
            (
                PrefabConfirmationWindow,
                "open_page",
                null,
                (result) => { WindowFunc(result.ToString()); },
                MessageBoxButtons.YesNo
            );
            mb.gameObject.transform.SetParent(gameObject.transform);
            mb.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
            mb.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            mb.gameObject.transform.localScale = Vector3.one;

            confirmationActive = true;
        }
        else
        if (CrossLinkInstance.instance.targetLinks != null)
        {
            StartCoroutine(CrossLinkInstance.instance.SendStatistic(CrossLinkInstance.instance.bannerNumСurrent, "clicks", 1));
            if (CrossLinkInstance.instance.targetLinks[CrossLinkInstance.instance.bannerNumСurrent] != null)
            {
                Application.OpenURL(CrossLinkInstance.instance.targetLinks[CrossLinkInstance.instance.bannerNumСurrent]);
                Show();
            }
        }
    }

    void WindowFunc(string result)
    {
        if (result.Equals("Yes"))
        {
            if (CrossLinkInstance.instance.targetLinks != null)
            {
                StartCoroutine(CrossLinkInstance.instance.SendStatistic(CrossLinkInstance.instance.bannerNumСurrent, "clicks", 1));
                if (CrossLinkInstance.instance.targetLinks[CrossLinkInstance.instance.bannerNumСurrent] != null)
                {
                    Application.OpenURL(CrossLinkInstance.instance.targetLinks[CrossLinkInstance.instance.bannerNumСurrent]);
                    Show();
                }
            }
        }
        confirmationActive = false;
    }
}

[System.Serializable]
public class LocalizeParams
{
    public List<string> Languages;
    public List<string> YesButton;
    public List<string> NoButton;
    public List<string> OpenPageText;

    public string Localize(string id)
    {
        for (int i = 0; i < Languages.Count; i++)
        {
            if (Application.systemLanguage.ToString().Equals(Languages[i]))
            {
                if (id.Equals("yes"))
                    return YesButton[i];
                else if (id.Equals("no"))
                    return NoButton[i];
                else if (id.Equals("open_page"))
                    return OpenPageText[i];
            }
        }
        if (id.Equals("yes"))
            return YesButton[0];
        else if (id.Equals("no"))
            return NoButton[0];
        else if (id.Equals("open_page"))
            return OpenPageText[0];
        else
            return id;
    }
}