using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyVocaList.View.Components;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MyVocaList.View.Interceptors
{
    /// <summary>
    /// ✅ INTERCEPTOR: Automatically shows loading for all database operations
    /// 🎯 AUTOMATIC: No manual code needed in services
    /// 🔄 INTELLIGENT: Detects operation type (SELECT, INSERT, UPDATE, DELETE)
    /// ✂️ TRIMMING: Automatically trims all string parameters before query execution
    /// </summary>
    public class DatabaseLoadingInterceptor : DbCommandInterceptor
    {
        private static readonly Dictionary<string, string> OperationMessages = new()
        {
            { "SELECT", "Loading data..." },
            { "INSERT", "Saving..." },
            { "UPDATE", "Updating..." },
            { "DELETE", "Deleting..." },
            { "CREATE", "Creating..." },
            { "DROP", "Removing..." },
            { "ALTER", "Modifying..." }
        };

        private static readonly HashSet<string> QuickOperations = new()
        {
            "PRAGMA", // SQLite metadata
            "SELECT sqlite_version", // Version checks
            "SELECT COUNT(*) FROM", // Quick counts
            "SELECT 1 FROM", // Existence checks
        };

        // 🛡️ NOVO: Comandos de migração que devem ser ignorados
        private static readonly HashSet<string> MigrationOperations = new()
        {
            "__EFMigrationsLock",
            "__EFMigrationsHistory",
            "INSERT OR IGNORE INTO \"__EFMigrationsLock\"",
            "DELETE FROM \"__EFMigrationsLock\"",
            "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\""
        };

        #region Command Execution Interception

        /// <summary>
        /// 🎯 INTERCEPTS: Synchronous commands
        /// </summary>
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters
            ShowLoadingForCommand(command);
            return result;
        }

        /// <summary>
        /// 🎯 INTERCEPTS: Asynchronous commands (most common in MAUI)
        /// </summary>
        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters
            await ShowLoadingForCommandAsync(command);
            return result;
        }

        /// <summary>
        /// 🎯 INTERCEPTS: NonQuery synchronous commands
        /// </summary>
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters (SELECT only)
            ShowLoadingForCommand(command);
            return result;
        }

        /// <summary>
        /// 🎯 INTERCEPTS: NonQuery asynchronous commands
        /// </summary>
        public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters (SELECT only)
            await ShowLoadingForCommandAsync(command);
            return result;
        }

        /// <summary>
        /// 🎯 INTERCEPTS: Scalar synchronous commands
        /// </summary>
        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters (SELECT only)
            ShowLoadingForCommand(command);
            return result;
        }

        /// <summary>
        /// 🎯 INTERCEPTS: Scalar asynchronous commands
        /// </summary>
        public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default)
        {
            TrimStringParameters(command);  // ✂️ Auto-trim string parameters (SELECT only)
            await ShowLoadingForCommandAsync(command);
            return result;
        }

        #endregion

        #region Command Completion Interception

        /// <summary>
        /// 🔄 ESCONDE: Loading após comando síncrono concluído
        /// </summary>
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            HideLoadingForCommand(command);
            return result;
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
            return result;
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após NonQuery síncrono concluído
        /// </summary>
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            HideLoadingForCommand(command);
            return result;
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
            return result;
        }

        /// <summary>
        /// 🔄 ESCONDE: Loading após Scalar síncrono concluído
        /// </summary>
        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            HideLoadingForCommand(command);
            return result;
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
            return result;
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

                // 🛡️ SKIP: Comandos de migração do Entity Framework
                if (IsMigrationOperation(sql))
                {
                    Console.WriteLine($"🛡️ DatabaseInterceptor: Migration command ignored: {sql.Substring(0, Math.Min(50, sql.Length))}...");
                    return;
                }

                // 🛡️ SKIP: Operações muito rápidas que não precisam de loading
                if (IsQuickOperation(sql))
                {
                    Console.WriteLine($"🏃 DatabaseInterceptor: Quick operation - no loading needed: {sql.Substring(0, Math.Min(50, sql.Length))}...");
                    return;
                }

                // 🎯 DETECTA: Tipo de operação
                var operation = GetOperationType(sql);
                var message = OperationMessages.GetValueOrDefault(operation, "Processing...");

                // ✅ SISTEMA CENTRALIZADO: Solicita loading com baixa prioridade
                var requesterId = $"Database_{operation}_{DateTime.Now.Ticks}";

                Console.WriteLine($"🔄 DatabaseInterceptor: Requesting loading for {operation}: {message}");
                Console.WriteLine($"🔍 SQL: {sql.Substring(0, Math.Min(100, sql.Length))}...");

                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: message,
                    priority: LoadingPriority.Database,
                    context: LoadingContext.DatabaseOperation,
                    isPersistent: false,
                    autoHideAfter: TimeSpan.FromSeconds(30) // Auto-hide após 30s como segurança
                );

                // 🎯 ARMAZENA: RequesterId para poder remover depois
                command.SetRequesterId(requesterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DatabaseInterceptor: Erro ao solicitar loading: {ex.Message}");
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
                if (string.IsNullOrEmpty(sql))
                {
                    return;
                }

                // 🛡️ SKIP: Comandos de migração (não criam loading, então não precisam remover)
                if (IsMigrationOperation(sql))
                {
                    return;
                }

                // 🛡️ SKIP: Operações rápidas (não criam loading, então não precisam remover)
                if (IsQuickOperation(sql))
                {
                    return;
                }

                // 🎯 RECUPERA: RequesterId armazenado durante ShowLoadingForCommandAsync
                var requesterId = command.GetRequesterId();
                if (string.IsNullOrEmpty(requesterId))
                {
                    // Silent skip - no requesterId means loading was never shown
                    return;
                }

                Console.WriteLine($"🔄 DatabaseInterceptor: Removing loading for {requesterId}");
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);

                // 🧹 CLEANUP: Remove requesterId after use to prevent duplicate removal attempts
                command.ClearRequesterId();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DatabaseInterceptor: Error removing loading: {ex.Message}");
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

        /// <summary>
        /// 🛡️ VERIFICA: Se é comando de migração do Entity Framework
        /// </summary>
        private bool IsMigrationOperation(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return false;

            var upperSql = sql.ToUpperInvariant().Trim();

            // 🛡️ VERIFICA: Se contém qualquer operação de migração
            foreach (var migrationOp in MigrationOperations)
            {
                if (upperSql.Contains(migrationOp.ToUpperInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region String Parameter Trimming

        /// <summary>
        /// ✂️ AUTO-TRIMS: All string parameters before query execution
        /// This eliminates the need for manual .Trim() calls in repositories
        /// Applies to SELECT queries only (not INSERT/UPDATE to preserve user input)
        /// </summary>
        private static void TrimStringParameters(DbCommand command)
        {
            if (command == null || command.CommandText == null)
                return;

            // Only trim for SELECT queries (read operations)
            // Don't trim for INSERT/UPDATE to preserve exact user input during writes
            var commandText = command.CommandText.TrimStart().ToUpperInvariant();
            if (!commandText.StartsWith("SELECT"))
                return;

            // Trim all string parameters
            foreach (DbParameter parameter in command.Parameters)
            {
                if (parameter.Value is string stringValue && !string.IsNullOrEmpty(stringValue))
                {
                    parameter.Value = stringValue.Trim();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Extension methods to store RequesterId in DbCommand
    /// </summary>
    public static class DbCommandExtensions
    {
        private static readonly ConditionalWeakTable<DbCommand, string> _requesterIds = new();

        public static void SetRequesterId(this DbCommand command, string requesterId)
        {
            _requesterIds.AddOrUpdate(command, requesterId);
        }

        public static string GetRequesterId(this DbCommand command)
        {
            _requesterIds.TryGetValue(command, out var requesterId);
            return requesterId;
        }

        public static void ClearRequesterId(this DbCommand command)
        {
            _requesterIds.Remove(command);
        }
    }
}