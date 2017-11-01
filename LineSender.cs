using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oi.plugin.linedrawing {

    public class LineSender : MonoBehaviour {

        public IMPRESS_UDPClient udpConnector;
        private IMPRESS_LineDrawer lineDrawer;
        private bool connected = false;

        void Start() {
            lineDrawer = FindObjectOfType<IMPRESS_LineDrawer>();

            LineSource[] lineSources = Resources.FindObjectsOfTypeAll(typeof(LineSource)) as LineSource[];
            foreach (LineSource lineSource in lineSources) {
                lineSource.OnNewPoint += NewPoint;
                lineSource.OnLineSettings += LineSettings;
                lineSource.OnRemoveLine += RemoveLine;
                lineSource.OnResetLines += ResetLines;
            }
        }

        private void Update() {
            if (connected != udpConnector.connected) {
                connected = udpConnector.connected;
                if (connected) {
                    SendAllLines();
                }
            }
        }


        void NewPoint(int lineID, int pointID, Vector3 point) {
            byte[] serialized = LinePointSerializer.Serialize(lineID, pointID, point);
            udpConnector.SendData(serialized);
        }

        void ResetLines() {
            byte[] serialized = LinesResetSerializer.Serialize();
            udpConnector.SendData(serialized);
        }

        void RemoveLine(int lineID) {
            byte[] serialized = LineRemoveSerializer.Serialize(lineID);
            udpConnector.SendData(serialized);
        }

        void LineSettings(int lineID, Color col, float width) {
            byte[] serialized = LineSettingsSerializer.Serialize(lineID, col, width);
            udpConnector.SendData(serialized);
        }


        void SendAllLines() {
            ResetLines();
            Dictionary<int, LineRenderer> lines = lineDrawer.GetLines();
            foreach (KeyValuePair<int, LineRenderer> entry in lines) {
                LineRenderer line = entry.Value;
                LineSettings(entry.Key, line.startColor, line.widthMultiplier);
                for (int i = 0; i < line.positionCount; i++) {
                    Vector3 point = line.GetPosition(i);
                    NewPoint(entry.Key, i, point);
                }
            }
        }
    }
}