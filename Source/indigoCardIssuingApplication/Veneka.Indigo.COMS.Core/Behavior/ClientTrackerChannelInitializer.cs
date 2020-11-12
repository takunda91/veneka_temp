using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core.Behavior
{
    public class ClientTrackerChannelInitializer : IChannelInitializer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientTrackerChannelInitializer));

        internal static int ConnectedClientCount = 0;
        
        public void Initialize(IClientChannel channel)
        {
            ConnectedClientCount++;
            log.Debug(string.Format("Client {0} initialized", channel.SessionId));
            channel.Closed += ClientDisconnected;
            channel.Faulted += ClientDisconnected;
        }

        static void ClientDisconnected(object sender, EventArgs e)
        {
            log.Debug(string.Format( "Client {0} disconnected", ((IClientChannel)sender).SessionId));
            ConnectedClientCount--;
        }
    }
}
