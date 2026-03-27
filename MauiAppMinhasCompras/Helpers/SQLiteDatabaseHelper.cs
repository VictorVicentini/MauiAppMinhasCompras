using SQLite;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);

            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=?, Categoria=? WHERE Id=?";

            return _conn.QueryAsync<Produto>(
                sql, p.Descricao, p.Quantidade, p.Preco, p.Categoria, p.Id 
            );
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * From Produto WHERE descricao LIKE '%" + q + "%'";

            return _conn.QueryAsync<Produto>(sql);
        }

        
        public Task<List<Produto>> SearchByCategoria(string categoria)
        {
            string sql = "SELECT * FROM Produto WHERE Categoria = ?";
            return _conn.QueryAsync<Produto>(sql, categoria);
        }

        public Task<List<RelatorioCategoria>> GetRelatorioPorCategoria()
        {
            string sql = "SELECT Categoria, SUM(Quantidade * Preco) AS Total FROM Produto GROUP BY Categoria";
            return _conn.QueryAsync<RelatorioCategoria>(sql);
        }


        public class CategoriaAux
        {
            public string Categoria { get; set; }
        }

        public async Task<List<string>> GetCategorias()
        {
            var result = await _conn.QueryAsync<CategoriaAux>(
                "SELECT DISTINCT Categoria FROM Produto ORDER BY Categoria"
            );
            return result.Select(c => c.Categoria).ToList();
        }




    }
}