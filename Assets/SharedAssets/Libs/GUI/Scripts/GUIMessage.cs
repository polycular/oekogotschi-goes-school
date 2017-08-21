using UnityEngine;
using System.Collections.Generic;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// this class is used to return possible GUI errors to the user 
    /// </summary>
    public class GUIMessage
    {
        public string Origin;
        public Type MessageType;
        public string Message;

        public enum Type
        {
            LOG,
            WARNING,
        };

        /// <summary>
        /// Create a new GUIMessage
        /// </summary>
        /// <param name="origin">Name of the class where the error occured</param>
        /// <param name="type">Log / Warning / Error - used to call the proper unity errors</param>
        /// <param name="message">The error message displayed to the user</param>
        public GUIMessage(string origin, Type type, string message)
        {
            this.Origin = origin;
            this.MessageType = type;
            this.Message = message;
        }

        public void showMessage()
        {
            switch (MessageType)
            {
                case Type.LOG:
                    Debug.Log(string.Format("{0}: {1}", Origin, Message));
                    break;

                case Type.WARNING:
                    Debug.LogWarning(string.Format("{0}: {1}", Origin, Message));
                    break;

                default:
                    break;
            }
        }
    }

}

