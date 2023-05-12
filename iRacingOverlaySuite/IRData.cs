using irsdkSharp;
using irsdkSharp.Models;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Models.Data;
using irsdkSharp.Serialization.Models.Session;
using System;

namespace iRacingOverlaySuite
{
    public class IRData
    {
        #region Public

        public bool IsConnected { get; private set; }

        public DataModel iRacingData;

        #endregion

        #region Private

        private IRacingSDK _sdk;

        private IRacingSessionModel? _session = null;

        #endregion

        public IRData()
        {
            _sdk = new IRacingSDK();
            iRacingData = new IRacingDataModel().Data;

            _sdk.OnConnected += Sdk_OnConnected;
            _sdk.OnDisconnected += Sdk_OnDisconnected;

        }

        private void Sdk_OnDisconnected()
        {
            IsConnected = false;
        }

        private void Sdk_OnConnected()
        {
            IsConnected = true;
        }

        public void UpdateData()
        {
            try
            {
                if (_sdk == null)
                    return;

                int lastUpdate = -1;
                
                // Check if we can find the sim
                if (IsConnected /* Check if iRacing process is open as well */)
                {
                    iRacingData = _sdk.GetSerializedData().Data;
                    
                    // Is the session info updated?
                    int newUpdate = _sdk.Header.SessionInfoUpdate;
                    if (newUpdate != lastUpdate)
                    {
                        lastUpdate = newUpdate;
                        _session = _sdk.GetSerializedSessionInfo();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
