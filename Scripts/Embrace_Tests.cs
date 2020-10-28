using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmbraceSDK
{
    public class Embrace_Tests
    {
        public void RunTests()
        {
            Embrace.Instance.StartSDK();
            Embrace.Instance.SetUserIdentifier("embrace_test_user");
            Embrace.Instance.SetUserIdentifier(null);
            Embrace.Instance.ClearUserIdentifier();
            Embrace.Instance.SetUserEmail("embrace_test_email");
            Embrace.Instance.SetUserEmail(null);
            Embrace.Instance.ClearUserEmail();
            Embrace.Instance.SetUserAsPayer();
            Embrace.Instance.ClearUserAsPayer();
            Embrace.Instance.SetUserPersona("embrace_test_persona");
            Debug.Log("running set b");
            Embrace.Instance.SetUserPersona(null);
            Embrace.Instance.ClearUserPersona("embrace_test_persona");
            Embrace.Instance.ClearUserPersona(null);
            Embrace.Instance.ClearAllUserPersonas();
            Embrace.Instance.AddSessionProperty("test_key", "test_value", true);
            Embrace.Instance.AddSessionProperty("test_key", "test_value", false);
            Embrace.Instance.AddSessionProperty("test_key", null, false);
            Embrace.Instance.AddSessionProperty(null, "test_value", false);
            Embrace.Instance.AddSessionProperty(null, null, false);
            Embrace.Instance.RemoveSessionProperty("test_key");
            Embrace.Instance.RemoveSessionProperty("test_key_doesnt_exist");
            Embrace.Instance.RemoveSessionProperty(null);
            Debug.Log("running set c");
            Embrace.Instance.AddSessionProperty("test_key", "test_value", true);
            Dictionary<string, string> sessionProperties = Embrace.Instance.GetSessionProperties();
            foreach (var item in sessionProperties.Keys)
            {
                string value = sessionProperties[item];
                Debug.Log("session properties: " + item + " = " + value);
            }
            Embrace.Instance.SetUsername("embrace_test_user");
            Embrace.Instance.SetUsername(null);
            Embrace.Instance.ClearUsername();
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("test_key", "test_value");
            Embrace.Instance.StartMoment("test_name", "test_id", true, properties);
            Embrace.Instance.StartMoment("test_name", "test_id", false, properties);
            Embrace.Instance.StartMoment("test_name", "test_id", true, null);
            Embrace.Instance.StartMoment("test_name", null, true, properties);
            Embrace.Instance.StartMoment(null, "test_id", true, properties);
            Embrace.Instance.StartMoment(null, null, true, properties);
            Embrace.Instance.EndMoment("test_name", "test_id", properties);
            Embrace.Instance.EndMoment("test_name", "test_id", null);
            Embrace.Instance.EndMoment("test_name", null, properties);
            Embrace.Instance.EndMoment(null, "test_id", properties);
            Embrace.Instance.EndMoment(null, null, properties);
            Embrace.Instance.EndAppStartup(null);
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Info, properties, true);
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Info, null, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Info, properties, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Info, null, true);
            Debug.Log("running set d");
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Warning, properties, true);
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Warning, null, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Warning, properties, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Warning, null, true);
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Error, properties, true);
            Embrace.Instance.LogMessage("test_message", EMBSeverity.Error, null, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Error, properties, true);
            Embrace.Instance.LogMessage(null, EMBSeverity.Error, null, true);
            Embrace.Instance.LogBreadcrumb("test_message");
            Embrace.Instance.LogBreadcrumb(null);
            Embrace.Instance.EndSession(true);
            Embrace.Instance.EndSession(false);
            string deviceId = Embrace.Instance.GetDeviceId();
            Debug.Log("deviceid: " + deviceId);
            Embrace.Instance.StartView("test_view");
            Embrace.Instance.StartView(null);
            Embrace.Instance.EndView("test_view");
            Embrace.Instance.EndView(null);
            Debug.Log("running set e");
        }
    }
}
