#if UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY_5_OR_NEWER
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_OR_NEWER
using UnityEngine.Networking;
#endif

public class CrossLinkInstance : MonoBehaviour
{
    public static CrossLinkInstance instance;

    public bool DontDestroy = false;

    public string gameID = "331";
#if !UNITY_IOS
    public string providerID = "629";
#else
    public string providerID = "931";
#endif
    private string deviceID;
    private string jsonURL;
    private string statURL;

    private string jsonFile;
    public List<object> promoList;
    private Dictionary<string, object>[] promoData;

    private List<string> xmlLinks = new List<string>();
    private List<string> pngLinks = new List<string>();
    private List<string> hash = new List<string>();
    public List<string> targetLinks = new List<string>();
    public List<string> bannerIDs = new List<string>();
    public List<int> impressions = new List<int>();

    public int bannerNum = 0;
    public int bannerNumСurrent = 0;

    public List<List<Sprite>> bannersFrames = new List<List<Sprite>>();
    public List<AnimationParams> animationParams = new List<AnimationParams>();

    public void Initialize()
    {
        if (!instance)
        {
            DontDestroyOnLoad(this);

            deviceID = SystemInfo.deviceUniqueIdentifier;
            jsonURL = "http://adv.herocraft.com/external/api/crosspromo/get?" + "game_id=" + gameID + "&" + "provider_id=" + providerID + "&" + "device_id=" + deviceID;
            LeanLoader.load(jsonURL, new LLOptions().setOnLoad(GetJSON).setUseCacheAsBackup(true));
        }
    }

    private void GetJSON(string json)
    {
        if (string.IsNullOrEmpty(json) || !(json.Contains("id") && json.Contains("atlas") && json.Contains("xml")))
        {
            return;
        }
        jsonFile = json;
        promoList = MiniJSON.JsonClass.Deserialize(jsonFile) as List<object>;
        StartCoroutine(DataProcessor(promoList));
    }

    private IEnumerator DataProcessor(List<object> promoList)
    {
        if (!instance)
            instance = this;

        bool useCache = false;

        if (promoList.Count > 0)
        {
            promoData = new Dictionary<string, object>[promoList.Count];

            for (int i = 0; i < promoList.Count; i++)
            {
                useCache = false;
                promoData[i] = promoList[i] as Dictionary<string, object>;
                if (promoData[i].Count > 0)
                {
                    xmlLinks.Add(promoData[i]["xml"].ToString());
                    pngLinks.Add(promoData[i]["atlas"].ToString());
                    hash.Add(promoData[i]["atlas_hash"].ToString());
                    targetLinks.Add(promoData[i]["target"].ToString());
                    bannerIDs.Add(promoData[i]["id"].ToString());
                    impressions.Add(PlayerPrefs.GetInt("impressions_" + bannerIDs[i], 0));
                    if (impressions[i] > 0)
                        StartCoroutine(SendStatistic(i, "impressions", impressions[i]));

                    if (PlayerPrefs.GetString("jsonFile").Equals(jsonFile))
                    {
                        useCache = true;
                    }
                    else if (!PlayerPrefs.GetString("jsonFile").Equals(""))
                    {
                        List<object> promoListOld = MiniJSON.JsonClass.Deserialize(PlayerPrefs.GetString("jsonFile", "")) as List<object>;

                        for (int j = 0; j < promoListOld.Count; j++)
                        {
                            if ((promoListOld[j] as Dictionary<string, object>)["atlas_hash"].Equals(hash[hash.Count - 1]))
                                useCache = true;
                        }
                    }

                    Hashtable onLoadParam = new Hashtable();
                    onLoadParam.Add("item", i);
                    onLoadParam.Add("cached", useCache);
                    onLoadParam.Add("xmlLink", xmlLinks[i]);

                    if (!useCache)
                        LeanLoader.load(xmlLinks[i], new LLOptions().setOnLoad(getFiles).setOnLoadParam(onLoadParam).saveInCache(true));
                    else
                        LeanLoader.load(xmlLinks[i], new LLOptions().setOnLoad(getFiles).setOnLoadParam(onLoadParam).setUseCache(useCache));
                }
                yield return new WaitForSeconds(1.0f);
            }
            PlayerPrefs.SetString("jsonFile", jsonFile);
        }
    }

