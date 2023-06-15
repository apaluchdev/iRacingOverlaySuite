using irsdkSharp;
using irsdkSharp.Models;
using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Models.Data;
using irsdkSharp.Serialization.Models.Session;
using System;

namespace iRacingOverlaySuite
{
    public static class IRData
    {
        #region Public

        public static bool IsConnected { get; private set; }

        public static DataModel iRacingData { get; private set; }

        public static DataModel PreviousiRacingData { get; private set; }

        public static IRacingSessionModel? Session { get; private set; }

        #endregion

        #region Private

        private static IRacingSDK _sdk;

        #endregion

        static IRData()
        {
            _sdk = new IRacingSDK();
            iRacingData = new IRacingDataModel().Data;

            _sdk.OnConnected += Sdk_OnConnected;
            _sdk.OnDisconnected += Sdk_OnDisconnected;

        }

        private static void Sdk_OnDisconnected()
        {
            IsConnected = false;
        }

        private static void Sdk_OnConnected()
        {
            IsConnected = true;
        }

        public static void UpdateData()
        {
            try
            {
                if (_sdk == null)
                    return;

                int lastUpdate = -1;
                
                // Check if we can find the sim
                if (IsConnected /* Check if iRacing process is open as well */)
                {
                    PreviousiRacingData = iRacingData;

                    iRacingData = _sdk.GetSerializedData().Data;
                    
                    // Is the session info updated?
                    int newUpdate = _sdk.Header.SessionInfoUpdate;
                    if (newUpdate != lastUpdate)
                    {
                        lastUpdate = newUpdate;
                        Session = _sdk.GetSerializedSessionInfo();
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
