using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oi.core.network;

namespace oi.plugin.linedrawing {

    public class LineSender : MonoBehaviour {

        public UDPConnector udpConnector;
        private LineDrawer lineDrawer;
        private bool connected = false;

        void Start() {
            lineDrawer = FindObjectOfType<LineDrawer>();

            LineSource[] lineSources = Resources.FindObjectsOfTypeAll(typeof(LineSource)) as LineSource[];
            foreach (LineSource lineSource in lineSources) {
                if (lineSource.GetType() == typeof(LineReceiver)) {
                    Debug.Log("Ignoring LineReceiver");
                    continue;
                }
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


        void NewPoint(string lineID, int pointID, Vector3 point) {
            byte[] serialized = LinePointSerializer.Serialize(lineID, pointID, point);
            udpConnector.SendData(serialized);
        }

        void ResetLines() {
            byte[] serialized = LinesResetSerializer.Serialize();
            udpConnector.SendData(serialized);
        }

        void RemoveLine(string lineID) {
            byte[] serialized = LineRemoveSerializer.Serialize(lineID);
            udpConnector.SendData(serialized);
        }

        void LineSettings(string lineID, Color col, float width) {
            byte[] serialized = LineSettingsSerializer.Serialize(lineID, col, width);
            udpConnector.SendData(serialized);
        }


        void SendAllLines() {
            ResetLines();
            Dictionary<string, LineRenderer> lines = lineDrawer.GetLines();
            foreach (KeyValuePair<string, LineRenderer> entry in lines) {
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