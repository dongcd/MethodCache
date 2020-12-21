namespace MethodCache.Tests.TestAssembly
{
	using MethodCache.Attributes;
	using MethodCache.Tests.TestAssembly.Cache;
    using CacheAttribute = Cache.CacheAttribute;

    [Attributes.Cache(Members.All)]
	public class TestClassAllExplicitlyIncluded
	{
		public TestClassAllExplicitlyIncluded(ICache cache)
		{
			Cache = cache;
		}

		public ICache Cache { get; private set; }

		[Cache(CacheKeyPrefix="G:BaseData:")]
		public string Property
		{
			get { return "some value"; }
		}

		public int Method(int x)
		{
			return x * x;
		}
	}
}