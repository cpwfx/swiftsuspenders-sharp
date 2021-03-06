﻿using System;
using SwiftSuspenders.DependencyProviders;
using SwiftSuspenders.Support.Types;
using SwiftSuspenders;
using System.Collections.Generic;

namespace SwiftSuspenders.Support.providers
{
	public class UnknownParametersUsingProvider : DependencyProvider
	{
		public event Action<DependencyProvider, object> PostApply;
		public event Action<DependencyProvider, object> PreDestroy;

		public string parameterValue;

		public object Apply(Type targetType , Injector activeInjector, Dictionary<string, object> injectParameters)
		{
			parameterValue = injectParameters["param"] as string;
			object instance = new Clazz ();
			if (PostApply != null)
			{
				PostApply(this, instance);
			}
			return instance;
		}

		public void Destroy()
		{
			if (PreDestroy != null) 
			{
				PreDestroy (this, null);
			}
		}
	}
}

