namespace Polycular.Collections
{
	public class Tuple<T, G>
	{
		public T First;
		public G Second;

		public Tuple(T first, G second)
		{
			First = first;
			Second = second;
		}

		public void Tie(out T first, out G second)
		{
			first = First;
			second = Second;
		}
	}
}