using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    // C#コードを解析するためのアナライザークラス
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class SummaryCommentAnalyzer : DiagnosticAnalyzer
    {
        // 診断IDの定義
        public const string DiagnosticId = "SUMM001";

        // 診断のタイトル、メッセージフォーマット、説明の定義
        private static readonly LocalizableString Title = "パブリックメンバーに<summary>コメントがありません";
        private static readonly LocalizableString MessageFormat = "パブリックなメンバー '{0}' には<summary>コメントが必要です";
        private static readonly LocalizableString Description = "パブリックなクラス、メソッド、またはプロパティには<summary>コメントを追加してください。";
        private const string Category = "ドキュメント";

        // 診断ルールの定義
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        // サポートする診断を設定
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

        // アナライザーの初期化
        public override void Initialize( AnalysisContext context )
        {
            // 自動生成されたコードを解析対象から除外
            context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.None );
            // 並列実行を有効にする
            context.EnableConcurrentExecution();
            // クラス、メソッド、プロパティの宣言を解析するアクションを登録
            context.RegisterSyntaxNodeAction( AnalyzeNode, SyntaxKind.ClassDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration );
        }

        // ノードの解析ロジック
        private static void AnalyzeNode( SyntaxNodeAnalysisContext context )
        {
            // ノードがパブリックかどうかを確認
            if ( !IsPublic( context.Node ) )
            {
                return;
            }

            // <summary>コメントがあるかどうかを確認
            if ( !HasSummaryComment( context.Node ) )
            {
                var identifier = GetIdentifier(context.Node);
                if ( identifier.HasValue )
                {
                    var diagnostic = Diagnostic.Create(Rule, identifier.Value.GetLocation(), identifier.Value.Text);
                    context.ReportDiagnostic( diagnostic );
                }
            }
        }

        // ノードがパブリックかどうかを確認するヘルパーメソッド
        private static bool IsPublic( SyntaxNode node )
        {
            switch ( node )
            {
                case ClassDeclarationSyntax classDecl:
                    return classDecl.Modifiers.Any( SyntaxKind.PublicKeyword );
                case MethodDeclarationSyntax methodDecl:
                    return methodDecl.Modifiers.Any( SyntaxKind.PublicKeyword );
                case PropertyDeclarationSyntax propDecl:
                    return propDecl.Modifiers.Any( SyntaxKind.PublicKeyword );
                default:
                    return false;
            }
        }

        // <summary>コメントがあるかどうかを確認するヘルパーメソッド
        private static bool HasSummaryComment( SyntaxNode node )
        {
            var trivia = node.GetLeadingTrivia();
            foreach ( var trivium in trivia )
            {
                if ( trivium.IsKind( SyntaxKind.SingleLineDocumentationCommentTrivia ) ||
                    trivium.IsKind( SyntaxKind.MultiLineDocumentationCommentTrivia ) )
                {
                    var xmlTrivia = trivium.GetStructure();
                    foreach ( var xmlNode in xmlTrivia.ChildNodes() )
                    {
                        if ( xmlNode is XmlElementSyntax xmlElement && xmlElement.StartTag.Name.LocalName.Text == "summary" )
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // ノードの識別子を取得するヘルパーメソッド
        private static SyntaxToken? GetIdentifier( SyntaxNode node )
        {
            switch ( node )
            {
                case ClassDeclarationSyntax classDecl:
                    return classDecl.Identifier;
                case MethodDeclarationSyntax methodDecl:
                    return methodDecl.Identifier;
                case PropertyDeclarationSyntax propDecl:
                    return propDecl.Identifier;
                default:
                    return null;
            }
        }
    }
}
