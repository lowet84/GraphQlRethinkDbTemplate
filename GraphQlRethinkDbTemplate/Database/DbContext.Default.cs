﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public T AddDefault<T>(T item, Id? replaces = null) where T : NodeBase
        {
            var type = typeof(T);
            var table = GetTable(type);
            Utils.InitalizeArrays(item);
            var jObject = JObject.FromObject(item);
            var jToken = Utils.ChangeTypeBaseItemsToIds(type, jObject);
            var chainLink = Chain.CreateChainLink<T>(item.Id, replaces);
            var result = table.Insert(jToken).Do_(e => GetTable(typeof(Chain)).Insert(chainLink)).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }

            return item;
        }

        public T ReadByIdDefault<T>(Id id, UserContext.ReadType readType, GraphQLDocument document) where T : class
        {
            var selectionSet = document!= null ? GetSelectionSet(document) : null;

            switch (readType)
            {
                case UserContext.ReadType.Normal:
                    return GetWithDocument<T>(selectionSet, id);
                case UserContext.ReadType.Deep:
                    throw new NotImplementedException();
                case UserContext.ReadType.Shallow:
                    return GetShallow<T>(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(readType), readType, null);
            }
        }
    }
}
