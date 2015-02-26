using UnityEngine;

public static class FightCamera {
	//TODO: finalize this stuff
	public static void AdaptMain() {
		float w = 520f;
		float h = 540f;
		float y = 7f;
		float s = 5f;

		float diffW = (Screen.width / w) / (Screen.height / h);

		float newY = y;
		if (diffW < 1f) {
			//newY = y;
			newY = y / diffW;
			newY = newY - (newY - y) * 0.33f;
		} else {
			newY = y / diffW;
			newY = newY + (y - newY) * 0.33f;
		}
		float newS = s / diffW;

		Camera.main.orthographicSize = newS;
		Vector3 cameraPos = Camera.main.transform.position;
		cameraPos.y = newY;
		Camera.main.transform.position = cameraPos;
	}

	public static void AdaptCanvas(float defaultWidth, Canvas canvasBG) {
		canvasBG.scaleFactor = Screen.width / defaultWidth;
	}
}
