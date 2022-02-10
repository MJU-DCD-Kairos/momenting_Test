using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameChatUnity;

namespace GameChatSample
{
    public class SampleGlobalData
    {
        public static Member G_User = null;

        public static bool G_isConnected = false;

        public static bool G_isSocketConnected = false;

        public static List<Channel> G_ChannelList = null;

    }
}
