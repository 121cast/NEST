﻿using System;
using System.Collections.Generic;

namespace Nest
{
	public partial class ElasticClient
	{
	
		/// <summary>
		/// Performs a count query over all indices
		/// </summary>
		public ICountResponse CountAll<T>(Func<QueryDescriptor<T>, BaseQuery> querySelector) where T : class
		{
			querySelector.ThrowIfNull("querySelector");
			var descriptor = new QueryDescriptor<T>();
			querySelector(descriptor);
			var query = this.Serialize(descriptor);
			return this._Count("_count", query);
		}

        
		/// <summary>
		/// Perform a count query over the default index and the inferred type name for T
		/// </summary>
		public ICountResponse Count<T>(Func<QueryDescriptor<T>, BaseQuery> querySelector) where T : class
		{
			var index = this.Infer.IndexName<T>();
			index.ThrowIfNullOrEmpty("Cannot infer default index for current connection.");

			var type = typeof(T);
			var typeName = this.Infer.TypeName<T>();
			string path = this.Path.CreateIndexTypePath(index, typeName, "_count");
			var descriptor = new QueryDescriptor<T>();
			querySelector(descriptor);
			var query = this.Serialize(descriptor);
			return _Count(path, query);
		}
		
		/// <summary>
		/// Performs a count query over the specified indices
		/// </summary>
		public ICountResponse Count<T>(IEnumerable<string> indices, Func<QueryDescriptor<T>, BaseQuery> querySelector) where T : class
		{
			indices.ThrowIfEmpty("indices");
			string path = this.Path.CreateIndexPath(indices, "_count");
			var descriptor = new QueryDescriptor<T>();
			querySelector(descriptor);
			var query = this.Serialize(descriptor);
			return _Count(path, query);
		}
		
		/// <summary>
		///  Performs a count query over the multiple types in multiple indices.
		/// </summary>
		public ICountResponse Count<T>(IEnumerable<string> indices, IEnumerable<string> types, Func<QueryDescriptor<T>, BaseQuery> querySelector) where T : class
		{
			indices.ThrowIfEmpty("indices");
			indices.ThrowIfEmpty("types");
			string path = this.Path.CreateIndexTypePath(indices, types, "_count");
			var descriptor = new QueryDescriptor<T>();
			querySelector(descriptor);
			var query = this.Serialize(descriptor);
			return _Count(path, query);
		}

		private CountResponse _Count(string path, string query)
		{
			var status = this.Connection.PostSync(path, query);
			var r = this.Deserialize<CountResponse>(status);
			return r;
		}

	}
}
