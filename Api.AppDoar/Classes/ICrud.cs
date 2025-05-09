namespace Api.AppDoar.Classes
{
    public interface ICrud<T>
    {
        T? GetById(int id);         // Buscar por ID
        IEnumerable<T> GetAll();    // Listar Todos
        long Create(T entidade);    // Inserir
        void Update(T entidade);    // Atualizar
        void Delete(int id);        // Deletar
        void UpdateStatus(int id);  // Alterar Status
    }
}
