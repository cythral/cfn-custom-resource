using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Amazon.Runtime;

using McMaster.NETCore.Plugins;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Cythral.CodeGeneration.Roslyn;


namespace Cythral.CloudFormation.CustomResource.Generator
{
    public class PermissionsCollector : CSharpSyntaxWalker
    {

        private CSharpCompilation compilation;

        public HashSet<string> Permissions { get; private set; } = new HashSet<string>();

        public PermissionsCollector(CSharpCompilation compilation)
        {
            this.compilation = compilation;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            try
            {
                var type = GetCallingMemberType(node);
                if (type == null || !IsAmazonResponseType(type))
                {
                    foreach (var child in node.ChildNodes())
                    {
                        Visit(child);
                    }

                    return;
                }


                var (loader, assembly) = LoadAssemblyForType(type);

                using (loader.EnterContextualReflection())
                {
                    var iamPrefix = GetClientConfig(type, assembly).AuthenticationServiceName;
                    var apiCallName = GetApiCallName(node);
                    var permission = iamPrefix + ":" + apiCallName;

                    if (permission == "lambda:Invoke") permission = "lambda:InvokeFunction";
                    Permissions.Add(permission);
                }

            }
            catch (Exception)
            {
                foreach (var child in node.ChildNodes())
                {
                    Visit(child);
                }
            }
        }

        private IClientConfig GetClientConfig(ITypeSymbol type, Assembly assembly)
        {
            try
            {
                var configClassName = GetConfigClassName(type);
                var metadataType = assembly.GetType(configClassName, true);

                if (metadataType != null)
                {
                    return (ClientConfig)Activator.CreateInstance(metadataType);
                }
            }
            catch (Exception) { }

            var configType = assembly.GetTypes().Where(t => typeof(ClientConfig).IsAssignableFrom(t)).FirstOrDefault();
            return configType != null ? (ClientConfig)Activator.CreateInstance(configType) : null;
        }

        private ITypeSymbol GetCallingMemberType(InvocationExpressionSyntax node)
        {
            var semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
            var info = semanticModel.GetTypeInfo(node);
            var symbol = info.ConvertedType as INamedTypeSymbol;
            return symbol.TypeArguments.Count() > 0 ? symbol.TypeArguments[0] : null;
        }

        private string GetApiCallName(InvocationExpressionSyntax node)
        {
            try
            {
                var accessExpression = node.Expression as MemberAccessExpressionSyntax;
                var matcher = new Regex("Async$");
                return matcher.Replace(accessExpression.Name.ToString(), "");
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsAmazonResponseType(ITypeSymbol type)
        {
            try
            {
                return type.ContainingAssembly.Name.Split('.')[0] == "AWSSDK" &&
                    type.Name.EndsWith("Response");
            }
            catch (Exception)
            {
                return false;
            }
        }

        // gets the assembly for an amazon api call
        private (PluginLoader, Assembly) LoadAssemblyForType(ITypeSymbol type)
        {
            var assemblyReferences = from reference in compilation.ExternalReferences
                                     where Path.GetFileNameWithoutExtension(reference.Display) == type.ContainingAssembly.Name
                                     select reference.Display;

            var referenceLocation = assemblyReferences.First();
            PluginLoader loader = PluginLoader.CreateFromAssemblyFile(referenceLocation, sharedTypes: new Type[] { typeof(ClientConfig) });
            return (loader, loader.LoadDefaultAssembly());
        }

        private string GetConfigClassName(ITypeSymbol type)
        {
            var fullNS = type.ContainingNamespace.ToString();
            var baseNS = string.Join(".", fullNS.Split('.').Take(2));
            var squashedNS = string.Join("", fullNS.Split('.').Take(2));
            return baseNS + "." + squashedNS + "Config";
        }
    }
}