    private void getFiles(string xml, Dictionary<string, object> hash)
    {
        //int item = System.Convert.ToInt32(hash["item"]);
        bool useCache = System.Convert.ToBoolean(hash["cached"]);

        animationParams.Add(new AnimationParams());

        int item = animationParams.Count - 1;

        animationParams[item].SetAnimationParams(xml);

        Hashtable onLoadParam = new Hashtable();
        onLoadParam.Add("item", item);
        if (!useCache)
            LeanLoader.load(pngLinks[item], new LLOptions().setOnLoad(CreateAnimations).setOnLoadParam(onLoadParam).saveInCache(true));
        else
            LeanLoader.load(pngLinks[item], new LLOptions().setOnLoad(CreateAnimations).setOnLoadParam(onLoadParam).setUseCache(useCache));
    }

    private void CreateAnimations(Texture2D tex, Dictionary<string, object> hash)
    {
        //int item = System.Convert.ToInt32(hash["item"]);
        if (tex != null)
        {
            bannersFrames.Add(new List<Sprite>());

            int item = bannersFrames.Count - 1;

            for (int i = 1; i <= animationParams[item].framesY; i++)
            {
                for (int j = 0; j < animationParams[item].framesX; j++)
                {
                    bannersFrames[item].Add(Sprite.Create(tex, new Rect(j * animationParams[item].width, tex.height - i * animationParams[item].height, animationParams[item].width, animationParams[item].height), new Vector2(0.5f, 0.5f)));
                }
            }
        }
    }

    public IEnumerator SendStatistic(int num, string param, int count = 0)
    {
        WWWForm formData = new WWWForm();
        statURL = "http://adv.herocraft.com/external/api/crosspromo/stats?" /*+ "banner_id=" + bannerIDs[num] + "&game_id=" + gameID + "&impressions=" + 1 + "&clicks=" + 1 + "&device_id=" + deviceID*/;
        formData.AddField("banner_id", instance.bannerIDs[num]);
        formData.AddField("game_id", gameID);
        if (param.Equals("impressions"))
            formData.AddField("impressions", count);
        else if (param.Equals("clicks"))
        {
            formData.AddField("clicks", count);
            formData.AddField("impressions", instance.impressions[num]);
        }
        formData.AddField("device_id", instance.deviceID);
#if UNITY_5_OR_NEWER
        UnityWebRequest www = UnityWebRequest.Post(statURL, formData);
#else
        WWW www = new WWW(statURL, formData.data);
#endif
        if (Debug.isDebugBuild)
            Debug.Log(www.url + System.Text.Encoding.UTF8.GetString(formData.data));
#if UNITY_5_OR_NEWER
        yield return www.Send();
#else
        yield return www;
#endif
        if (www.isDone)
        {
            instance.impressions[num] = 0;
            PlayerPrefs.SetInt("impressions_" + instance.bannerIDs[num], instance.impressions[num]);

            if (Debug.isDebugBuild)
                Debug.Log("Done");
        }
        else
        {
            if (Debug.isDebugBuild)
                Debug.Log(www.error);
        }
    }
}

[System.Serializable]
public class AnimationParams
{
    private XMLParser xmlParser = new XMLParser();
    private XMLNode xmlNode;

    public float width;
    public float height;
    public int framesX;
    public int framesY;
    public float speed;
    public int loops;
    public bool stopAnimationa;

    public void SetAnimationParams(string xml)
    {
        string data = xml;
        xmlNode = xmlParser.Parse(data);
        width = float.Parse(xmlNode.GetValue("animfile>0>width>0>_text"));
        height = float.Parse(xmlNode.GetValue("animfile>0>height>0>_text"));
        framesX = int.Parse(xmlNode.GetValue("animfile>0>framesX>0>_text"));
        framesY = int.Parse(xmlNode.GetValue("animfile>0>framesY>0>_text"));
        speed = float.Parse(xmlNode.GetValue("animfile>0>speed>0>_text"));
        loops = int.Parse(xmlNode.GetValue("animfile>0>loop>0>_text"));
    }
}