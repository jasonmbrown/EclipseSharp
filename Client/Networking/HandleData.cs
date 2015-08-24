using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Networking;
using Client.Rendering;

namespace Client.Networking {
    public static class HandleData {

        internal static void HandleAlertMessage(DataBuffer buffer) {
            var message = buffer.ReadString();
            Interface.ShowMessagebox("Error", message);
        }
    }
}
