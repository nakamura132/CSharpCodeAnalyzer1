using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Analyzer1
{
    // C#コード用の診断アナライザーを定義する
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class Analyzer1Analyzer : DiagnosticAnalyzer
    {
        // 診断IDの定義
        public const string DiagnosticId = "MethodNamingAnalyzer";

        // 各診断のメタデータの設定
        private static readonly LocalizableString Title = "メソッド名は大文字で始まる必要があります";
        private static readonly LocalizableString MessageFormat = "メソッド名 '{0}' は大文字で始まる必要があります";
        private static readonly LocalizableString Description = "メソッド名が小文字で始まっています。コーディング規則に従い、大文字で始めてください。";
        private const string Category = "Naming";

        // 診断規則を作成
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        // サポートする診断を返す
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

        // アナライザーの初期化
        public override void Initialize( AnalysisContext context )
        {
            // 自動生成されたコードの解析を無効化し、並列実行を有効にする
            context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.None );
            context.EnableConcurrentExecution();

            // メソッド宣言に対してアクションを登録
            context.RegisterSyntaxNodeAction( AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration );
        }

        // メソッド宣言を解析する
        private static void AnalyzeMethodDeclaration( SyntaxNodeAnalysisContext context )
        {
            // 現在のノードをメソッド宣言として取得
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            // メソッド名を取得
            var methodName = methodDeclaration.Identifier.Text;

            // メソッド名が小文字で始まる場合に警告を生成
            if ( !string.IsNullOrEmpty( methodName ) && char.IsLower( methodName[0] ) )
            {
                // 診断を生成し、コンテキストに報告
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodName);
                context.ReportDiagnostic( diagnostic );
            }
        }
    }
}
