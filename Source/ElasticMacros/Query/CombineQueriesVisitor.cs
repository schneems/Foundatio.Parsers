﻿using System;
using System.Linq;
using ElasticQueryParser;
using Exceptionless.ElasticQueryParser.Extensions;
using Exceptionless.ElasticQueryParser.Query.Nodes;
using Exceptionless.LuceneQueryParser.Extensions;
using Nest;

namespace Exceptionless.ElasticQueryParser.Query {
    public class CombineQueriesVisitor : ElasticQueryNodeVisitorBase {
        private readonly ElasticQueryParserConfiguration _config;

        public CombineQueriesVisitor(ElasticQueryParserConfiguration config) {
            _config = config;
        }

        public override void Visit(QueryGroupNode node) {
            if (node.Query != null) {
                base.Visit(node);
                return;
            }

            QueryContainer query = null;
            foreach (var child in node.Children.OfType<IElasticQueryNode>()) {
                var childQuery = child.Query;
                var op = node.GetOperator(_config.DefaultQueryOperator);
                if (child.IsNegated())
                    childQuery = !childQuery;

                if (op == Operator.Or && !String.IsNullOrEmpty(node.Prefix) && node.Prefix == "+")
                    op = Operator.And;

                if (op == Operator.And) {
                    query &= childQuery;
                } else if (op == Operator.Or) {
                    query |= childQuery;
                }
            }

            node.Query = query;
            base.Visit(node);
        }
    }
}
