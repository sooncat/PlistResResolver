using System;
using System.Collections.Generic;
using System.Text;

namespace PlistResResolver
{
    class Rect
    {
        public float x, y, w, h;
        public Rect()
        { }

        public Rect(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}
