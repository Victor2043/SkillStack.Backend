using MediatR;
using System;

namespace SkillStack.Domain.DTOs
{
    public class ExecuteLinqQuery : IRequest<object>
    {
        public string Query { get; set; }
    }
}