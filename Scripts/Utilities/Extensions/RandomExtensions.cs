using Random = System.Random;

namespace FPTemplate.Utilities.Extensions
{
    public static class RandomExtensions
	{
		public static bool Flip(this Random rand)
		{
			return rand.NextDouble() > 0.5;
		}
	}
}