using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SkillStack.Domain.DTOs;
using SkillStack.Domain.Entities;
using System.Linq;

namespace SkillStack.API.Controllers
{
    [ApiController]
    [Route("api/linq-playground")]
    public class LinqPlaygroundController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly MockDataService _mockData;

        public LinqPlaygroundController(IMediator mediator, MockDataService mockData)
        {
            _mediator = mediator;
            _mockData = mockData;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteQuery([FromBody] LinqRequest request)
        {
            try
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
                        "System.Collections.Generic"
                    );

                Console.WriteLine($"Query recebida: '{request.Query}'");
                string queryToExecute = request.Query.Trim().Replace(";", "");
                var scalarMethods = new[] { "Max", "Min", "Sum", "Average", "Count", "First", "FirstOrDefault", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
                bool containsScalar = scalarMethods.Any(method => queryToExecute.Contains($".{method}(") || queryToExecute.EndsWith($".{method}", StringComparison.OrdinalIgnoreCase));
                if (!containsScalar && !queryToExecute.EndsWith(".ToList()", StringComparison.OrdinalIgnoreCase))
                {
                    queryToExecute += ".ToList()";
                }
                Console.WriteLine($"Query ajustada: '{queryToExecute}'");

                dynamic result = await CSharpScript.EvaluateAsync(
                    queryToExecute,
                    scriptOptions,
                    globals: globals,
                    cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token
                );

                // Encapsula o resultado em um objeto JSON
                var response = new { Value = result };
                Console.WriteLine($"Resultado: {result}");
                return Ok(response);
            }
            catch (CompilationErrorException ex)
            {
                Console.WriteLine($"Erro de compilação: {ex.Message}");
                return BadRequest(new { Error = "Erro de compilação", Details = ex.Diagnostics });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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