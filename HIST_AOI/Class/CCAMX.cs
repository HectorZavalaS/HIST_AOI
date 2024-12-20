using HIST_AOI.wsCAMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    class CCAMX
    {
        private String m_token_id;
        private String m_user;
        private string m_password;
        private String m_tool;
        private wsCAMX.CAMXMessageBrokerClient m_CAMXEvents;

        public string Token_id { get => m_token_id; set => m_token_id = value; }
        public string User { get => m_user; set => m_user = value; }
        public string Password { get => m_password; set => m_password = value; }
        public string Tool { get => m_tool; set => m_tool = value; }
        public CAMXMessageBrokerClient CAMXEvents { get => m_CAMXEvents; set => m_CAMXEvents = value; }

        public CCAMX(String user, String pass, String tool)
        {
            m_user = user;
            m_password = pass;
            m_tool = tool;
            m_CAMXEvents = new wsCAMX.CAMXMessageBrokerClient();
        }

        public bool login()
        {
            bool result = true;
            String res = m_CAMXEvents.login(m_user, m_password);

            if (res.Contains("<InvalidCredentials />")) result = false;
            else m_token_id = res.Replace("<Token tokenId=\"", "").Replace("\" />\r\n", "");

            return result;
        }

        public void logout()
        {
            m_CAMXEvents.logout(m_token_id);
        }

        public bool suscribeIWStart()
        {
            return (m_CAMXEvents.subscribeForEvent(m_token_id, m_tool, "2541/ItemWorkStart.xsd")).Contains("<Success />");
        }

        public bool suscribeIWComplete()
        {
            return (m_CAMXEvents.subscribeForEvent(m_token_id, m_tool, "2541/ItemWorkComplete.xsd")).Contains("<Success />");
        }

        public bool suscribeITransferIn()
        {
            return (m_CAMXEvents.subscribeForEvent(m_token_id, m_tool, "2541/ItemTransferIn.xsd")).Contains("<Success />");
        }

        public async Task<wsCAMX.waitForEventResponse> waitForEvent()
        {
            return await m_CAMXEvents.waitForEventAsync(m_token_id);
        }
        public async Task<wsCAMX.pullEventsResponse> pullEvents()
        {
            return await m_CAMXEvents.pullEventsAsync(m_token_id);
        }
        public async Task<wsCAMX.acknowledgeEventsResponse> acknowledgeEvents(String message)
        {
            return await m_CAMXEvents.acknowledgeEventsAsync(m_token_id, message);
        }
    }
}
