using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oi.plugin.linedrawing {

    public class LineDrawer : MonoBehaviour {
        private Dictionary<string, LineRenderer> lines = new Dictionary<string, LineRenderer>();
        public Material lineMat;

        // Use this for initialization
        void Start() {
            LineSource[] lineSources = Resources.FindObjectsOfTypeAll(typeof(LineSource)) as LineSource[];
            foreach (LineSource lineSource in lineSources) {
                lineSource.OnNewPoint += NewPoint;
                lineSource.OnLineSettings += LineSettings;
                lineSource.OnRemoveLine += RemoveLine;
                lineSource.OnResetLines += ResetLines;
            }
        }

        public Dictionary<string, LineRenderer> GetLines() {
            return lines;
        }

        void ResetLines() {
            foreach (KeyValuePair<string, LineRenderer> entry in lines) {
                LineRenderer line = entry.Value;
                Destroy(line.gameObject);
            }
            lines.Clear();
        }

        void RemoveLine(string lineID) {
            if (lines.ContainsKey(lineID)) {
                Destroy(lines[lineID].gameObject);
                lines.Remove(lineID);
            }
        }

        void LineSettings(string lineID, Color col, float width) {
            if (!lines.ContainsKey(lineID))
                StartNewLine(lineID);
            LineRenderer line = lines[lineID];

            line.material.color = col;
            line.startColor = col;
            line.endColor = col;
            line.widthMultiplier = width;
        }

        void StartNewLine(string lineID) {
            GameObject obj = new GameObject();
            obj.layer = gameObject.layer;
            obj.transform.SetParent(transform);
            obj.name = lineID.ToString();

            LineRenderer line = obj.AddComponent<LineRenderer>();
            line.positionCount = 0;
            line.useWorldSpace = false;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.material = new Material(lineMat);
            line.startColor = new Color(1, 0.5f, 0);
            line.widthMultiplier = 0.01f;

            BoxCollider collider = line.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.enabled = true;
            lines.Add(lineID, line);
        }

        void NewPoint(string lineID, int index, Vector3 point) {
            if (!lines.ContainsKey(lineID))
                StartNewLine(lineID);
            LineRenderer line = lines[lineID];

            if (line.positionCount <= index) {

                if (line.positionCount < index) { // a position has been skipped; check for empty positions
                    line.positionCount = index + 1;
                    Vector3 lastPoint = point;
                    Vector3 emptyPos = new Vector3();
                    for (int i = line.positionCount - 1; i >= 0; i--) {
                        if (line.GetPosition(i) == emptyPos) {
                            line.SetPosition(i, lastPoint);
                        } else lastPoint = line.GetPosition(i);
                    }
                } else line.positionCount = index + 1;
            }
            line.SetPosition(index, point);

            BoxCollider collider = line.gameObject.GetComponent<BoxCollider>();
            collider.center = line.bounds.center;
            collider.size = line.bounds.size;
        }
    }
}