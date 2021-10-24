using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crono.Configuration.Log;
using Crono.Model;
using Dapper;

namespace Crono.Repository
{
    /// <summary>
    /// Repository MSSQL
    /// </summary>
    public class MSSqlRepository : IRepository
    {
        private string _connectionString;
        private LogSplitter _log;

        public MSSqlRepository(string connectionString, LogSplitter log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        private IDbConnection GetConnection() => new SqlConnection(_connectionString);

        public Task<bool> DeleteTask(CronoTask task)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all phases of a commission
        /// </summary>
        public async Task<List<CronoTask>> GetAllTask(string codiceCommessa)
        {
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    string query = @"SELECT [Id]
                    ,[Commessa] AS CodiceCommessa
                    ,[IdFase]
                    ,[Intervento]
                    ,[StartDate]
                    ,[EndDate]
                    ,[Obsoleto]
                    ,[Timestamp]
                    FROM[Crono_fase] WHERE Commessa=@codiceCommessa;
                    SELECT DISTINCT[IdSource] As S
                    ,[IdDest] AS D
                    FROM[Crono_constraints] A
                    JOIN[Crono_fase] B ON A.IdSource = b.Id WHERE B.Commessa = @codiceCommessa";
                    using (var multi = await connection.QueryMultipleAsync(query, new { codiceCommessa }))
                    {
                        List<CronoTask> result = multi.Read<CronoTask>().AsList();
                        List<dynamic> constraints = multi.Read<dynamic>().AsList();
                        result.ForEach(i =>
                        i.Constraints = new ObservableCollection<CronoTask>((result.Where(f => constraints.Where(w => w.S == i.Id && f.Id == w.D).ToList().Count > 0))));
                        return result;
                    }
                }
            }catch(Exception e)
            {
                _log.Error($"Errore durante il recupero delle fasi per la commessa {codiceCommessa}", e);
                return new List<CronoTask>();
            }
        }

