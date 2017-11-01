using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace oi.plugin.linedrawing {

    public class LineSource : MonoBehaviour {
        public delegate void _NewPoint(int lineID, int index, Vector3 point);
        public event _NewPoint OnNewPoint;

        public delegate void _LineSettings(int lineID, Color col, float width);
        public event _LineSettings OnLineSettings;

        public delegate void _RemoveLine(int lineID);
        public event _RemoveLine OnRemoveLine;

        public delegate void _ResetLines();
        public event _ResetLines OnResetLines;

        protected void NewPoint(int lineID, int index, Vector3 pos) {
            OnNewPoint?.Invoke(lineID, index, pos);
        }

        protected void LineSettings(int lineID, Color col, float width) {
            OnLineSettings?.Invoke(lineID, col, width);
        }

        public void RemoveLine(int lineID) {
            OnRemoveLine?.Invoke(lineID);
        }

        protected void ResetLines() {
            OnResetLines?.Invoke();
        }

    }
}
