﻿namespace MethodCache.Tests
{
	using System;
	using System.Reflection;
	using System.Xml.Linq;
	using MethodCache.Tests.TestAssembly;
	using MethodCache.Tests.TestAssembly.Cache;
	using NUnit.Framework;

	public class PropertyCacheDisabledAtConfigTests : ModuleWeaverTestsBase
	{
		protected override XElement WeaverConfig
		{
			get { return new XElement("MethodCache", new XAttribute("CacheProperties", false)); }
		}

		[Test]
		public void ClassLevelCache_ShouldCachePropertiesWhenAllMembersSelectedExplicitlyOnClassAttribute()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassAllExplicitlyIncluded>(Assembly, cache);

			// Act
			dynamic value = instance.Property;
			value = instance.Property;

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(1));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(1));
		}

		[Test]
		public void ClassLevelCache_ShouldCachePropertiesWhenSelectedExplicitlyOnClassAttribute()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassMethodsExcluded>(Assembly, cache);

			// Act
			dynamic value = instance.Property;
			value = instance.Property;

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(1));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(1));
		}

		[Test]
		public void ClassLevelCache_ShouldNotCacheProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassWithProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyNoCache;
			value = instance.ReadOnlyNoCache;
			value = instance.AutoProperty;
			value = instance.AutoProperty;
			value = instance.ReadWriteProperty;
			value = instance.ReadWriteProperty;

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
		}

		[Test]
		public void ClassLevelCache_ShouldNotCacheStaticReadOnlyProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
			Type cacheType = Assembly.GetType(typeof(ICacheWithRemove).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			PropertyInfo property = testClassType.GetProperty("ReadOnlyProperty");

			// Act
			object value = property.GetValue(null, null);
			value = property.GetValue(null, null);

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
		}

		[Test]
		public void ClassLevelCache_ShouldNotCacheStaticReadWriteProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);

			Type testClassType = Assembly.GetType(typeof(TestClassStaticProperties).FullName);
			Type cacheType = Assembly.GetType(typeof(ICacheWithRemove).FullName);

			PropertyInfo cacheProperty = testClassType.GetProperty("Cache", cacheType);
			cacheProperty.SetValue(null, cache, null);

			PropertyInfo property = testClassType.GetProperty("ReadWriteProperty");

			// Act
			object value = property.GetValue(null, null);
			value = property.GetValue(null, null);

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(0));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(0));
		}

		[Test]
		public void IndividualCache_ShouldCacheEligibleProperties()
		{
			// Arrange
			dynamic cache = WeaverHelper.CreateInstance<DictionaryCache>(Assembly);
			dynamic instance = WeaverHelper.CreateInstance<TestClassIndividualProperties>(Assembly, cache);

			// Act
			dynamic value = instance.ReadOnlyProperty;
			value = instance.ReadOnlyProperty;
			value = instance.ReadWriteProperty;
			value = instance.ReadWriteProperty;

			// Assert
			Assert.That(cache.NumStoreCalls, Is.EqualTo(2));
			Assert.That(cache.NumRetrieveCalls, Is.EqualTo(2));
		}
	}
}