        /// <summary>
        /// Get all commissions
        /// </summary>
        public async Task<List<Commessa>> GetCommesse()
        {
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    string query = @"SELECT DISTINCT A.[RifAcquisizione] as Codice, A.[DataRegistrazione], A.[Cantiere],
                        A.FineLavori as Chiusa, A.Intervento, A.com_manut as Manutenzione, B.Nominativo as Tecnico
                    FROM [Tabella acquisizioni] A LEFT JOIN
                    Anagrafica_Tecnici B On A.IDResponsabile = B.IDTecnico
                    WHERE A.RifAcquisizione NOT LIKE '%/%'
                    ORDER BY DataRegistrazione DESC";
                    return (await connection.QueryAsync<Commessa>(query)).ToList();
                }
            }catch(Exception e)
            {
                _log.Error($"Errore durante il recupero delle commesse", e);
                return new List<Commessa>();
            }
        }

        /// <summary>
        /// Get all commissions between two dates
        /// </summary>
        public async Task<List<CronoTask>> GetAllCommesseBetween(DateTime d1, DateTime d2)
        {
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    string query = @"SELECT [Id]
                    ,[Commessa] AS CodiceCommessa
                    ,[IdFase]
                    ,[Intervento]
                    ,[StartDate]
                    ,[EndDate]
                    ,[Obsoleto]
                    ,[Timestamp]
                    FROM crono_fase WHERE (StartDate>=@from and StartDate<=@to) OR (EndDate>=@from and EndDate<=@to)
                    order by Commessa;
                    SELECT DISTINCT [IdSource] As S
                   ,[IdDest] AS D
                    FROM[Crono_constraints] A
                    JOIN[Crono_fase] B ON A.IdSource = b.Id WHERE (B.StartDate>=@from and B.StartDate<=@to) OR (B.EndDate>=@from and B.EndDate<=@to)";
                    using (var multi = await connection.QueryMultipleAsync(query, new { from = d1, to = d2 }))
                    {
                        List<CronoTask> result = multi.Read<CronoTask>().AsList();
                        List<dynamic> constraints = multi.Read<dynamic>().AsList();
                        result.ForEach(i =>
                        i.Constraints = new ObservableCollection<CronoTask>((result.Where(f => constraints.Where(w => w.S == i.Id && f.Id == w.D).ToList().Count > 0))));
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Errore durante il recupero delle commesse", e);
                return new List<CronoTask>();
            }
        }

        public Task<List<CronoTask>> GetTaskBetween(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert a new phase
        /// </summary>
        public async Task<int> InsertTask(CronoTask task)
        {
            try
            {
                string query = @"INSERT INTO [dbo].[Crono_fase]
               ([Commessa]
               ,[IdFase]
               ,[Intervento]
               ,[StartDate]
               ,[EndDate]
               ,[Obsoleto]
               ,[Timestamp])
                 VALUES
                (@commessa
                ,@idFase
                ,@intervento
                ,@startDate
                ,@endDate
                ,@obsoleto
                ,@timestamp)
                SELECT CAST(SCOPE_IDENTITY() as int)";
                string updateFase = "UPDATE Crono_fase SET IdFase = @newId WHERE Id = @newId";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        int newId = (await connection.QueryAsync<int>(query,
                        new { commessa = task.CodiceCommessa, idFase = task.IdFase, startDate = task.StartDate, intervento = task.Intervento, endDate = task.EndDate, obsoleto = 0, timestamp = DateTime.Now }, transaction: transaction)).Single();
                        if (task.IdFase <= 0)
                            await connection.ExecuteAsync(updateFase, new { newId }, transaction: transaction); //viene settato l'id fasi pari all'identity se non fa parte di una fase già esistente
                        transaction.Commit();
                        return newId;
                    }

                }
            }catch(Exception e)
            {
                _log.Error($"Errore durante l'inserimento del task id: {task.Id} e intervento: {task.Intervento}", e);
                return -1;
            }
        }

        /// <summary>
        /// Insert new constraint
        /// </summary>
        public async Task InsertConstraints(CronoTask task)
        {
            try
            {
                string queryConstraints = @"INSERT INTO [Crono_constraints]
                ([IdSource],[IdDest])
                VALUES (@idSource,@idDest)";

                using (var connection = GetConnection())
                {
                    foreach (var item in task.Constraints)
                        await connection.ExecuteAsync(queryConstraints, new { idSource = task.Id, idDest = item.Id });
                }
            }catch(Exception e)
            {
                _log.Error($"Errore durante l'inserimento dei vincoli del task id: {task.Id} e intervento: {task.Intervento}", e);                
            }
            
        }

        /// <summary>
        /// Update phase
        /// </summary>
        public async Task<int> UpdateTask(CronoTask task)
        {
            try
            {
                string query = @"UPDATE [dbo].[Crono_fase]
                   SET [Commessa] = @commessa
                      ,[Intervento] = @intervento
                      ,[StartDate] = @startDate
                      ,[EndDate] = @endDate
                      ,[Obsoleto] = @obsoleto
                      ,[Timestamp] = @timestamp
                 WHERE Id = @id";
                //vengono eliminati tutti i vincoli non più presenti nella fase
                string deleteConstraint = @"DELETE FROM [Crono_Constraints] WHERE IdSource=@idSource AND IdDest NOT IN (@idDest)";
                string insertConstraints = @"IF NOT EXISTS (SELECT IdSource,IdDest FROM [Crono_Constraints] WHERE IdSource = @idSource AND IdDest = @idDest) 
                    BEGIN
                        INSERT INTO [Crono_Constraints] VALUES (@idSource, @idDest)
                    END";

                using (var connection = GetConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        int rows = (await connection.ExecuteAsync(query,
                        new {
                            id = task.Id,
                            commessa = task.CodiceCommessa,
                            intervento = task.Intervento,
                            startDate = task.StartDate,
                            endDate = task.EndDate,
                            obsoleto = 0,
                            timestamp = DateTime.Now
                        }, 
                        transaction: transaction));
                        foreach (var item in task.Constraints)
                        {
                            await connection.ExecuteAsync(insertConstraints, new { idSource = task.Id, idDest = item.Id }, transaction: transaction);
                        }
                        await connection.ExecuteAsync(deleteConstraint, new { idSource = task.Id, idDest = task.Constraints.Select(s => s.Id) }, transaction: transaction);
                        transaction.Commit();
                        return rows;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Errore durante l'aggiornamento del task id: {task.Id} e intervento: {task.Intervento}", e);
                return 0;
            }
        }
    }
}
