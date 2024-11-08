





public class @Class
{
	//public private local routines
	public int AddOne(ref int i) { return i++; }

	private int _AddOne(ref int i) { return i++; }

	public int Work()
	{
		int i = 0;

		AddOne(ref i);
		_AddOne(ref i);
		goto AddOne;
		@return:
		
		return i;
		
		AddOne:
		{ i++; goto @return; }
	}
}






