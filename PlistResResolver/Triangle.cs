using System;
using System.Collections.Generic;
using System.Text;

namespace PlistResResolver
{
    class Triangle
    {
        public Vector a, b, c;

        public Triangle(Vector a, Vector b, Vector c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public bool ContainsPoint(Vector p)
        {
            Vector pa = new Vector(p, a);
            Vector pb = new Vector(p, b);
            Vector pc = new Vector(p, c);

            float t1 = pa.Cross(pb);
            float t2 = pb.Cross(pc);
            float t3 = pc.Cross(pa);

            if (Math.Abs(t1) < float.Epsilon)
            {
                return t2 * t3 >= 0;
            }
            if (Math.Abs(t2) < float.Epsilon)
            {
                return t1 * t3 >= 0;
            }
            if (Math.Abs(t3) < float.Epsilon)
            {
                return t2 * t1 >= 0;
            }

            return t1 * t2 >= 0 && t1 * t3 >= 0;
        }
    }
}
