using MediatR;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SkillStack.Application.Commands.ExecuteLinqQueryCommand;
using SkillStack.Domain.DTOs;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillStack.Application.Handlers
{
    public class ExecuteLinqQueryCommandHandler : IRequestHandler<ExecuteLinqQueryCommand, object>
    {
        private readonly MockDataService _mockData;

        public ExecuteLinqQueryCommandHandler(MockDataService mockData)
        {
            _mockData = mockData;
        }

        public async Task<object> Handle(ExecuteLinqQueryCommand request, CancellationToken cancellationToken)
        {
            var products = _mockData.GetProducts().AsQueryable();
            var productTypes = _mockData.GetProductTypes().AsQueryable();

            var globals = new Globals
            {
                products = products,
                types = productTypes
            };

            var scriptOptions = ScriptOptions.Default
                .WithReferences(
                    typeof(Product).Assembly,
                    typeof(Enumerable).Assembly,
                    typeof(Queryable).Assembly
                )
                                    .WithImports(
                    "System",
                    "System.Linq",
                    "System.Collections.Generic",
                    "SkillStack.Domain.Extensions"
                );

            Console.WriteLine($"Query recebida: '{request.Query}'");
            string queryToExecute = request.Query.Trim().Replace(";", "");

            var scalarMethods = new[] { "Max", "Min", "Sum", "Average", "Count", "First", "FirstOrDefault", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
            bool containsScalar = scalarMethods.Any(method =>
                queryToExecute.Contains($".{method}(") ||
                queryToExecute.EndsWith($".{method}", StringComparison.OrdinalIgnoreCase));

            if (!containsScalar && !queryToExecute.EndsWith(".ToList()", StringComparison.OrdinalIgnoreCase))
            {
                queryToExecute += ".ToList()";
            }

            Console.WriteLine($"Query ajustada: '{queryToExecute}'");

            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken).Token;

            try
            {
                dynamic result = await CSharpScript.EvaluateAsync(
                    queryToExecute,
                    scriptOptions,
                    globals: globals,
                    cancellationToken: linkedToken
                );

                Console.WriteLine($"Resultado: {result}");
                return result;
            }
            catch (CompilationErrorException ex)
            {
                Console.WriteLine($"Erro de compilação: {ex.Message}");
                throw new ApplicationException("Erro de compilação: " + string.Join(", ", ex.Diagnostics.Select(d => d.GetMessage())), ex);
            }
            catch (OperationCanceledException)
            {
                throw new ApplicationException("A operação excedeu o tempo limite de execução.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao executar a query: {ex.Message}", ex);
            }
        }

        // Classe para as variáveis globais acessíveis no script
        public class Globals
        {
            public IQueryable<Product> products { get; set; }
            public IQueryable<ProductType> types { get; set; }
        }
    }
}