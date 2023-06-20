using System.ComponentModel.DataAnnotations;
using keevotec.Contracts.Movimento;
using keevotec.Data;
using keevotec.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace keevotec.Services.Movimento;

public class MovimentoService : IMovimentoService
{
    private readonly KeevotecContext _dbContext;

    public MovimentoService(KeevotecContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MovimentoResponse> CreateMovimento(CreateMovimentoRequest request)
    {
        var produto = await _dbContext.Produtos.FindAsync(request.ProdutoId);

        if (produto == null)
        {
            throw new ValidationException($"Produto com id {request.ProdutoId} não existente.");
        }

        var ultimaSequencia = await _dbContext.Movimentos
            .Where(m => m.ProdutoId == request.ProdutoId && m.DataHora <= request.DataLancamento)
            .OrderByDescending(m => m.Sequencia)
            .Select(m => new { Sequencia = m.Sequencia, Atual = m.Atual })
            .FirstOrDefaultAsync();

        var movimento = new Models.Movimento
        {
            Ajuste = request.Ajuste,
            Atual = (ultimaSequencia?.Atual ?? 0) + request.Ajuste,
            Descricao = request.Descricao,
            DataHora = request.DataLancamento,
            Sequencia = (ultimaSequencia?.Sequencia ?? 0) + 1,
            ProdutoId = request.ProdutoId,
            Produto = produto
        };
        var movimentosToUpdate = await _dbContext.Movimentos
            .Where(m => m.ProdutoId == movimento.ProdutoId && m.Sequencia >= movimento.Sequencia)
            .ToListAsync();

        if (movimentosToUpdate.Count > 0)
        {
            foreach (var movimentoToUpdate in movimentosToUpdate)
            {
                movimentoToUpdate.Sequencia += 1;
                movimentoToUpdate.Atual += movimento.Ajuste;
            }
        }
        _dbContext.Add(movimento);
        await _dbContext.SaveChangesAsync();

        return new MovimentoResponse(
            Id: movimento.Id,
            ProdutoId: movimento.ProdutoId,
            ProdutoDescricao: movimento.Produto.Descricao,
            Ajuste: movimento.Ajuste,
            Atual: movimento.Atual,
            UM: movimento.Produto.UM,
            Descricao: movimento.Descricao,
            Sequencia: movimento.Sequencia,
            DataLancamento: movimento.DataHora);
    }

    public async Task DeleteMovimento(int id)
    {
        var movimento = await _dbContext.Movimentos.FindAsync(id);
        if (movimento == null)
        {
            throw new NotFoundException($"Movimento com id {id} não encontrado.");
        }

        _dbContext.Movimentos.Remove(movimento);
        
        var movimentosToUpdate = await _dbContext.Movimentos
            .Where(m => m.ProdutoId == movimento.ProdutoId && m.Sequencia > movimento.Sequencia)
            .ToListAsync();

        if (movimentosToUpdate.Count > 0)
        {
            foreach (var movimentoToUpdate in movimentosToUpdate)
            {
                movimentoToUpdate.Sequencia -= 1;
                movimentoToUpdate.Atual -= movimento.Ajuste;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<MovimentoResponse> GetMovimento(int id)
    {
        var movimento = await _dbContext.Movimentos
            .Include(m => m.Produto)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movimento == null)
        {
            throw new NotFoundException($"Movimento com id {id} não encontrado.");
        }
        
        return new MovimentoResponse(
            Id: movimento.Id,
            ProdutoId: movimento.ProdutoId,
            ProdutoDescricao: movimento.Produto.Descricao,
            Ajuste: movimento.Ajuste,
            Atual: movimento.Atual,
            UM: movimento.Produto.UM,
            Descricao: movimento.Descricao,
            Sequencia: movimento.Sequencia,
            DataLancamento: movimento.DataHora);
    }

    public async Task<List<MovimentoResponse>> GetMovimentos(GetMovimentosRequest request)
    {
        var query = _dbContext.Movimentos
            .Include(m => m.Produto)
            .AsQueryable();

        if (request.ProdutoId != null)
        {
            query = query.Where(m => m.ProdutoId == request.ProdutoId);
        }

        if (request.DataInicio != null)
        {
            query = query.Where(m => m.DataHora >= request.DataInicio.GetValueOrDefault().Date.AddHours(3));
        }

        if (request.DataFim != null)
        {
            query = query.Where(m => m.DataHora < request.DataFim.GetValueOrDefault().Date.AddDays(1).AddHours(3));
        }

        var movimentos = await query
            .OrderBy(m => m.DataHora)
            .Select(m => new MovimentoResponse(m.Id,
                m.ProdutoId,
                m.Produto.Descricao,
                m.Ajuste,
                m.Atual,
                m.Produto.UM,
                m.Descricao,
                m.Sequencia,
                m.DataHora))
            .ToListAsync();
        return (movimentos);
    }

    public async Task UpdateMovimento(int id, UpdateMovimentoRequest request)
    {
        var movimento = await _dbContext.Movimentos.FindAsync(id);
        if (movimento == null)
        {
            throw new NotFoundException($"Movimento com id {id} não encontrado.");
        }
        
        if (request.DataLancamento != null)
        {
            if (request.DataLancamento < movimento.DataHora)
            {
                var ultimaSequencia = await _dbContext.Movimentos
                    .Where(m => m.ProdutoId == movimento.ProdutoId && m.DataHora <= request.DataLancamento)
                    .OrderByDescending(m => m.Sequencia)
                    .Select(m => new { Sequencia = m.Sequencia, Atual = m.Atual })
                    .FirstOrDefaultAsync();

                int sequenciaNova = (ultimaSequencia?.Sequencia ?? 0) + 1;
                
                var movimentosToUpdate = await _dbContext.Movimentos
                    .Where(m => m.ProdutoId == movimento.ProdutoId && m.Sequencia >= sequenciaNova && m.Sequencia < movimento.Sequencia)
                    .ToListAsync();
                
                if (movimentosToUpdate.Count > 0)
                {
                    foreach (var movimentoToUpdate in movimentosToUpdate)
                    {
                        movimentoToUpdate.Sequencia += 1;
                        movimentoToUpdate.Atual += movimento.Ajuste;
                    }
                }

                _dbContext.Remove(movimento);
                await _dbContext.SaveChangesAsync();
                
                movimento.Atual = (ultimaSequencia?.Atual ?? 0) + movimento.Ajuste;
                movimento.DataHora = request.DataLancamento.GetValueOrDefault();
                movimento.Sequencia = sequenciaNova;
                _dbContext.Add(movimento);
                await _dbContext.SaveChangesAsync();
            }
            else if (request.DataLancamento > movimento.DataHora)
            {
                var ultimaSequencia = await _dbContext.Movimentos
                    .Where(m => m.ProdutoId == movimento.ProdutoId && m.DataHora <= request.DataLancamento)
                    .OrderByDescending(m => m.Sequencia)
                    .Select(m => new { Sequencia = m.Sequencia, Atual = m.Atual })
                    .FirstOrDefaultAsync();

                int sequenciaNova = (ultimaSequencia?.Sequencia ?? 0);
                
                var movimentosToUpdate = await _dbContext.Movimentos
                    .Where(m => m.ProdutoId == movimento.ProdutoId && m.Sequencia <= sequenciaNova && m.Sequencia > movimento.Sequencia)
                    .ToListAsync();

                if (movimentosToUpdate.Count > 0)
                {
                    foreach (var movimentoToUpdate in movimentosToUpdate)
                    {
                        movimentoToUpdate.Sequencia -= 1;
                        movimentoToUpdate.Atual -= movimento.Ajuste;
                    }
                }
                _dbContext.Remove(movimento);
                await _dbContext.SaveChangesAsync();
                
                movimento.Atual = (ultimaSequencia?.Atual ?? movimento.Ajuste);
                movimento.DataHora = request.DataLancamento.GetValueOrDefault();
                movimento.Sequencia = sequenciaNova;
                _dbContext.Add(movimento);
                await _dbContext.SaveChangesAsync();
            }
        }

        if (request.Ajuste != null)
        {
            double diferenca = (request.Ajuste ?? 0) - movimento.Ajuste;
            
            movimento.Ajuste = (request.Ajuste ?? 0);
            movimento.Atual += diferenca;

            var movimentosToUpdate = await _dbContext.Movimentos
                .Where(m => m.ProdutoId == movimento.ProdutoId && m.Sequencia > movimento.Sequencia)
                .ToListAsync();

            if (movimentosToUpdate.Count > 0)
            {
                foreach (var movimentoToUpdate in movimentosToUpdate)
                {
                    movimentoToUpdate.Atual += diferenca;
                }
            }
        }
        
        if (request.Descricao != null)
        {
            movimento.Descricao = request.Descricao;
        }
        
        await _dbContext.SaveChangesAsync();
    }
}
