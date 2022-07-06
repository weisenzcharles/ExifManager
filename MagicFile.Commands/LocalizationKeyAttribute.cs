using System;

namespace MagicFile
{
	public class LocalizationKeyAttribute : Attribute
	{
		public string LocalizationKey { get; }

		public LocalizationKeyAttribute(string key)
		{
			LocalizationKey = key;
		}
	}
}
