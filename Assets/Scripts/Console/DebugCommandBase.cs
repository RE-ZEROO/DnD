using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    public string commandId {  get; private set; }
    public string commandDescription {  get; private set; }
    public string commandFormat {  get; private set; }

    public DebugCommandBase(string id, string description, string format)
    {
        commandId = id;
        commandDescription = description;
        commandFormat = format;
    }
}
