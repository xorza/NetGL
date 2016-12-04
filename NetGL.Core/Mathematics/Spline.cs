using System.Collections.Generic;

namespace NetGL.Core.Mathematics {
    public static class Spline {
        public static List<Vector3> CatmullRom(IList<Vector3> points, float step) {

            var spline = new List<Vector3>();

            for (int i = 0; i < points.Count - 1; i++) {
                Vector3 v;
                var steps = (int)(Vector3.Distance(points[i + 1], points[i]) / step);

                for (int j = 0; j < steps; j++) {
                    var t = j / (float)steps;

                    if (i == 0)
                        v = Vector3.CatmullRom(points[0], points[0], points[1], points[2], t);
                    else if (i == points.Count - 2)
                        v = Vector3.CatmullRom(points[i - 1], points[i], points[i + 1], points[i + 1], t);
                    else
                        v = Vector3.CatmullRom(points[i - 1], points[i], points[i + 1], points[i + 2], t);

                    spline.Add(v);
                }
            }

            return spline;
        }
    }
}
