using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineToMesh {
    [System.Serializable]
    public enum ColorMode {
        BASIC,
        RAINBOW
    };

    public static Mesh CreateMeshFromLine(List<Vector3> line, List<Vector3> nonShiftedLine, float thickness, ColorMode colorMode, Color color) {
        /* INIT */
        Mesh mesh = new Mesh();

        //TESTING ADD POS TO LINE, TO MAKE LINE LOCAL TO OBJECT
        //for (int i = 0; i < line.Count; i++) {
        //    var oldPos = line[i];
        //    oldPos -= pos;
        //    line[i] = oldPos;
        //}

        if (line.Count > 0) {
            List<Vector3> vertices = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector2> uv0 = new List<Vector2>();

            List<Vector2> lineNormals = CalculateLineNormals(line);

            
            var rainbowIndex = Mathf.PerlinNoise(line[0].x*100f, line[0].y * 100f);
            var rainbowStep = 0.05f;

            // circle formula x=rsin?, y=rcos?
            // note that we'll use duplicate vertices for the sake of simplicity in this code

            //first line segment piece
            vertices.Add(line[0]);
            normals.Add(Vector3.back);
            colors.Add(GetColorFromMode(colorMode, color, rainbowIndex + rainbowStep));
            uv0.Add(nonShiftedLine[0]);

            var capAngle = 0f;
            if (lineNormals.Count > 0) {
                capAngle = Mathf.Atan2(lineNormals[0].y, lineNormals[0].x);
            }

            int capVerticesResolution = 16;
            float angleStep = 180f / (capVerticesResolution - 1);

            for (int i = 0; i < capVerticesResolution; i++) {
                var angle = (i * angleStep);

                var norm = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0);
                norm = Quaternion.Euler(0, 0, Mathf.Rad2Deg * capAngle + 90) * norm;
                //norm = Vector3.RotateTowards()

                var x = line[0].x + norm.x * thickness;
                var y = line[0].y + norm.y * thickness;

                var nsx = nonShiftedLine[0].x + norm.x * thickness;
                var nsy = nonShiftedLine[0].y + norm.y * thickness;

                vertices.Add(new Vector3(x, y, line[0].z));
                uv0.Add(new Vector2(nsx,nsy));
                normals.Add(norm);
                colors.Add(GetColorFromMode(colorMode, color, rainbowIndex + rainbowStep));
            }

            rainbowIndex += rainbowStep;

            for (int i = 0; i < capVerticesResolution - 1; i++) {
                tris.Add(0);
                tris.Add(i + 1);
                tris.Add(i + 2);
            }

            var lastColor = GetColorFromMode(colorMode, color, rainbowIndex);

            /* Line segments */ //DUPLICATED VERTS
            for (int i = 0; i < line.Count - 1; i++) {
                int currentVertexIndex = vertices.Count;

                vertices.Add(line[i]); // 0
                vertices.Add(line[i + 1]); // 1

                vertices.Add(line[i] + (Vector3)lineNormals[i] * thickness); // 2
                vertices.Add(line[i + 1] + (Vector3)lineNormals[i + 1] * thickness); // 3

                vertices.Add(line[i] + -(Vector3)lineNormals[i] * thickness); // 4
                vertices.Add(line[i + 1] + -(Vector3)lineNormals[i + 1] * thickness); // 5

                uv0.Add(nonShiftedLine[i]); // 0
                uv0.Add(nonShiftedLine[i + 1]); // 1

                uv0.Add(nonShiftedLine[i] + (Vector3)lineNormals[i] * thickness); // 2
                uv0.Add(nonShiftedLine[i + 1] + (Vector3)lineNormals[i + 1] * thickness); // 3

                uv0.Add(nonShiftedLine[i] + -(Vector3)lineNormals[i] * thickness); // 4
                uv0.Add(nonShiftedLine[i + 1] + -(Vector3)lineNormals[i + 1] * thickness); // 5

                tris.Add(currentVertexIndex);
                tris.Add(currentVertexIndex + 2);
                tris.Add(currentVertexIndex + 3);

                tris.Add(currentVertexIndex);
                tris.Add(currentVertexIndex + 3);
                tris.Add(currentVertexIndex + 1);

                tris.Add(currentVertexIndex);
                tris.Add(currentVertexIndex + 5);
                tris.Add(currentVertexIndex + 4);

                tris.Add(currentVertexIndex);
                tris.Add(currentVertexIndex + 1);
                tris.Add(currentVertexIndex + 5);


                normals.Add(Vector3.back); // 0
                normals.Add(Vector3.back); // 1

                normals.Add(lineNormals[i]); // 2
                normals.Add(lineNormals[i + 1]); // 3

                normals.Add(-lineNormals[i]); // 4
                normals.Add(-lineNormals[i + 1]); // 5

                var ColorOne = GetColorFromMode(colorMode, color, rainbowIndex);
                var ColorTwo = GetColorFromMode(colorMode, color, rainbowIndex + rainbowStep);

                colors.Add(ColorOne);
                colors.Add(ColorTwo);

                colors.Add(ColorOne);
                colors.Add(ColorTwo);

                colors.Add(ColorOne);
                colors.Add(ColorTwo);

                lastColor = ColorTwo;
                rainbowIndex += rainbowStep;
            }

            /* End cap */
            //last line segment piece
            var triStartingIndex = vertices.Count;

            vertices.Add(line[line.Count - 1]);
            normals.Add(Vector3.back);
            colors.Add(lastColor);
            uv0.Add(nonShiftedLine[nonShiftedLine.Count - 1]);

            capAngle = 0f;
            if (lineNormals.Count > 0) {
                capAngle = Mathf.Atan2(lineNormals[lineNormals.Count - 1].y, lineNormals[lineNormals.Count - 1].x);
            }



            for (int i = 0; i < capVerticesResolution; i++) {
                var angle = (i * angleStep);

                var norm = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0);
                norm = Quaternion.Euler(0, 0, Mathf.Rad2Deg * capAngle - 90) * norm;
                //norm = Vector3.RotateTowards()

                var x = line[line.Count - 1].x + norm.x * thickness;
                var y = line[line.Count - 1].y + norm.y * thickness;

                var nsx = nonShiftedLine[nonShiftedLine.Count - 1].x + norm.x * thickness;
                var nsy = nonShiftedLine[nonShiftedLine.Count - 1].y + norm.y * thickness;

                vertices.Add(new Vector3(x, y, line[line.Count - 1].z));
                normals.Add(norm);
                colors.Add(lastColor);
                uv0.Add(new Vector2(nsx, nsy));

            }

            for (int i = 0; i < capVerticesResolution - 1; i++) {
                tris.Add(triStartingIndex);
                tris.Add(triStartingIndex + i + 1);
                tris.Add(triStartingIndex + i + 2);
            }

            /* Finishing up */
            mesh.SetVertices(vertices);
            mesh.SetTriangles(tris, 0);
            mesh.SetNormals(normals);
            mesh.SetColors(colors);
            mesh.SetUVs(0, uv0);
        }
        return mesh;
    }

    private static Color GetColorFromMode(ColorMode colorMode, Color color, float step) {
        switch (colorMode) {
            case ColorMode.BASIC:
                return color;
                break;
            case ColorMode.RAINBOW:
                return Color.HSVToRGB(Mathf.Repeat(step, 1.0f), 1f, 1f);
                break;
            default:
                return Color.white;
                break;
        }
    }

    private static List<Vector2> CalculateLineNormals(List<Vector3> line) {
        List<Vector2> lineNormals = new List<Vector2>();

        if (line.Count > 1) {
            Vector2 firstNormal = GetSegmentNormal(line[0], line[1]);
            lineNormals.Add(firstNormal);

            for (int i = 0; i < line.Count - 2; i++) {
                Vector2 firstHalfNormal = GetSegmentNormal(line[i], line[i + 1]);
                Vector2 lastHalfNormal = GetSegmentNormal(line[i + 1], line[i + 2]);

                Vector2 extrustionVector = (firstHalfNormal + lastHalfNormal).normalized;
                lineNormals.Add(extrustionVector);
            }

            Vector2 lastNormal = GetSegmentNormal(line[line.Count - 2], line[line.Count - 1]);
            lineNormals.Add(lastNormal);

        }

        return lineNormals;
    }

    private static Vector2 GetSegmentNormal(Vector2 a, Vector2 b) {
        var x1 = a.x;
        var x2 = b.x;

        var y1 = a.y;
        var y2 = b.y;

        var dx = x2 - x1;
        var dy = y2 - y1;

        return new Vector2(-dy, dx).normalized;
    }
}
