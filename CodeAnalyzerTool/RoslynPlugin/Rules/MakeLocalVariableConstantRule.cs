﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MakeLocalVariableConstantRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string CATEGORY = RuleCategories.PERFORMANCE;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantDescription), Resources.ResourceManager, typeof(Resources));

    public MakeLocalVariableConstantRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Info;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY, Severity, true, Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
    }

    private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node.ContainsDiagnostics) return;

        var localDeclarationStatement = (LocalDeclarationStatementSyntax)context.Node;

        if (localDeclarationStatement.IsConst) return; // check whether type is already const

        var parent = localDeclarationStatement.Parent;
        if (parent is null) return;

        // check whether the type of the var can legally be declared as const
        var type = context.SemanticModel.GetTypeInfo(localDeclarationStatement.Declaration.Type, context.CancellationToken).Type;
        if (type is null || !CanTypeBeConst(type)) return;

        // if (localDeclaration.Declaration.Type.IsVar) todo check if var can be explicitly declared?


        foreach (VariableDeclaratorSyntax declarator in localDeclarationStatement.Declaration.Variables)
        {
            ExpressionSyntax value = declarator.Initializer?.Value?.WalkDownParentheses();
        
            if (value?.IsMissing != false)
                return;
        
            if (!HasConstantValue(value, type, context.SemanticModel, context.CancellationToken))
                return;
        }


        SyntaxList<StatementSyntax> statements = parent.Kind() switch
        {
            SyntaxKind.Block => ((BlockSyntax)parent).Statements,
            SyntaxKind.SwitchSection => ((SwitchSectionSyntax)parent).Statements,
            _ => throw new ArgumentOutOfRangeException() // todo
        };

        int index = statements.IndexOf(localDeclarationStatement);
        if (!CanBeMarkedAsConst(context, localDeclarationStatement.Declaration.Variables, statements, index + 1))
            return;

        var diagnostic = Diagnostic.Create(_rule,
            localDeclarationStatement.GetLocation(),
            Severity);
        context.ReportDiagnostic(diagnostic);
    }
    
    
    private static bool CanBeMarkedAsConst(
        SyntaxNodeAnalysisContext context,
        SeparatedSyntaxList<VariableDeclaratorSyntax> variables,
        SyntaxList<StatementSyntax> statements,
        int startIndex)
    {
        MakeLocalVariableConstantWalker walker = null;

        try
        {
            walker = MakeLocalVariableConstantWalker.GetInstance();

            walker.SemanticModel = context.SemanticModel;
            walker.CancellationToken = context.CancellationToken;

            foreach (VariableDeclaratorSyntax variable in variables)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) as ILocalSymbol;

                if (symbol is not null)
                    walker.Identifiers[variable.Identifier.ValueText] = symbol;
            }

            for (int i = startIndex; i < statements.Count; i++)
            {
                walker.Visit(statements[i]);

                if (walker.Result)
                    return false;
            }
        }
        finally
        {
            if (walker is not null)
                MakeLocalVariableConstantWalker.Free(walker);
        }

        return true;
    }
    
    private static bool HasConstantValue(
        ExpressionSyntax expression,
        ITypeSymbol typeSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Boolean:
            {
                if (expression.Kind() == SyntaxKind.TrueLiteralExpression || expression.Kind() == SyntaxKind.FalseLiteralExpression)
                    return true;

                break;
            }
            case SpecialType.System_Char:
            {
                if (expression.IsKind(SyntaxKind.CharacterLiteralExpression))
                    return true;

                break;
            }
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            {
                if (expression.IsKind(SyntaxKind.NumericLiteralExpression))
                    return true;

                break;
            }
            case SpecialType.System_String:
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        return true;
                    case SyntaxKind.InterpolatedStringExpression:
                        return false; // todo
                }

                break;
            }
        }

        return false;
    }

    private static bool CanTypeBeConst(ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueType ||
               typeSymbol.SpecialType == SpecialType.System_String ||
               typeSymbol.TypeKind == TypeKind.Enum;
    }
}