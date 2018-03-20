using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace oi.plugin.linedrawing {

    public class LineSource : MonoBehaviour {
        public delegate void _NewPoint(string lineID, int index, Vector3 point);
        public event _NewPoint OnNewPoint;

        public delegate void _LineSettings(string lineID, Color col, float width);
        public event _LineSettings OnLineSettings;

        public delegate void _RemoveLine(string lineID);
        public event _RemoveLine OnRemoveLine;

        public delegate void _ResetLines();
        public event _ResetLines OnResetLines;

        protected void NewPoint(string lineID, int index, Vector3 pos) {
            if (OnNewPoint != null) OnNewPoint.Invoke(lineID, index, pos);
        }

        protected void LineSettings(string lineID, Color col, float width) {
            if (OnNewPoint != null) OnLineSettings.Invoke(lineID, col, width);
        }

        public void RemoveLine(string lineID) {
            if (OnNewPoint != null) OnRemoveLine.Invoke(lineID);
        }

        protected void ResetLines() {
            if (OnNewPoint != null) OnResetLines.Invoke();
        }

    }
}
