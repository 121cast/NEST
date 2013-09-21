﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Nest
{
	public partial class ElasticClient
	{
		/// <summary>
		/// Search using T as the return type
		/// </summary>
		public IQueryResponse<T> MoreLikeThis<T>(Func<MoreLikeThisDescriptor<T>, MoreLikeThisDescriptor<T>> mltSelector) where T : class
		{
			var mltDescriptor = new MoreLikeThisDescriptor<T>();
			var descriptor = mltSelector(mltDescriptor);

			var path = this.Path.GetMoreLikeThisPathFor(descriptor);
			ConnectionStatus status = null;
			if (descriptor._Search == null)
			{
				status = this.Connection.GetSync(path);
			}
			else
			{
				var search = this.Serialize(descriptor._Search);
				status = this.Connection.PostSync(path, search);
			}
			return this.Deserialize<QueryResponse<T>>(status, extraConverters: new List<JsonConverter>
			{
				new ConcreteTypeConverter(typeof (T), (d, h) => typeof (T))
			});
		}
	}
}