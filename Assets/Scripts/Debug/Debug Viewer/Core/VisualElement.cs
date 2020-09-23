using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization {

    public class VisualElement {
        public Mesh mesh;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Color colour;
        public Style style;

        public VisualElement (Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour, Style style) {
            this.mesh = mesh;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.colour = colour;
            this.style = style;
        }
    }
}