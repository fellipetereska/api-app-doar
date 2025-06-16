
## ⚙️ README - Backend (C# .NET + Dapper)

# DOAR - Plataforma de Doações (Backend)

Este repositório contém o backend da plataforma **DOAR**, responsável por gerenciar autenticação, regras de negócio, controle de doações, ONGs, assistidos e integrações com banco de dados e geração de PDF.

## 🧰 Tecnologias

- ASP.NET Core
- Dapper (micro ORM)
- SQL Server
- JWT para autenticação
- Swagger para documentação de API

## 📦 Funcionalidades

- Cadastro e autenticação de usuários (CPF/CNPJ)
- CRUD de doações, instituições e assistidos
- Geração de termo de doação em PDF
- Listagem e controle de estoque
- API RESTful documentada com Swagger

## 🗂️ Estrutura do Projeto
├── Controllers/ # Endpoints da API
├── Classes/ # Entidades que representam as tabelas do banco
├── DTOs/ # Objetos de Transferência de Dados entre camadas
├── Repositories/ # Interação com o banco de dados via Dapper
├── Services/ # Regras de negócio centralizadas
├── PersistenciaDB/ # Configuração da conexão e contexto com o banco
├── Utils/ # Funções auxiliares (validações, formatações, helpers)
├── Uploads/ # Imagens armazenadas localmente
├── Reports/ # Templates e lógica de geração de documentos PDF
└── Program.cs # Ponto de entrada da aplicação

## 📄 Requisitos

- .NET 6.0+
- SQL Server
- Visual Studio ou VS Code

## 🔄 Como rodar

# Clonar o repositório
git clone https://github.com/seu-usuario/doar-backend.git
cd doar-backend

# Restaurar pacotes
dotnet restore

# Rodar a aplicação
dotnet run

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=doar_db;User Id=usuario;Password=senha;"
  },
  "Jwt": {
    "Key": "chave-secreta-super-segura",
    "Issuer": "doar-api"
  }
}
