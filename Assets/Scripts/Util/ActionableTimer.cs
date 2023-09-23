public class ActionableTimer {
	public delegate void ActionDelegate();
	private float time = 0;

	public void Update(ActionDelegate actionDelegate, float actionInterval, float deltaTime) {
		time += deltaTime;
		if (time >= actionInterval) {
			time = 0;
			actionDelegate();
		}
	}

	public void Reset(){
		time = 0;
	}
}