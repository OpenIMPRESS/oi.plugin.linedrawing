using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using oi.core.network;

namespace oi.plugin.linedrawing {

    public enum LineDrawMode {
        draw = 0,
        erase = 1,
    }

    public class LineDrawCapture : LineSource {

        private string lineIDPrefix = "_";

        private Vector3 lastPos;
        private float lastDist = 0;
        private Vector3 heading;
        private bool trackHeading = false;
        private int pointAm = 0;
        private string lineID = "0";
        public static int globalLineID = 0;

        public Color col;
        public float size = 0.01f;

        public LineDrawMode mode = LineDrawMode.draw;
        private bool recording = false;

        public bool buttonDown = false;
        public bool pauze = false;
        public bool resetDrawings = false;
        private string collidingID = "";

        private Material mat;

        // Use this for initialization
        void Start() {
            SessionManager sm = FindObjectOfType<SessionManager>();
            if (sm != null) lineIDPrefix = sm.GetGUID();
            ResetLines();
            GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
            mat = GetComponent<Renderer>().material;
            SetColor(col);
            SetSize(size);
        }

        private void OnTriggerEnter(Collider other) {
            collidingID = other.gameObject.name;
        }

        private void OnTriggerExit(Collider other) {
            string id = other.gameObject.name;
            if (collidingID == id) {
                collidingID = "";
            }
        }

        public void SetButtonDown(bool _down) {
            buttonDown = _down;
            if (pauze)
                buttonDown = false;
        }

        public void SetPauze(bool _pauze) {
            pauze = _pauze;
            buttonDown = false;
        }

        void StartNewLine() {
            globalLineID++;
            lineID = lineIDPrefix+"_"+globalLineID;
            pointAm = 0;
            NewPos(transform.position);
            LineSettings(lineID, col, size);
        }

        void NewPos(Vector3 pos) {
            lastPos = pos;
            trackHeading = false;
            NewPoint(lineID, pointAm, pos);
            pointAm++;
        }

        public void SetColor(Color _col) {
            col = _col;
            mat.color = col;
        }

        public void SetSize(float _size) {
            size = _size;
            transform.localScale = new Vector3(_size, _size, _size);
        }

        // Update is called once per frame
        void Update() {

            if (resetDrawings) {
                resetDrawings = false;
                ResetLines();
            }

            if (buttonDown && !pauze) {
                if (mode == LineDrawMode.erase) {
                    if (collidingID != "") {
                        RemoveLine(collidingID);
                        collidingID = "";
                    }
                } else if (mode == LineDrawMode.draw && !recording) {
                    recording = true;
                    StartNewLine();
                }
            } else {
                if (recording) {
                    recording = false;
                    LineSettings(lineID, col, size);
                }
            }



            if (recording) {
                Vector3 currentPos = transform.position;
                NewPoint(lineID, pointAm, currentPos);
                float dist = Vector3.Distance(currentPos, lastPos);

                if (!trackHeading) {
                    if (dist > 0.01) {
                        heading = currentPos - lastPos;
                        heading.Normalize();
                        trackHeading = true;
                    }

                } else {
                    Vector3 currentHeading = currentPos - lastPos;
                    currentHeading.Normalize();

                    float angle = Vector3.Angle(currentHeading, heading);

                    float angleThresh = Mathf.Min(2, 1 / dist);
                    if (angle > angleThresh || dist < lastDist) {
                        NewPos(currentPos);
                    }
                }
                lastDist = dist;
            }
        }
    }
}