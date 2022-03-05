using UnityEngine;
using UnityEngine.UI;

public class CrossLinkExample : MonoBehaviour {

    public Button ShowBanner;
    public Button HideBanner;

	void Start ()
    {
#if !UNITY_IOS
        CrossLink.Init(331, 629);
#else
        CrossLink.Init(331, 931);
#endif

        if (ShowBanner)
        {
            ShowBanner.GetComponent<Button>().onClick.RemoveAllListeners();
            ShowBanner.GetComponent<Button>().onClick.AddListener(delegate { BtnClicked("show"); });
        }

        if (HideBanner)
        {
            HideBanner.GetComponent<Button>().onClick.RemoveAllListeners();
            HideBanner.GetComponent<Button>().onClick.AddListener(delegate { BtnClicked("hide"); });
        }
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameObject.Find("MessageBox"))
            Application.Quit();
    }

    public void BtnClicked(string param)
    {
        if (param.Equals("show"))
            CrossLink.Show();
        else if (param.Equals("hide"))
            CrossLink.Hide();
    }
}
