//C# stack allocation exercise

//is A stack allocated? no, A exists outside of method stack frame 
int A = 0;
public void MethodA()
{
	A++;
}

//is B stack allocated? yes, it should be, B is local to method stack frame
public void MethodB()
{
	int B = 0;
	B++;
}

//is C stack allocated? probably (?), C is local to first method's stack frame
public void MethodC()
{
	int C = 0;
	Increment(ref C);
}

public void Increment(ref int C)
{
	C++;
}