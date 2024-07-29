using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    /// <summary>
    /// 条件式で代入を行う場合に警告を表示するアナライザー。
    /// </summary>
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class AssignmentInConditionAnalyzer : DiagnosticAnalyzer
    {
        // 診断IDの定義
        public const string DiagnosticId = "COND001";

        // 診断のタイトル、メッセージフォーマット、説明の定義
        private static readonly LocalizableString Title = "条件式内での代入が検出されました";
        private static readonly LocalizableString MessageFormat = "条件式の中で代入が行われています: '{0}'";
        private static readonly LocalizableString Description = "条件式の中で代入を行うと誤解を招く恐れがあります。== を = と誤って使用している可能性があります。";
        private const string Category = "構文";

        // 診断ルールの定義
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
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
            // IfStatement の解析を登録
            context.RegisterSyntaxNodeAction( AnalyzeIfStatement, SyntaxKind.IfStatement );
        }

        // IfStatement ノードの解析ロジック
        private static void AnalyzeIfStatement( SyntaxNodeAnalysisContext context )
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            // 条件式を取得
            var condition = ifStatement.Condition;

            // 条件式内に代入演算子が含まれているか確認
            if ( ContainsAssignment( condition ) )
            {
                var diagnostic = Diagnostic.Create(Rule, condition.GetLocation(), condition.ToString());
                context.ReportDiagnostic( diagnostic );
            }
        }
        private static bool ContainsAssignment( SyntaxNode node )
        {
            // ノードが AssignmentExpressionSyntax である場合は true を返す
            if ( node is AssignmentExpressionSyntax )
            {
                return true;
            }
            // 条件式内の代入を検出するためのロジック
            return node.DescendantNodes().OfType<AssignmentExpressionSyntax>().Any();
        }
        
    }
}
