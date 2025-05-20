using MediatR;
using System;

namespace SkillStack.Application.Commands.ExecuteLinqQueryCommand
{
    public class ExecuteLinqQueryCommand : IRequest<object>
    {
        public string Query { get; set; }
    }
}