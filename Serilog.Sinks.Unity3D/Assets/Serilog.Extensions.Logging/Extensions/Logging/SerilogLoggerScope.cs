// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Extensions.Logging
{
    public interface ISerilogLoggerScopeChain
    {
        SerilogLoggerScope CurrentScope { get; set; }
    }

    public class SerilogLoggerScope : IDisposable
    {
        const string NoName = "None";

        readonly ISerilogLoggerScopeChain _provider;
        readonly object _state;
        readonly IDisposable _chainedDisposable;

        // An optimization only, no problem if there are data races on this.
        bool _disposed;

        public SerilogLoggerScope(ISerilogLoggerScopeChain provider, object state, IDisposable chainedDisposable = null)
        {
            _provider = provider;
            _state = state;

            Parent = _provider.CurrentScope;
            _provider.CurrentScope = this;
            _chainedDisposable = chainedDisposable;
        }

        public SerilogLoggerScope Parent { get; }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                // In case one of the parent scopes has been disposed out-of-order, don't
                // just blindly reinstate our own parent.
                for (var scan = _provider.CurrentScope; scan != null; scan = scan.Parent)
                {
                    if (ReferenceEquals(scan, this))
                        _provider.CurrentScope = Parent;
                }

                _chainedDisposable?.Dispose();
            }
        }

        public void EnrichAndCreateScopeItem(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, out LogEventPropertyValue scopeItem)
        {
            if (_state == null)
            {
                scopeItem = null;
                return;
            }

            if (_state is IEnumerable<KeyValuePair<string, object>> stateProperties)
            {
                scopeItem = null; // Unless it's `FormattedLogValues`, these are treated as property bags rather than scope items.

                foreach (var stateProperty in stateProperties)
                {
                    var item = AddOrCreateProperty(stateProperty.Key, stateProperty.Value);
                    if (item != null) scopeItem = item;
                }
            }
            else if (_state is KeyValuePair<string, object> pair)
            {
                scopeItem = AddOrCreateProperty(pair.Key, pair.Value);
            }
            else if (_state is ValueTuple<string, object> tuple)
            {
                var (key, value) = tuple;
                scopeItem = AddOrCreateProperty(key, value);
            }
            else
            {
                scopeItem = propertyFactory.CreateProperty(NoName, _state).Value;
            }

            LogEventPropertyValue AddOrCreateProperty(string key, object value)
            {
                if (key == SerilogLoggerProvider.OriginalFormatPropertyName && value is string)
                {
                    return new ScalarValue(_state.ToString());
                }

                var destructureObject = key.StartsWith("@");
                var property = propertyFactory.CreateProperty(
                    destructureObject ? key.Substring(1) : key
                    , value, destructureObject
                );
                logEvent.AddPropertyIfAbsent(property);
                return null;
            }
        }
    }
}
