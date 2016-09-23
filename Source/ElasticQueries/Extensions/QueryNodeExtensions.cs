﻿using System;
using Foundatio.Parsers.LuceneQueries.Nodes;
using Foundatio.Parsers.LuceneQueries.Visitors;
using Nest;

namespace Foundatio.Parsers.ElasticQueries.Extensions {
    public static class QueryNodeExtensions {
        public static void InvalidateQuery(this IQueryNode node) {
            IQueryNode current = node;
            while (current != null) {
                if (current is GroupNode)
                    current.RemoveQuery();

                current = current.Parent;
            }
        }

        public static Operator GetOperator(this IQueryNode node, Operator defaultOperator) {
            var groupNode = node as GroupNode;
            if (groupNode == null)
                return defaultOperator;

            switch (groupNode.Operator) {
                case GroupOperator.And:
                    return Operator.And;
                case GroupOperator.Or:
                    return Operator.Or;
                default:
                    return defaultOperator;
            }
        }

        private const string QUERY_KEY = "@query";
        public static QueryBase GetQuery(this IQueryNode node) {
            object value = null;
            if (!node.Meta.TryGetValue(QUERY_KEY, out value))
                return null;

            return value as QueryBase;
        }

        public static void SetQuery(this IQueryNode node, QueryBase container) {
            node.Meta[QUERY_KEY] = container;
        }

        public static void RemoveQuery(this IQueryNode node) {
            if (node.Meta.ContainsKey(QUERY_KEY))
                node.Meta.Remove(QUERY_KEY);
        }
    }
}