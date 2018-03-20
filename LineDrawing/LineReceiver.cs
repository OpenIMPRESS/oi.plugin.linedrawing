using UnityEngine;
using System.IO;
using System.Collections.Generic;

using oi.core.network;

namespace oi.plugin.linedrawing {


    public class LineReceiver : LineSource {
        private List<byte[]> newData = new List<byte[]>();

        public UDPConnector UDPClient;

        private void Start() {
        }

        private void Update() {
            UpdateReceiveData();
        }

        void UpdateReceiveData() {
            byte[] data = UDPClient.GetNewData();
            while (data != null) {
                if (data.Length != 0) {

                    int packetID = -1;
                    using (MemoryStream str = new MemoryStream(data)) {
                        using (BinaryReader reader = new BinaryReader(str)) {
                            packetID = reader.ReadInt32();
                        }
                    }

                    if (packetID == 3) {  // line point packet
                        string lineID;
                        int pointID;
                        Vector3 pos;
                        LinePointSerializer.Deserialize(data, out lineID, out pointID, out pos);
                        NewPoint(lineID, pointID, pos);
                    } else if (packetID == 4) {    // line settings packet
                        string lineID;
                        Color col;
                        float width;
                        LineSettingsSerializer.Deserialize(data, out lineID, out col, out width);
                        LineSettings(lineID, col, width);
                    } else if (packetID == 5) {    // line remove packet
                        string lineID;
                        LineRemoveSerializer.Deserialize(data, out lineID);
                        RemoveLine(lineID);
                    } else if (packetID == 6) {    // lines reset packet
                        ResetLines();
                    }
                }
                data = UDPClient.GetNewData();
            }
        }
    }
}