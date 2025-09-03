using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MyKaraoke.View.Components;

namespace MyKaraoke.View.Interceptors
{
    /// <summary>
    /// ✅ INTERCEPTADOR: Mostra loading automaticamente em todas as operações de banco
    /// 🎯 AUTOMÁTICO: Sem necessidade de código manual nos services
    /// 🔄 INTELIGENTE: Detecta tipo de operação (SELECT, INSERT, UPDATE, DELETE)
    /// </summary>
    public class DatabaseLoadingInterceptor : DbCommandInterceptor
    {
        private static readonly Dictionary<string, string> OperationMessages = new()
        {
            { "SELECT", "Carregando dados..." },
            { "INSERT", "Salvando..." },
            { "UPDATE", "Atualizando..." },
            { "DELETE", "Excluindo..." },
            { "CREATE", "Criando..." },
            { "DROP", "Removendo..." },
            { "ALTER", "Modificando..." }
        };

        private static readonly HashSet<string> QuickOperations = new()
        {
            "PRAGMA", // SQLite metadata
            "SELECT sqlite_version", // Version checks
            "SELECT COUNT(*) FROM", // Quick counts
            "SELECT 1 FROM", // Existence checks
        };

        #region Command Execution Interception

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos síncronos
        /// </summary>
        public override DbDataReader ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            ShowLoadingForCommand(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos assíncronos (mais comum no MAUI)
        /// </summary>
        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            await ShowLoadingForCommandAsync(command);
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos NonQuery síncronos
        /// </summary>
        public override int NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            ShowLoadingForCommand(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos NonQuery assíncronos
        /// </summary>
        public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await ShowLoadingForCommandAsync(command);
            return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos Scalar síncronos
        /// </summary>
        public override object ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            ShowLoadingForCommand(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Comandos Scalar assíncronos
        /// </summary>
        public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default)
        {
            await ShowLoadingForCommandAsync(command);
            return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        #endregion

        #region Command Completion Interception

        /// <summary>
        /// 🔄 ESCONDE: Loading após comando síncrono concluído
        /// </summary>
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            HideLoadingForCommand(command);
            return base.ReaderExecuted(command, eventData, result);
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após comando assíncrono concluído
        /// </summary>
        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            await HideLoadingForCommandAsync(command);
            return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após NonQuery síncrono concluído
        /// </summary>
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            HideLoadingForCommand(command);
            return base.NonQueryExecuted(command, eventData, result);
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após NonQuery assíncrono concluído
        /// </summary>
        public override async ValueTask<int> NonQueryExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            await HideLoadingForCommandAsync(command);
            return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após Scalar síncrono concluído
        /// </summary>
        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            HideLoadingForCommand(command);
            return base.ScalarExecuted(command, eventData, result);
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após Scalar assíncrono concluído
        /// </summary>
        public override async ValueTask<object> ScalarExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            object result,
            CancellationToken cancellationToken = default)
        {
            await HideLoadingForCommandAsync(command);
            return await base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// 🛡️ ERRO: Esconde loading em caso de erro
        /// </summary>
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            HideLoadingForCommand(command);
            base.CommandFailed(command, eventData);
        }

        /// <summary>
        /// 🛡️ ERRO: Esconde loading em caso de erro assíncrono
        /// </summary>
        public override async Task CommandFailedAsync(
            DbCommand command,
            CommandErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            await HideLoadingForCommandAsync(command);
            await base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        #endregion

        #region Loading Management

        /// <summary>
        /// 🔄 MOSTRA: Loading baseado no comando SQL
        /// </summary>
        private void ShowLoadingForCommand(DbCommand command)
        {
            _ = Task.Run(async () => await ShowLoadingForCommandAsync(command));
        }

        /// <summary>
        /// 🔄 MOSTRA: Loading baseado no comando SQL (async)
        /// </summary>
        private async Task ShowLoadingForCommandAsync(DbCommand command)
        {
            try
            {
                var sql = command.CommandText?.Trim();
                if (string.IsNullOrEmpty(sql))
                {
                    return;
                }

                // 🛡️ SKIP: Operações muito rápidas que não precisam de loading
                if (IsQuickOperation(sql))
                {
                    System.Diagnostics.Debug.WriteLine($"🏃 DatabaseInterceptor: Operação rápida - sem loading: {sql.Substring(0, Math.Min(50, sql.Length))}...");
                    return;
                }

                // 🎯 DETECTA: Tipo de operação
                var operation = GetOperationType(sql);
                var message = OperationMessages.GetValueOrDefault(operation, "Processando...");

                System.Diagnostics.Debug.WriteLine($"🔄 DatabaseInterceptor: Mostrando loading para {operation}: {message}");
                System.Diagnostics.Debug.WriteLine($"🔍 SQL: {sql.Substring(0, Math.Min(100, sql.Length))}...");

                await GlobalLoadingOverlay.ShowLoadingAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DatabaseInterceptor: Erro ao mostrar loading: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após comando
        /// </summary>
        private void HideLoadingForCommand(DbCommand command)
        {
            _ = Task.Run(async () => await HideLoadingForCommandAsync(command));
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após comando (async)
        /// </summary>
        private async Task HideLoadingForCommandAsync(DbCommand command)
        {
            try
            {
                var sql = command.CommandText?.Trim();
                if (string.IsNullOrEmpty(sql) || IsQuickOperation(sql))
                {
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"🔄 DatabaseInterceptor: Escondendo loading");
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DatabaseInterceptor: Erro ao esconder loading: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 🔍 DETECTA: Tipo de operação SQL
        /// </summary>
        private string GetOperationType(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return "UNKNOWN";

            var upperSql = sql.ToUpperInvariant().Trim();

            // 🎯 DETECTA: Primeira palavra do comando
            var firstWord = upperSql.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            return firstWord switch
            {
                "SELECT" => "SELECT",
                "INSERT" => "INSERT",
                "UPDATE" => "UPDATE",
                "DELETE" => "DELETE",
                "CREATE" => "CREATE",
                "DROP" => "DROP",
                "ALTER" => "ALTER",
                "MERGE" => "UPDATE", // MERGE é tipo de UPDATE
                "UPSERT" => "UPDATE", // UPSERT é tipo de UPDATE
                _ => "UNKNOWN"
            };
        }

        /// <summary>
        /// 🏃 VERIFICA: Se é operação rápida que não precisa de loading
        /// </summary>
        private bool IsQuickOperation(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return true;

            var upperSql = sql.ToUpperInvariant().Trim();

            // 🛡️ SKIP: Operações conhecidas como rápidas
            foreach (var quickOp in QuickOperations)
            {
                if (upperSql.StartsWith(quickOp))
                {
                    return true;
                }
            }

            // 🛡️ SKIP: Comandos muito curtos (provavelmente metadata)
            if (sql.Length < 20)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}