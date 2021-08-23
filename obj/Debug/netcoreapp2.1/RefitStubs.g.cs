﻿// <auto-generated />
using System;
using System.Net.Http;
using System.Collections.Generic;
using CoreBot.RefitInternalGenerated;

/* ******** Hey You! *********
 *
 * This is a generated file, and gets rewritten every time you build the
 * project. If you want to edit it, you need to edit the mustache template
 * in the Refit package */

#pragma warning disable
namespace CoreBot.RefitInternalGenerated
{
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    sealed class PreserveAttribute : Attribute
    {

        //
        // Fields
        //
        public bool AllMembers;

        public bool Conditional;
    }
}
#pragma warning restore

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
namespace CoreBot.Fields
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Newtonsoft.Json;
    using global::Refit;

    /// <inheritdoc />
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [Preserve]
    [global::System.Reflection.Obfuscation(Exclude=true)]
    partial class AutoGeneratedICepApiService : ICepApiService
    {
        /// <inheritdoc />
        public HttpClient Client { get; protected set; }
        readonly IRequestBuilder requestBuilder;

        /// <inheritdoc />
        public AutoGeneratedICepApiService(HttpClient client, IRequestBuilder requestBuilder)
        {
            Client = client;
            this.requestBuilder = requestBuilder;
        }

        /// <inheritdoc />
        Task<CepFields> ICepApiService.GetAddressAsync(string cep)
        {
            var arguments = new object[] { cep };
            var func = requestBuilder.BuildRestResultFuncForMethod("GetAddressAsync", new Type[] { typeof(string) });
            return (Task<CepFields>)func(Client, arguments);
        }
    }
}

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning restore CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
