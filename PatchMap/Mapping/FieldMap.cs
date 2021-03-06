﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PatchMap.Mapping
{
    internal static class CompiledRegexes
    {
        public static readonly Regex WordSeperator = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);
    }

    public class FieldMap<TTarget, TContext> : Map<TTarget, TContext>
    {
        public List<PropertyInfo> SourceField { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> TargetField { get; protected set; } = new List<PropertyInfo>();
        
        protected internal bool CollectionItem { get; protected set; }
        protected internal LabelMethod<TTarget, TContext>? LabelGenerator { get; protected set; }
        protected internal EnabledMethod<TTarget, TContext>? Enabled { get; protected set; }
        protected internal RequiredMethod<TTarget, TContext>? Required { get; protected set; }
        protected internal ConversionMethod<TTarget, TContext>? Converter { get; protected set; }
        protected internal FieldPostMapMethod<TTarget, TContext>? PostMap { get; protected set; }
        
        public FieldMap() { }

        public FieldMap(LambdaExpression sourceFieldExp, LambdaExpression? targetFieldExp)
        {
            static List<PropertyInfo> GetProperties(Expression body)
            {
                var properties = new List<PropertyInfo>();
                var memberExp = body as MemberExpression ?? (body as UnaryExpression)?.Operand as MemberExpression;
                while (memberExp != null)
                {
                    properties.Add((PropertyInfo)memberExp.Member);
                    memberExp = memberExp.Expression as MemberExpression;
                }

                properties.Reverse();
                return properties;
            }

            SourceField = GetProperties(sourceFieldExp.Body);
            if (SourceField.Any())
            {
                LabelGenerator = (target, ctx) => CompiledRegexes.WordSeperator.Replace(SourceField.Last().Name, "$1 $2");
            }

            if (targetFieldExp != null)
            {
                TargetField = GetProperties(targetFieldExp.Body);
            }
        }

        public string? GenerateLabel(TTarget target, TContext context) => LabelGenerator?.Invoke(target, context);

        public FieldMapConversionResult<object> ConvertValue(TTarget target, object? value, TContext context)
        {
            return Converter == null || value == null
                ? new FieldMapConversionResult<object> { Value = value! }  // TODO: find a way to indicate this is nullable
                : Converter.Invoke(target, context, value);
        }

        public FieldMap<TTarget, TContext> HasLabel(LabelMethod<TTarget, TContext> label)
        {
            LabelGenerator = label;

            return this;
        }
        
        public FieldMap<TTarget, TContext> IsEnabled(EnabledMethod<TTarget, TContext> enabled)
        {
            Enabled = enabled;

            return this;
        }
        
        public FieldMap<TTarget, TContext> IsRequired(RequiredMethod<TTarget, TContext> required)
        {
            Required = required;
            
            return this;
        }
        
        public FieldMap<TTarget, TContext> IsCollectionItem()
        {
            CollectionItem = true;
            
            return this;
        }
        
        public FieldMap<TTarget, TContext> HasPostMap(FieldPostMapMethod<TTarget, TContext> postMap)
        {
            PostMap = postMap;
            
            return this;
        }
    }

    public class FieldMap<TTarget, TContext, TSourceProp, TTargetProp> : FieldMap<TTarget, TContext>
    {
        public FieldMap(LambdaExpression sourceFieldExp, LambdaExpression? targetFieldExp) : base(sourceFieldExp, targetFieldExp) { }

        public FieldMap<TTarget, TContext, TSourceProp, TTargetProp> HasConverter(ConversionMethod<TTarget, TContext, TSourceProp, TTargetProp> converter)
        {
            Converter = (TTarget target, TContext ctx, object value) =>
            {
                var r = converter(target, ctx, (TSourceProp)value);
                return new FieldMapConversionResult<object>
                {
                    Value = r.Value!, // TODO: find a way to indicate this is nullable
                    FailureReason = r.FailureReason
                };
            };

            return this;
        }
    }
}
