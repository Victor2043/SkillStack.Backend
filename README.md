

## 🇧🇷 PT-BR - README.md

# 🚀 SkillStack.Backend

Backend do projeto **SkillStack**, construído em .NET com arquitetura baseada em  **CQRS** (Command Query Responsibility Segregation), visando escalabilidade, manutenção facilitada e separação clara de responsabilidades.

---

## 🏗️ Arquitetura

O projeto está estruturado seguindo os princípios de **Clean Architecture** e **DDD**:


---

## 🛠️ Padrões e Tecnologias

✅ **CQRS**  
- Separação clara entre comandos (escritas) e consultas (leituras).  
- Implementado com MediatR para distribuição de requisições.  

✅ **Entity Framework Core**  
- ORM para persistência de dados.  
- Migrations para versionamento do banco de dados.  

✅ **Injeção de Dependência**  
- Serviços e repositórios configurados via DI nativa do .NET.  

✅ **FluentValidation**  
- Validação desacoplada dos controllers.  

✅ **MediatR**  
- Padrão Mediator para orquestração de comandos e consultas.  

---

## 🐳 Docker

O projeto está preparado para ser **containerizado** com **Docker**, facilitando o deploy e execução em diversos ambientes.

### ✅ Como rodar com Docker:

1. **Build da imagem:**

docker build -t skillstack-backend .

docker run -d -p 5000:80 --name skillstack skillstack-backend





## 🇺🇸 EN-US - README.md


# 🚀 SkillStack.Backend

Backend for the **SkillStack** project, built with .NET following **CQRS** (Command Query Responsibility Segregation) architectures, aiming for scalability, maintainability, and clear separation of concerns.

---

## 🏗️ Architecture

The project is structured following **Clean Architecture** and **DDD** principles:


---

## 🛠️ Patterns and Technologies

✅ **CQRS**  
- Clear separation between Commands (writes) and Queries (reads).  
- Implemented using MediatR for request distribution.  

 

✅ **Entity Framework Core**  
- ORM for data persistence.  
- Migrations for database versioning.  

✅ **Dependency Injection**  
- Services and repositories configured via .NET's native DI.  

✅ **FluentValidation**  
- Decoupled validation from controllers.  

✅ **MediatR**  
- Mediator pattern for orchestration of Commands and Queries.  

---

## 🐳 Docker

The project is prepared for **containerization** with **Docker**, making deployment and execution in different environments easier.

### ✅ Running with Docker:

1. **Build the image:**

docker build -t skillstack-backend .

docker run -d -p 5000:80 --name skillstack skillstack-backend
