using L_AI.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace L_AI.CodeAnalysis
{
    public class CSharp : BaseAnalyzer<CSharp>
    {
        public override string LanguageCode => "C#";

        private bool _initialized;
        private VisualStudioWorkspace _workspace;
        private IEnumerable<Project> _projects;
        private IEnumerable<Document> _documents;
        private List<BaseTypeDeclarationSyntax> _types = new List<BaseTypeDeclarationSyntax>();

        // Should I cache ClassDeclaration to Documents?

        public CSharp()
        {
            Initialize();
        }

        public override async Task<string> GetSourceCodeFromClassAsync(string className)
        {
            return _types.FirstOrDefault(c => c.Identifier.Text == className)?.NormalizeWhitespace()?.ToFullString();
        }

        private async Task<Document> GetDocumentFromClassNameAsync(string className)
        {
            foreach (var document in _documents)
            {
                var syntaxRoot = await document.GetSyntaxRootAsync();

                // Find the class declaration
                var classDeclaration = syntaxRoot.DescendantNodes()
                                                 .OfType<ClassDeclarationSyntax>()
                                                 .FirstOrDefault(c => c.Identifier.Text == className);

                if (classDeclaration != null)
                    return document;
            }

            return null;
        }

        public override async Task<IEnumerable<string>> GetAllReferenesAsTextAsync(string className)
        {
            var symbols = await GetUniqueReferencesFromClassAsync(className);
            if (symbols == null) return null;

            var allStrings = symbols.Select(x => GetSourceCodeFromClassAsync(x.Name).Result);
            return allStrings;
        }

        public override async Task<IEnumerable<ISymbol>> GetUniqueReferencesFromClassAsync(string className)
        {
            Document document = await GetDocumentFromClassNameAsync(className);
            if (document == null) return null;

            var symbols = new HashSet<ISymbol>();
            var syntaxRoot = await document.GetSyntaxRootAsync();
            var semanticModel = await document.GetSemanticModelAsync();

            var allNodes = syntaxRoot.DescendantNodes();

            foreach (var node in allNodes)
            {
                var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                if (symbol != null)
                    symbols.Add(symbol);
            }

            var assemblies = _projects.Select(x => x.AssemblyName).ToList();

            // Right here LINQ shat itself and thinks "assemblies" is out of scope...
            var fromAssembly = new List<INamespaceOrTypeSymbol>();
            foreach (var symbol in symbols)
            {
                if (!(symbol is INamespaceOrTypeSymbol namespaceOrTypeSymbol) || !(symbol is ITypeSymbol typeSymbol)) continue;
                if (!assemblies.Contains(symbol.ContainingAssembly?.Name)) continue;

                fromAssembly.Add(namespaceOrTypeSymbol);
            }

            var toRemove = new List<INamespaceOrTypeSymbol>();
            foreach (var symbol in fromAssembly)
            {
                if (!(symbol is INamespaceOrTypeSymbol namespaceOrTypeSymbol) || !(symbol is ITypeSymbol typeSymbol)) continue;

                // Remove if it's a base or interface of already existing reference
                if (typeSymbol.TypeKind == TypeKind.Interface)
                {
                    var referencedChild = fromAssembly.FirstOrDefault(x => (x as ITypeSymbol).Interfaces.Contains(x));
                    if (referencedChild != null)
                        toRemove.Add(referencedChild);
                }

                // Remove current if it's a generic that has the base type referenced
                if (typeSymbol.TypeKind == TypeKind.TypeParameter)
                {
                    var refTypes = (typeSymbol as ITypeParameterSymbol).ConstraintTypes;
                    var referencedChild = fromAssembly.FirstOrDefault(x => refTypes.Contains(x));
                    if (referencedChild != null)
                        toRemove.Add(typeSymbol);
                }

            }

            fromAssembly.RemoveAll(x => toRemove.Contains(x));

            return fromAssembly;
        }

        public override async Task<IEnumerable<string>> GetAllReferenesAsTextFromPathAsync(string path)
        {
            var document = _documents.FirstOrDefault(x => x.FilePath == path);
            if (document == null)
                return null;

            var syntaxRoot = await document.GetSyntaxRootAsync();
            var allClassesInDocument = syntaxRoot.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Select(x => x.Identifier.Text);

            //
            var allStrings = new List<string>();

            foreach(var classDefenition in allClassesInDocument)
            {
                allStrings.AddRange(await GetAllReferenesAsTextAsync(classDefenition));
            }
            return allStrings;
        }

        public override async Task Test() { }

        private void Initialize()
        {
            if (_initialized) return;

            LAIPackage.Instance.JoinableTaskFactory.Run(async () =>
            {
                await UpdateWorkspaceAsync();
            });

            _workspace.WorkspaceChanged += Workspace_WorkspaceChanged;

            _initialized = true;
        }

        private async Task UpdateWorkspaceAsync()
        {
            // Does workspace change as well or is it only CurrentSolution?
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            _workspace = componentModel.GetService<VisualStudioWorkspace>();
            _projects = _workspace.CurrentSolution.Projects;
            _documents = _projects.SelectMany(x => x.Documents);

            _types.Clear();
            foreach (var document in _documents)
            {
                var syntaxRoot = await document.GetSyntaxRootAsync();
                if (syntaxRoot.Language != LanguageCode) continue;

                _types.AddRange(syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>());
                _types.AddRange(syntaxRoot.DescendantNodes().OfType<InterfaceDeclarationSyntax>());
                _types.AddRange(syntaxRoot.DescendantNodes().OfType<StructDeclarationSyntax>());
                _types.AddRange(syntaxRoot.DescendantNodes().OfType<RecordDeclarationSyntax>());
                _types.AddRange(syntaxRoot.DescendantNodes().OfType<EnumDeclarationSyntax>());
            }
        }

        private void Workspace_WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            LAIPackage.Instance.JoinableTaskFactory.Run(async () =>
            {
                await UpdateWorkspaceAsync();
            });
        }
    }
}
