using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int width, int height, float a, float b) {
		float[,] map = new float[width,height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				float x = i / (float)width * 2 - 1;
				float y = j / (float)height * 2 - 1;

				float value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y));
				map [i, j] = Evaluate(value, a, b);
			}
		}

		return map;
	}

	static float Evaluate(float value, float a, float b) {
		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}
}
