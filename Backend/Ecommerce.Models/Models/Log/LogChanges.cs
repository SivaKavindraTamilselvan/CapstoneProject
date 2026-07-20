namespace Ecommerce.Models;
public class LogChanges
{
    public int LogChangesId {get;set;}
    public string TableName {get;set;} = string.Empty;
    public int RecordId {get;set;}
    public int Actions {get;set;}
    public string OldValue {get;set;} = string.Empty;
    public string NewValue {get;set;} = string.Empty;
    public int UserId {get;set;}
    public User? Users {get;set;}
    public DateTime ChangedAt {get;set;}
}