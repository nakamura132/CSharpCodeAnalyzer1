using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    // C#コードを解析するためのアナライザークラス
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class NestedCodeAnalyzer : DiagnosticAnalyzer
    {
        // 診断IDの定義
        public const string DiagnosticId = "NEST001";

        // 診断のタイトル、メッセージフォーマット、説明の定義
        private static readonly LocalizableString Title = "メソッドのネストが深すぎます";
        private static readonly LocalizableString MessageFormat = "メソッド '{0}' には5レベルを超えるネストが含まれています";
        private static readonly LocalizableString Description = "メソッドのネストを5レベル以下にするようリファクタリングしてください。";
        private const string Category = "コードスメル";

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
            // メソッド宣言を解析するアクションを登録
            context.RegisterSyntaxNodeAction( AnalyzeMethod, SyntaxKind.MethodDeclaration );
        }

        // メソッド宣言の解析ロジック
        private static void AnalyzeMethod( SyntaxNodeAnalysisContext context )
        {
            // メソッド宣言ノードを取得
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            // ネストレベルを計算
            var nestingLevel = GetNestingLevel(methodDeclaration.Body);

            // ネストレベルが5を超える場合、警告を生成
            if ( nestingLevel > 5 )
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDeclaration.Identifier.GetLocation(),
                    methodDeclaration.Identifier.Text);

                context.ReportDiagnostic( diagnostic );
            }
        }

        // ネストレベルを計算するヘルパーメソッド
        private static int GetNestingLevel( SyntaxNode node )
        {
            var maxNesting = 0;
            var stack = new Stack<(SyntaxNode node, int level)>();
            stack.Push( (node, 1) );

            while ( stack.Count > 0 )
            {
                var (currentNode, level) = stack.Pop();
                maxNesting = Math.Max( maxNesting, level );

                foreach ( var childNode in currentNode.ChildNodes() )
                {
                    // ネストを増やす条件のノードを探索
                    if ( childNode is BlockSyntax || childNode is IfStatementSyntax ||
                        childNode is ForStatementSyntax || childNode is WhileStatementSyntax ||
                        childNode is DoStatementSyntax || childNode is SwitchStatementSyntax )
                    {
                        stack.Push( (childNode, level + 1) );
                    }
                }
            }

            return maxNesting;
        }
    }
}
