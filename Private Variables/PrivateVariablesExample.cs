

public class PrivateVariables
{
	//1. just use private keyword
	private float MyPrivateVariable = 1.0f;

	//2. use a method
	private float GetPrivateVar() { return 1.0f; }

	//3. use a property
	private string myPrivateVar;
	public string MyPrivateVar
	{
		get { return myPrivateVar; }
		set { myPrivateVar = value; }
	}

	//4. keep variable local to methods only
	public void DoSomeWork()
	{
		float MyPrivateVar = 1.0f;
		MyPrivateVar += 0.01f;
		DoMoreWork(ref MyPrivateVar);
		//etc...
	}
	private void DoMoreWork(ref float MyPrivateVar)
	{
		MyPrivateVar += 0.01f;
	}
}

