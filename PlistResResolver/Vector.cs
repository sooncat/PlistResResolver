namespace PlistResResolver
{
    class Vector
    {
        public float x, y;

        public Vector()
        {
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// the vector = a point to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Vector(Vector a, Vector b)
        {
            this.x = b.x - a.x;
            this.y = b.y - a.y;
        }

        public float Dot(Vector vec)
        {
            return x * vec.x + y * vec.y;
        }

        public float Cross(Vector vec)
        {
            return x * vec.y - y * vec.x;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }
}
