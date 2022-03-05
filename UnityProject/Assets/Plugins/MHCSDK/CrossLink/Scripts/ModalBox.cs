using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public abstract class ModalBox : MonoBehaviour
{
    /// <summary>
    /// The title text object that will be used to show the title of the dialog.
    /// </summary>
    [Tooltip("The title text object that will be used to show the title of the dialog.")]
    public Text Title;

    /// <summary>
    /// The message text object that will be used to show the message of the dialog.
    /// </summary>
    [Tooltip("The message text object that will be used to show the message of the dialog.")]
    public Text Message;

    /// <summary>
    /// The botton object that will be used to interact with the dialog. This will be cloned to produce additional options.
    /// </summary>
    [Tooltip("The botton object that will be used to interact with the dialog. This will be cloned to produce additional options.")]
    public Button Button;

    /// <summary>
    /// The RectTransform of the panel that contains the frame of the dialog window. This is needed so that it can be centered correctly after it's size is adjusted to the dialogs contents.
    /// </summary>
    [Tooltip("The RectTransform of the panel that contains the frame of the dialog window. This is needed so that it can be centered correctly after it's size is adjusted to the dialogs contents.")]
    public RectTransform Panel;

    public Sprite[] ButtonImages;

    Transform buttonParent;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CrossLink.confirmationActive = false;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public virtual void Close()
    {
        Destroy(gameObject);
    }

    protected void SetText(string message, string title)
    {
        if (Button != null)
            buttonParent = Button.transform.parent;

        if (Title != null)
        {
            if (!String.IsNullOrEmpty(title))
            {
                Title.text = MessageBox.LocalizeTitleAndMessage ? CrossLink.instance.localizeParams.Localize(title) : title;
            }
            else
            {
                Destroy(Title.gameObject);
                Title = null;
            }
        }

        if (Message != null)
        {
            if (!String.IsNullOrEmpty(message))
            {
                Message.text = MessageBox.LocalizeTitleAndMessage ? CrossLink.instance.localizeParams.Localize(message) : message;
            }
            else
            {
                Destroy(Message.gameObject);
                Message = null;
            }
        }
    }
}
