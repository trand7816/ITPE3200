namespace itpe3200.Services;

//enumeration, used to give a variable a predefined result instead of true or false
public enum GroupJoinResult
{
    Success, 
    GroupNotFound, 
    StudentNotFound, 
    GroupFull, 
    AlreadyInSession, 

    //Random assignment: every group in the session is full
    SessionFull,
    DatabaseError
}