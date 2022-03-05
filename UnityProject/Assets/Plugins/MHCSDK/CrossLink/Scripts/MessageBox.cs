using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class MessageBox : ModalBox
{
    [Tooltip("Set to true to send the title and message of message boxes and menus thru the Localize function.")]
    public static bool LocalizeTitleAndMessage = true;

    DialogResult result;
    Action<DialogResult> onFinish;

    public static MessageBox Show(GameObject MessageBox, string message, Action<DialogResult> onFinished, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        return Show(MessageBox, message, null, onFinished, buttons);
    }

    public static MessageBox Show(GameObject MessageBox, string message, string title = null, Action<DialogResult> onFinished = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        var box = (Instantiate(MessageBox) as GameObject).GetComponent<MessageBox>();
        box.name = "MessageBox";

        box.onFinish = onFinished;

        box.SetUpButtons(buttons);
        box.SetText(message, title);

        return box;
    }

    void SetUpButtons(MessageBoxButtons buttons)
    {
        while (!CrossLink.instance)
            SetUpButtons(buttons);

        var button = Button.gameObject;
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("OK");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.OK; Close(); });
                break;
            case MessageBoxButtons.OKCancel:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("OK");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.OK; Close(); });

                CreateButton(button, CrossLink.instance.localizeParams.Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); }, (ButtonImages.Length > 1) ? ButtonImages[1] : null);
                break;
            case MessageBoxButtons.YesNo:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("yes");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Yes; Close(); });

                CreateButton(button, CrossLink.instance.localizeParams.Localize("no"), () => { result = DialogResult.No; Close(); }, (ButtonImages.Length > 1) ? ButtonImages[1] : null);
                break;
            case MessageBoxButtons.RetryCancel:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("Retry");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Retry; Close(); });

                CreateButton(button, CrossLink.instance.localizeParams.Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); }, (ButtonImages.Length > 1) ? ButtonImages[1] : null);
                break;
            case MessageBoxButtons.YesNoCancel:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("yes");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Yes; Close(); });

                CreateButton(button, CrossLink.instance.localizeParams.Localize("no"), () => { result = DialogResult.No; Close(); });
                CreateButton(button, CrossLink.instance.localizeParams.Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); });
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                button.GetComponentInChildren<Text>().text = CrossLink.instance.localizeParams.Localize("Abort");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Abort; Close(); });

                CreateButton(button, CrossLink.instance.localizeParams.Localize("Retry"), () => { result = DialogResult.Retry; Close(); });
                CreateButton(button, CrossLink.instance.localizeParams.Localize("Ignore"), () => { result = DialogResult.Ignore; Close(); });
                break;
        }
    }

    GameObject CreateButton(GameObject buttonToClone, string label, UnityAction target, Sprite sprite = null)
    {
        var button = Instantiate(buttonToClone) as GameObject;

        button.transform.SetParent(buttonToClone.transform.parent, false);

        button.GetComponentInChildren<Text>().text = label;
        button.GetComponent<Button>().onClick.AddListener(target);

        if (sprite != null)
        {
            button.GetComponent<Image>().sprite = sprite;
        }

        return button;
    }

    public override void Close()
    {
        if (onFinish != null)
            onFinish(result);
        base.Close();
    }
}

public enum DialogResult
{
    Yes,
    No,
    OK,
    Cancel,
    Abort,
    Retry,
    Ignore
}

public enum MessageBoxButtons
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel,
    RetryCancel,
    AbortRetryIgnore
}
