using System.Collections.Generic;

namespace DisCatSharp.Support.Entities
{
	/// <summary>
	/// Multidictionary
	/// </summary>
	/// <typeparam name="TKey">Key</typeparam>
	/// <typeparam name="TValue">Value</typeparam>
	internal class MultiDict<TKey, TValue>
	{
		private readonly Dictionary<TKey, List<TValue>> _data = new();

		/// <summary>
		/// Adds a <see cref="List{T}"/> to an <see cref="Dictionary{TKey, TValue}"/>
		/// </summary>
		/// <param name="k">Key</param>
		/// <param name="v">Value</param>
		public void Add(TKey k, TValue v)
		{
			if (_data.ContainsKey(k))
				_data[k].Add(v);
			else
				_data.Add(k, new List<TValue>() { v });
		}

		/// <summary>
		/// Deletes a <see cref="List{T}"/> from  an <see cref="Dictionary{TKey, TValue}"/>
		/// </summary>
		/// <param name="k">Key</param>
		/// <param name="v">Value</param>
		public void Del(TKey k, TValue v)
		{
			if (_data.ContainsKey(k))
				_data[k].Remove(v);
		}

		/// <summary>
		/// Gets a <see cref="Dictionary{TKey, TValue}"/>
		/// </summary>
		/// <returns>Dictionary</returns>
		public Dictionary<TKey, List<TValue>> Get()
		{
			return _data;
		}
	}
}
