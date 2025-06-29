﻿using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
{
    public class UsuarioRepositorio : ICrud<Usuario>
    {
        private MySqlConnection conn;

        public Usuario? BuscarPorEmail(string email)
        {
            using var conn = ConnectionDB.GetConnection();
            string query = "SELECT * FROM usuario WHERE email = @Email";
            return conn.QueryFirstOrDefault<Usuario>(query, new { Email = email });
        }

        public long Create(Usuario pUsuario)
        {
            using var conn = ConnectionDB.GetConnection();
            return conn.Insert<Usuario>(pUsuario);
        }

        public void Delete(int id)
        {
            using var conn = ConnectionDB.GetConnection();
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> GetAll()
        {
            using var conn = ConnectionDB.GetConnection();
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> GetAllByInstituicao(int id)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                string sql = "SELECT * FROM usuario WHERE instituicao_id = @idInstituicao";
                return conn.Query<Usuario>(sql, new { idInstituicao = id }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuarios: {ex.Message}");
            }
        }

        public Usuario? GetById(int id)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                return conn.Get<Usuario>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuário: {ex.Message}");
            }
        }

        public void Update(Usuario pUsuario)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                conn.Update(pUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar usuário: {ex.Message}");
            }
        }

        public void UpdateDoador(Usuario pUsuario)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                var usuarioAtual = conn.Get<Usuario>(pUsuario.id);
                if (usuarioAtual == null)
                    throw new Exception("Usuário não encontrado");

                usuarioAtual.nome = pUsuario.nome ?? usuarioAtual.nome;
                usuarioAtual.email = pUsuario.email ?? usuarioAtual.email;
                usuarioAtual.telefone = pUsuario.telefone ?? usuarioAtual.telefone;
                usuarioAtual.senha = pUsuario.senha ?? usuarioAtual.senha;

                conn.Update(usuarioAtual);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }

        public Usuario? GetByIdComEndereco(int id)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                string query = @"
            SELECT 
                id, nome, email, telefone, role,
                logradouro, numero, complemento, 
                bairro, cidade, uf, cep
            FROM usuario 
            WHERE id = @Id";

                return conn.QueryFirstOrDefault<Usuario>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuário com endereço: {ex.Message}");
            }
        }
    }
}