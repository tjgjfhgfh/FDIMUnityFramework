using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoCanvas : MonoBehaviour
{
    public Text ClientMessages;
    public Text HostMessages;

    public InputField HostInput;
    public InputField ClientInput;

    public DemoStringEvent HostSendEvent;
    public DemoStringEvent ClientSendEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClientMessage(string message)
    {
        HostMessages.text += $"{message}\n";
    }

    public void OnHostMessage(string message)
    {
        ClientMessages.text += $"{message}\n";
    }

    public void OnHostSendClick()
    {
        HostSendEvent?.Invoke(HostInput.text);
        HostInput.text = string.Empty;
    }

    public void OnClientSendClick()
    {
        ClientSendEvent?.Invoke(ClientInput.text);
        ClientInput.text = string.Empty;
    }
}
