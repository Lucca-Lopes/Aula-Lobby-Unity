//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Services.Lobbies.Http;



namespace Unity.Services.Lobbies.Models
{
    [Preserve]
    [DataContract(Name = "TokenRequest")]
    public class TokenRequest
    {
        /// <summary>
        /// Details about a token being requested.
        /// </summary>
        /// <param name="tokenType">tokenType param</param>
        [Preserve]
        public TokenRequest(TokenTypeOptions tokenType)
        {
            TokenType = tokenType;
        }

        /// <summary>
        /// 
        /// </summary>
        [Preserve]
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember(Name = "tokenType", IsRequired = true, EmitDefaultValue = true)]
        public TokenTypeOptions TokenType{ get; }
    

        /// <summary>
        /// Defines TokenType
        /// </summary>
        [Preserve]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TokenTypeOptions
        {
            /// <summary>
            /// Enum VivoxJoin for value: vivoxJoin
            /// </summary>
            [EnumMember(Value = "vivoxJoin")]
            VivoxJoin = 1,

            /// <summary>
            /// Enum WireJoin for value: wireJoin
            /// </summary>
            [EnumMember(Value = "wireJoin")]
            WireJoin = 2

        }

    }
}

