﻿using Api.AppDoar.Classes;
using Api.AppDoar.Classes.doador;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories.doador
{
    public class DoadorRepositorio : ICrud<Doador>
    {
        private MySqlConnection conn;

        public long Create(Doador pDoador)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                return conn.Insert<Doador>(pDoador);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar doador: {ex.Message}");
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Doador> GetAll()
        {
            throw new NotImplementedException();
        }

        public Doador? GetById(int id)
        {
            using var conn = ConnectionDB.GetConnection();
            try
            {
                return conn.QueryFirstOrDefault<Doador>("SELECT * FROM usuario WHERE id = @Id AND role = 'doador'", new { Id = id });
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erro no banco de dados. Satatus: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar doador: {ex.Message}");
            }
        }

        public void Update(Doador entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }

        public (Usuario usuario, Doador doador) CreateWithUsuario(Usuario usuario, Doador doador)
        {
            using var conn = ConnectionDB.GetConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                var usuarioId = conn.Insert(usuario, transaction);
                usuario.id = (int)usuarioId;

                doador.id = usuario.id;
                conn.Insert(doador, transaction);

                transaction.Commit();

                return (usuario, doador);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Erro ao cadastrar doador e usuário: {ex.Message}");
            }
        }

        public Usuario GetUsuarioCompleto(int id)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                var usuario = conn.QueryFirstOrDefault<Usuario>(
                    "SELECT * FROM usuario WHERE id = @Id",
                    new { Id = id });

                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado");
                }

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuário completo: {ex.Message}");
            }
        }

        public bool UpdateUsuario(Usuario usuario)
        {
            try
            {
                using var conn = ConnectionDB.GetConnection();
                return conn.Execute(
                    @"UPDATE usuario SET 
              logradouro = @logradouro,
              numero = @numero,
              complemento = @complemento,
              bairro = @bairro,
              cidade = @cidade,
              uf = @uf,
              cep = @cep
            WHERE id = @id",
                    usuario) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

    }
}
