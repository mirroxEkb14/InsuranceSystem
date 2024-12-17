#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
#endregion

namespace InsuranceSystemDemo.Utils;

public class CurrentSession
{
    public static int? UserId { get; private set; }
    public static string Username { get; private set; } = string.Empty;

    public static void SetSession(User user)
    {
        UserId = user.Id;
        Username = user.Username;
    }

    public static void ClearSession()
    {
        UserId = null;
        Username = string.Empty;
    }

    public static void SetDatabaseSession(DatabaseContext context)
    {
        if (UserId == null)
            throw new InvalidOperationException("User ID is not set in the session.");

        context.Database.ExecuteSqlRaw(
            "BEGIN DBMS_SESSION.SET_CONTEXT('USER_SESSION_CONTEXT', 'CURRENT_USER_ID', :p_user_id); END;",
            new OracleParameter("p_user_id", UserId)
        );
    }

    public static void SetSessionUser(DatabaseContext context, User user)
    {
        // Get current session ID from the database
        var sessionId = GetSessionId(context);

        // Insert user ID into SESSION_USER_CONTEXT table
        context.Database.ExecuteSqlRaw(
            "BEGIN " +
            "MERGE INTO USER_SESSION USING DUAL ON (SESSION_ID = :p_session_id) " +
            "WHEN MATCHED THEN UPDATE SET USER_ID = :p_user_id " +
            "WHEN NOT MATCHED THEN INSERT (SESSION_ID, USER_ID) VALUES (:p_session_id, :p_user_id);" +
            "END;",
            new OracleParameter("p_session_id", sessionId),
            new OracleParameter("p_user_id", user.Id)
        );

        SetSession(user);
    }

    public static string GetSessionId(DatabaseContext context)
    {
        var sessionIdParam = new OracleParameter("p_session_id", OracleDbType.Varchar2, 100)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        context.Database.ExecuteSqlRaw(
            "BEGIN :p_session_id := SYS_CONTEXT('USERENV', 'SESSIONID'); END;",
            sessionIdParam
        );

        return sessionIdParam.Value.ToString()!;
    }

    public static void ClearSessionInDatabase(DatabaseContext context)
    {
        var sessionId = GetSessionId(context);
        context.Database.ExecuteSqlRaw(
            "DELETE FROM USER_SESSION WHERE SESSION_ID = :p_session_id",
            new OracleParameter("p_session_id", sessionId)
        );
        ClearSession();
    }

    public static void SetTemporarySession(DatabaseContext context)
    {
        const int TemporaryUserId = -1;

        context.Database.ExecuteSqlRaw(
            "INSERT INTO USER_SESSION (SESSION_ID, USER_ID) VALUES (:p_session_id, :p_user_id)",
            new OracleParameter("p_session_id", GetSessionId(context)),
            new OracleParameter("p_user_id", TemporaryUserId)
        );
    }
}
